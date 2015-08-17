using System.Linq;
using System.Web.Http;
using System.Web.OData;
using BookShopSystem.Data;
using BookShopSystem.Models.BindingModels;
using BookShopSystem.Models.DbModels;
using BookShopSystem.Models.ViewModels;

namespace BookShopSystem.Services.Controllers
{
    [RoutePrefix("api/categories")]
    public class CategoriesController : ApiController
    {
        private BookShopContext context = new BookShopContext();

        // api/categories
        [HttpGet]
        [EnableQuery]
        public IHttpActionResult GetCategories()
        {
            var result = context.Categories.Select(
                c=> new CategoryViewModel()
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name
                });

            return this.Ok(result);
        }

        // api/categories/{id}
        [HttpGet]
        public IHttpActionResult GetCategory(int id)
        {
            var category = context.Categories.Where(c => c.Id == id).Select(c => new {c.Id, c.Name}).FirstOrDefault();
            if (category != null)
            {
                return this.Ok(category);    
            }
            return this.BadRequest("Sorry but thre is no category with such id!");
        }

        // /api/categories/{id}
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditCategory([FromUri]int id, [FromBody]EditCategoryBindingModel editCategory)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var duplicate = context.Categories.FirstOrDefault(c => c.Name == editCategory.CategoryName);
            if (duplicate != null)
            {
                return this.BadRequest("The category already exists");
            }

            var cat = context.Categories.FirstOrDefault(c => c.Id == id);
            if (cat == null)
            {
                return this.BadRequest("There is no category with such id!");
            }

            cat.Name = editCategory.CategoryName;
            context.SaveChanges();

            return this.Ok(editCategory);
        }

        // /api/categories/{id}
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteCategory([FromUri]int id)
        {
            var cat = context.Categories.FirstOrDefault(c => c.Id == id);
            if (cat == null)
            {
                return this.BadRequest("There is no category with such id!");
            }
            foreach (var book in context.Books)
            {
                var category = book.Categories.FirstOrDefault(c => c.Name == cat.Name);
                if (category != null)
                {
                    book.Categories.Remove(category);
                }
            }
            context.Categories.Remove(cat);
            context.SaveChanges();

            return this.Ok("The category " + cat.Name + " was deleted successfully!");
        }

        // api/categories/
        [HttpPost]
        public IHttpActionResult AddCategory([FromBody]CreateCategoryBindingModel catModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var duplicate = context.Categories.FirstOrDefault(c => c.Name == catModel.CategoryName);
            if (duplicate != null)
            {
                return this.BadRequest("The category already exists");
            }

            context.Categories.Add(new Category(){ Name = catModel.CategoryName });
            context.SaveChanges();
            return this.Ok(catModel);
        }
    }
}
