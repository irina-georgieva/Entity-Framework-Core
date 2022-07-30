using System;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using BookShop.Models;
using BookShop.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            //string command = Console.ReadLine();
            //int year = int.Parse(Console.ReadLine());
            //string input = Console.ReadLine();
            //string categoryInput = Console.ReadLine();
            //string date = Console.ReadLine();
            //string inputTitle = Console.ReadLine();
            //string inputAuthor = Console.ReadLine();
            int inputLength = int.Parse(Console.ReadLine());

            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //Console.WriteLine(GetBooksByAgeRestriction(db, command));
            //Console.WriteLine(GetGoldenBooks(db));
            //Console.WriteLine(GetBooksByPrice(db));
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));
            //Console.WriteLine(GetAuthorNamesEndingIn(db, input));
            //Console.WriteLine(GetBooksByCategory(db, categoryInput));
            //Console.WriteLine(GetBooksReleasedBefore(db, date));
            //Console.WriteLine(GetBookTitlesContaining(db, inputTitle));
            //Console.WriteLine(CountCopiesByAuthor(db));
            //Console.WriteLine(GetTotalProfitByCategory(db));
            //Console.WriteLine(GetMostRecentBooks(db));
            //Console.WriteLine(GetBooksByAuthor(db, inputAuthor));
            Console.WriteLine(CountBooks(db, inputLength));
        }

        //02 Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestriction;
            bool hasParsed =
                Enum.TryParse<AgeRestriction>(command, true, out ageRestriction);

            if (!hasParsed)
            {
                return String.Empty;
            }

            string[] bookTitles = context
                .Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //03 Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] bookTitles = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold &&
                            b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //04 Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price

                })
                .OrderByDescending(b => b.Price)
                .ToArray();

            foreach (var book in books)
            {
                output
                    .AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return output.ToString().TrimEnd();
        }

        //05 Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            string[] bookTitles = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //06 Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.Trim().ToLower();

            var booksWithTitles = context
                .Books
                .Where(b => b.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            return String.Join(Environment.NewLine, booksWithTitles);
        }

        //07 Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder output = new StringBuilder();

            var releasedBooks = context
                .Books
                .Where(b => b.ReleaseDate.Value < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToArray();

            foreach (var book in releasedBooks)
            {
                output
                    .AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return output.ToString().TrimEnd();
        }

        //08 Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            string[] authorNames = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => $"{a.FirstName} {a.LastName}")
                .ToArray()
                .OrderBy(n => n)
                .ToArray();

            return String.Join(Environment.NewLine, authorNames);
        }

        //09 Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string inputLowerCase = input.ToLower();

            var foundBooks = context
                .Books
                .Where(b => b.Title.ToLower().Contains(inputLowerCase))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            return String.Join(Environment.NewLine, foundBooks);
        }

        //10 Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            //string inputLower = input.ToLower();

            //StringBuilder output = new StringBuilder();

            //var books = context
            //    .Books
            //    .Where(b => b.Author.LastName.StartsWith(inputLower))
            //    .OrderBy(b => b.BookId)
            //    .Select(b => new
            //    {
            //        b.Title,
            //        AuthorName = $"{b.Author.FirstName} {b.Author.LastName}",
            //    })
            //    .ToArray();

            //foreach (var book in books)
            //{
            //    output
            //        .AppendLine($"{book.Title} ({book.AuthorName})");
            //}

            //return output.ToString().TrimEnd();

            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Include(x => x.Author)
                .ToArray()
                .Where(x => x.Author.LastName.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.BookId)
                .Select(x => new
                {
                    Title = x.Title,
                    AuthorName = $"{x.Author.FirstName} {x.Author.LastName}",
                });

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorName})");
            }

            return sb.ToString().TrimEnd();
        }

        //11 Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booktsTitles = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .Select(b => b.Title)
                .ToArray();

            return booktsTitles.Count();
        }

        //12 Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var authorsWithCopies = context
                .Authors
                .Select(a => new
                {
                    AuthorName = $"{a.FirstName} {a.LastName}",
                    Copies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.Copies)
                .ToArray();

            foreach (var a in authorsWithCopies)
            {
                output
                    .AppendLine($"{a.AuthorName} - {a.Copies}");
            }

            return output.ToString().TrimEnd();
        }

        //13 Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var categoriesProfits = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    Profit = c.CategoryBooks
                        .Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.Name)
                .ToArray();

            foreach (var cp in categoriesProfits)
            {
                output
                    .AppendLine($"{cp.Name} ${cp.Profit:f2}");
            }

            return output.ToString().TrimEnd();
        }

        //14 Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var categoriesWithRecentBooks = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    MostRecentBooks = c.CategoryBooks
                        .OrderByDescending(cb => cb.Book.ReleaseDate.Value)
                        .Select(cb => new
                        {
                            BookTitle = cb.Book.Title,
                            BookReleaseYear = cb.Book.ReleaseDate.Value.Year
                        })
                        .Take(3)
                        .ToArray()
                })
                .OrderBy(c => c.Name)
                .ToArray();

            foreach (var c in categoriesWithRecentBooks)
            {
                output
                    .AppendLine($"--{c.Name}");

                foreach (var book in c.MostRecentBooks)
                {
                    output
                        .AppendLine($"{book.BookTitle} ({book.BookReleaseYear})");
                }
            }

            return output.ToString().TrimEnd();
        }

        //15 Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            //IQueryable<Book> booksBeforeYear = context
            //    .Books
            //    .Where(b => b.ReleaseDate.Value.Year < 2010);

            //foreach (var book in booksBeforeYear)
            //{
            //    book.Price += 5;
            //}

            //context.SaveChanges();

            context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .Update(b => new Book() { Price = b.Price + 5 });
        }

        //16 Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var removedBooks = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.Books.RemoveRange(removedBooks);
            context.SaveChanges();

            return removedBooks.Count();
        }
    }
}
