using System.ComponentModel.DataAnnotations;

namespace BookShopSystem.Models.BindingModels
{
    public class CreateAuthorBindingModel
    {
        public string AuthorFirstName { get; set; }

        [Required]
        public string AuthorLastname { get; set; }
    }
}