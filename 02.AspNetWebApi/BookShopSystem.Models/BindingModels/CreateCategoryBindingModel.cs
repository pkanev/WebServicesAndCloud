using System.ComponentModel.DataAnnotations;

namespace BookShopSystem.Models.BindingModels
{
    public class CreateCategoryBindingModel
    {
        [Required]
        public string CategoryName { get; set; }
    }
}