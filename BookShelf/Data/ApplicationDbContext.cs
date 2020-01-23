using System;
using System.Collections.Generic;
using System.Text;
using BookShelf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<BookGenre> BookGenre { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Create a new user for Identity Framework
            ApplicationUser user = new ApplicationUser
            {
                FirstName = "admin",
                LastName = "admin",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = "7f434309-a4d9-48e9-9ebb-8803db794577",
                Id = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);

            //Create some Authors 
            Author author1 = new Author
            {
                Id = 7,
                Name = "Gianni Rodari",
                ApplicationUserId = user.Id

            };
            modelBuilder.Entity<Author>().HasData(author1);
            Author author2 = new Author
            {
                Id = 8,
                Name = "Jimmy John",
                ApplicationUserId = user.Id

            };
            modelBuilder.Entity<Author>().HasData(author2);
            Author author3 = new Author
            {
                Id = 9,
                Name = "Jersey Mike",
                ApplicationUserId = user.Id

            };
            modelBuilder.Entity<Author>().HasData(author3);

            //Create some Books
            Book book1 = new Book
            {
                Title = "Favole a telefono",
                Id = 10,
                Genre = "Kids Novel",
                YearPublished = 1990,
                AuthorId = author1.Id,
                ApplicationUserId = user.Id,
                Rating = 9
            };
            modelBuilder.Entity<Book>().HasData(book1);
            Book book2 = new Book
            {
                Title = "Free Smells",
                Id = 11,
                Genre = "Sandwiches",
                YearPublished = 1990,
                AuthorId = author2.Id,
                ApplicationUserId = user.Id,
                Rating = 8
            };
            modelBuilder.Entity<Book>().HasData(book2);
            Book book3 = new Book
            {
                Title = "Jersey Subs",
                Id = 12,
                Genre = "Sandwiches",
                YearPublished = 1996,
                AuthorId = author3.Id,
                ApplicationUserId = user.Id,
                Rating = 7
            };
            modelBuilder.Entity<Book>().HasData(book3);

            //Create some Comments
            Comment comment1 = new Comment
            {
                Id = 1,
                Text = "It smells like jersey",
                BookId = book3.Id,
                ApplicationUserId = user.Id
            };
            modelBuilder.Entity<Comment>().HasData(comment1);
            Comment comment2 = new Comment
            {
                Id = 2,
                Text = "A beautiful book of fairy tales for kids",
                BookId = book1.Id,
                ApplicationUserId = user.Id
            };
            modelBuilder.Entity<Comment>().HasData(comment2);
            Comment comment3 = new Comment
            {
                Id = 3,
                Text = "What is even jimmy john's",
                BookId = book2.Id,
                ApplicationUserId = user.Id
            };
            modelBuilder.Entity<Comment>().HasData(comment3);

            modelBuilder.Entity<BookGenre>()
               .HasOne(bc => bc.Book)
               .WithMany(b => b.BookGenres)
               .HasForeignKey(bc => bc.BookId);
            modelBuilder.Entity<BookGenre>()
                .HasOne(bc => bc.Genre)
                .WithMany(c => c.BookGenres)
                .HasForeignKey(bc => bc.GenreId);

            modelBuilder.Entity<Genre>().HasData(
                new Genre
                {
                    Id = 1,
                    Description = "Fantasy"
                },
                new Genre
                {
                    Id = 2,
                    Description = "Science Fiction"
                },
                new Genre
                {
                    Id = 3,
                    Description = "Horror"
                },
                new Genre
                {
                    Id = 4,
                    Description = "Western"
                },
                new Genre
                {
                    Id = 5,
                    Description = "Romance"
                },
                new Genre
                {
                    Id = 6,
                    Description = "Thriller"
                },
                new Genre
                {
                    Id = 7,
                    Description = "Mystery"
                },
                new Genre
                {
                    Id = 8,
                    Description = "Detective"
                },
                new Genre
                {
                    Id = 9,
                    Description = "Distopia"
                }
                );

        }

        
    }
}
