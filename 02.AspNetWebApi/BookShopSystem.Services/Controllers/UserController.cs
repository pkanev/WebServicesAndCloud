using System.Linq;
using System.Web.Http;
using BookShopSystem.Data;
using BookShopSystem.Models.ViewModels;

namespace BookShopSystem.Services.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        [HttpGet]
        [Route("{username}/purchases")]
        public IHttpActionResult GetPurchassesForUser(string username)
        {
            var purchasses = context.Purchasses.Where(p => p.User.UserName == username)
                .Select(p => new PurchaseViewModel()
                {
                    BookTitle = p.Book.Title,
                    BookPrice = p.Price,
                    DateOfPurchase = p.DateOfPurchase,
                    IsRecalled = p.IsRecalled
                });

            if (purchasses.FirstOrDefault() == null)
            {
                return this.BadRequest("There were no purchases made by this user");
            }

            return this.Ok(purchasses);
        }

    }
}
