using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Models.ViewModels
{
    public class BookCommentsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        [Display(Name = "Year Published")]
        public int YearPublished { get; set; }
        public int Rating { get; set; }

        [Display(Name = "Genres")]
        public List<int> GenreIds { get; set; }

        [Display(Name = "Genres")]
        public List<BookGenre> BookGenres { get; set; }
        
    }
}
