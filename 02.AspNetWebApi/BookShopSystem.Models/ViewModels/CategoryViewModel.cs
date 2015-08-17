using BookShopSystem.Models.DbModels;

namespace BookShopSystem.Models.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public CategoryViewModel(Category category) : this ()
        {
            this.CategoryId = category.Id;
            this.CategoryName = CategoryName;
        }

        public CategoryViewModel()
        {
            
        }
    }
}