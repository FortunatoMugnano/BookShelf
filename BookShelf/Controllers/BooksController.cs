using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShelf.Data;
using BookShelf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BookShelf.Models.ViewModels;

namespace BookShelf.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        public async Task<IActionResult> Index([FromQuery]string search)
        {

            if (!string.IsNullOrWhiteSpace(search))
            {
                var user = await GetCurrentUserAsync();
                var books = _context.Book
                    .Where(a => a.ApplicationUserId == user.Id)
                    .Where(a => a.Title.Contains(search) || a.Author.Name.Contains(search))
                    .Include(b => b.ApplicationUser)
                    .Include(b => b.Author)
                    .Include(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);
                return View(books);

            }
            else
            {
                var user = await GetCurrentUserAsync();
                var books = _context.Book
                    .Where(a => a.ApplicationUserId == user.Id)
                    .Include(b => b.ApplicationUser)
                    .Include(b => b.Author)
                    .Include(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);


                return View(books);
            }
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                 .Include(b => b.Comments)
                 .Include(b => b.BookGenres)
                    .ThenInclude(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            var user = await GetCurrentUserAsync();
            var authors = _context.Author.Where(a => a.ApplicationUserId == user.Id);
            ViewData["AuthorId"] = new SelectList(authors, "Id", "Name");
            ViewData["Genres"] = new SelectList(_context.Genre, "Id", "Description");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,AuthorId,YearPublished,Rating,GenreIds")] BookViewModel bookViewModel)
        {

            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();

                // Add the book to the database
                var bookDataModel = new Book
                {
                    Title = bookViewModel.Title,
                    AuthorId = bookViewModel.AuthorId,
                    YearPublished = bookViewModel.YearPublished,
                    Rating = bookViewModel.Rating,
                    ApplicationUserId = user.Id
                    
                };
                _context.Add(bookDataModel);
                await _context.SaveChangesAsync();

                // After saving, the Book data model now has an Id. Add genres now
                bookDataModel.BookGenres = bookViewModel.GenreIds.Select(genreId => new BookGenre
                {
                    BookId = bookDataModel.Id,
                    GenreId = genreId
                }).ToList();

                // Save again to database
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name", bookViewModel.AuthorId);
            ViewData["Genres"] = new SelectList(_context.Genre, "Id", "Description", bookViewModel.GenreIds);
            return View(bookViewModel);
        }

        // GET: Comments/Create
        [HttpGet("books/CreateComment/{bookId}")]
        public async Task<IActionResult> CreateComment()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("books/CreateComment/{bookId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([FromRoute]int bookId, [Bind("Id,Text,ApplicationUserId,BookId,Date")] Comment comment)
        {
            var user = await GetCurrentUserAsync();
            comment.ApplicationUserId = user.Id;
            comment.BookId = bookId;
            comment.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Books", new { id = bookId });
            }
            return View(comment);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var book = await _context.Book
                .Include(b => b.BookGenres)
                .Where(b => b.ApplicationUserId == user.Id)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }
           
            // convert data model to view model
            var bookViewModel = new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                Rating = book.Rating,
                YearPublished = book.YearPublished,
                GenreIds = book.BookGenres.Select(bg => bg.GenreId).ToList(),
                ApplicationUserId = user.Id
            };

            ViewData["Genres"] = new SelectList(_context.Genre, "Id", "Description", bookViewModel.GenreIds);
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name", bookViewModel.AuthorId);
            return View(bookViewModel);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,AuthorId,YearPublished,Rating,ApplicationUserId,GenreIds")] BookViewModel bookViewModel)
        {
            if (id != bookViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                // Get existing data
                var bookDataModel = await _context.Book
                    .Include(b => b.BookGenres)
                    .FirstOrDefaultAsync(b => b.Id == id);

                // Update data
                bookDataModel.Id = bookViewModel.Id;
                bookDataModel.Title = bookViewModel.Title;
                bookDataModel.Rating = bookViewModel.Rating;
                bookDataModel.YearPublished = bookViewModel.YearPublished;
                bookDataModel.AuthorId = bookViewModel.AuthorId;
                bookDataModel.ApplicationUserId = user.Id;
                bookDataModel.BookGenres = bookViewModel.GenreIds.Select(gid => new BookGenre
                {
                    BookId = bookViewModel.Id,
                    GenreId = gid
                }).ToList();

                try
                {
                    // Save changes
                    _context.Update(bookDataModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(bookViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name", bookViewModel.AuthorId);
            ViewData["Genres"] = new SelectList(_context.Genre, "Id", "Description", bookViewModel.GenreIds);
            return View(bookViewModel);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                 .Include(b => b.ApplicationUser)
                 .Include(b => b.Author)
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
