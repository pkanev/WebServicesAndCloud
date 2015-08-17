using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using BookShopSystem.Data;
using BookShopSystem.Models.BindingModels;
using BookShopSystem.Models.DbModels;
using BookShopSystem.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace BookShopSystem.Services.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        // api/books/{id}
        [HttpGet]
        [Route("{id}")]
        [EnableQuery]
        public IHttpActionResult GetBook(int id)
        {
            var result = context.Books.Where(b => b.Id == id)
                .Select(b => new BooksViewModel()
                {
                    BookId = b.Id,
                    BookTitle = b.Title,
                    BookAuthorFirstName = b.Author.FirstName,
                    BookAuthorLastName = b.Author.LastName,
                    BookAgeRestriction = b.AgeRestriction.ToString(),
                    BookDescription = b.Description,
                    BookCopies = b.Copies,
                    BookEdition = b.Edition.ToString(),
                    BookPrice = b.Price,
                    BookRelease = b.ReleaseDate,
                    BookGenres = b.Categories.Select(c => c.Name)
                }).FirstOrDefault();

            if (result == null)
            {
                return this.BadRequest("There is no book with such id");
            }

            return this.Ok(result);
        }

        // api/books?search={word}
        [HttpGet]
        public IHttpActionResult GetBookConatiningWord([FromUri] string search)
        {
            var result = context.Books.Where(b => b.Title.Contains(search))
                .OrderBy(b=>b.Title).Take(10)
                .Select(b => new BooksBriefViewModel()
                {
                    Id = b.Id,
                    Title = b.Title
                });

            if (result.FirstOrDefault() == null)
            {
                return this.NotFound();
            }

            return this.Ok(result);
        }

        // api/books/{id}
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditBook([FromUri]int id, [FromBody]EditBookBindingModel bookModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            
            Book book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.BadRequest("There is no book with such id!");
            }

            book.Title = bookModel.BookTitle;
            book.Description = bookModel.BookDescription;
            book.Price = bookModel.BookPrice;
            book.Copies = bookModel.BookCopies;
            book.Edition = (Edition)Enum.Parse(typeof(Edition), bookModel.BookEdition);
            book.AgeRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), bookModel.BookAgeRestriction);
            book.ReleaseDate = bookModel.BookReleaseDate;
            book.AuthorId = bookModel.AuthorId;

            context.Books.AddOrUpdate(book);
            context.SaveChanges();

            return this.Ok(bookModel);
        }

        // api/books/{id}
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteBook([FromUri]int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.BadRequest("There is no book with such id!");
            }
            foreach (var category in book.Categories)
            {
                category.Books.Remove(book);
            }
            context.Books.Remove(book);
            context.SaveChanges();

            return this.Ok("The Book " + book.Title + " was deleted successfully!");
        }

        // api/books
        [HttpPost]
        public IHttpActionResult CreateBook([FromBody] CreateBookBindingModel bookModel )
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            
            if (context.Authors.FirstOrDefault(a => a.Id == bookModel.AuthorId) == null)
            {
                return this.BadRequest("There is no author with id: " + bookModel.AuthorId);
            }

            var book = new Book()
            {
                Title = bookModel.BookTitle,
                Description = bookModel.BookDescription,
                Price = bookModel.BookPrice,
                Copies = bookModel.BookCopies,
                Edition = (Edition)Enum.Parse(typeof(Edition), bookModel.BookEdition),
                AgeRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), bookModel.AgeRestriction),
                ReleaseDate = bookModel.BookReleaseDate,
                AuthorId = bookModel.AuthorId,
            };


            string[] categories = bookModel.BookCategories.Split(' ');

            foreach (string category in categories)
            {
                var cat = context.Categories.FirstOrDefault(c => c.Name == category);
                if (cat == null)
                {
                    return this.BadRequest("The category " + category + " does not exist!");
                }
                book.Categories.Add(cat);
            }

            context.Books.Add(book);
            context.SaveChanges();

            return this.Ok(bookModel);
        }

        // api/books/buy/{id}
        [Authorize]
        [HttpPut]
        [Route("buy/{id}")]
        public IHttpActionResult BuyBook([FromUri]int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.BadRequest("A book with such id: " + id + " does not exist!");
            }
            if (book.Copies == 0)
            {
                return this.BadRequest("Currently we do not have any copies of this title. Please try later.");
            }

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = context.Users.FirstOrDefault(x => x.Id == currentUserId);

            if (currentUser == null)
            {
                return this.BadRequest("You must be logged in.");
            }

            Purchase purchase = new Purchase()
            {
                Book = book,
                DateOfPurchase = DateTime.Now,
                IsRecalled = false,
                Price = book.Price,
                User = currentUser
            };

            book.Copies --;
            context.Purchasses.Add(purchase);

            context.SaveChanges();
            string message = string.Format("User: {0} has successfully purchassed {1}",
                currentUser.UserName, book.Title);

            return this.Ok(message);
        }

        // api/books/recall/{id}
        [Authorize]
        [HttpPut]
        [Route("recall/{id}")]
        public IHttpActionResult RecallBook([FromUri] int id)
        {
            var book = context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return this.BadRequest("A book with such id: " + id + " does not exist!");
            }

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = context.Users.FirstOrDefault(x => x.Id == currentUserId);

            if (currentUser == null)
            {
                return this.BadRequest("You must be logged in.");
            }

            var purchase = context.Purchasses
                .FirstOrDefault(p => p.User.Id == currentUser.Id && p.Book.Id == book.Id);
            if (purchase == null)
            {
                return this.BadRequest("You have not bought this book!");
            }
            
            double daysAfterPurchase = (DateTime.Now - purchase.DateOfPurchase).TotalDays;
            if (daysAfterPurchase > 30)
            {
                return this.BadRequest("You are allowed to return this book. More than 30 days have passed.");
            }

            if (purchase.IsRecalled)
            {
                return this.BadRequest("This book has already been returned.");
            }

            purchase.IsRecalled = true;
            book.Copies ++;
            context.SaveChanges();

            string message = string.Format("The book {0} has been successfully returned.", book.Title);
            return this.Ok(message);
        }
    }
}
