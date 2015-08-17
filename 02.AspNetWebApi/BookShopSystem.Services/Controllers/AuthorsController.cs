using System.Linq;
using System.Web.Http;
using System.Web.OData;
using BookShopSystem.Data;
using BookShopSystem.Models.BindingModels;
using BookShopSystem.Models.DbModels;
using BookShopSystem.Models.ViewModels;

namespace BookShopSystem.Services.Controllers
{
    [RoutePrefix("api/authors")]
    public class AuthorsController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        // api/authors/5
        [HttpGet]
        [Route("{id}")]
        [EnableQuery]
        public IHttpActionResult GetAuthor(int id)
        {
            var result = context.Authors.Where(a=>a.Id == id).Select(
                a => new AuthorViewModel()
                {
                    AuthorId = a.Id,
                    AuthorFirstname = a.FirstName,
                    AuthorLastName = a.LastName,
                    BooksByAuthor = a.Books.Select(b => b.Title)
                });

            if (result.FirstOrDefault() == null)
            {
                return this.NotFound();
            }
            return this.Ok(result);
        }

        // api/authors/5/books
        [HttpGet]
        [Route("{id}/books")]
        [EnableQuery]
        public IHttpActionResult GetBooksByAuthor(int id)
        {
            var result = context.Books.Where(b => b.Author.Id == id).Select(
                b => new BooksViewModel()
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
                });

            if (result.FirstOrDefault() == null)
            {
                return this.NotFound();
            }
            return this.Ok(result);
        }

        // api/authors
        [HttpPost]
        public IHttpActionResult CreateAuthor([FromBody] CreateAuthorBindingModel authorModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            context.Authors.Add(new Author()
            {
                FirstName = authorModel.AuthorFirstName,
                LastName = authorModel.AuthorLastname
            });

            context.SaveChanges();
            
            return this.Ok(authorModel);
        }
    }
}
