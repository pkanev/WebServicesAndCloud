using System.ComponentModel.DataAnnotations;

namespace BookShopSystem.Models.BindingModels
{
    public class EditCategoryBindingModel
    {
        [Required]
        public string CategoryName { get; set; }
    }
}