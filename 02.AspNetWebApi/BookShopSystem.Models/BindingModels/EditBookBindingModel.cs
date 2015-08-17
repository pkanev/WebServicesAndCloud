using System;
using System.ComponentModel.DataAnnotations;

namespace BookShopSystem.Models.BindingModels
{
    public class EditBookBindingModel
    {
        [Required]
        public string BookTitle { get; set; }

        public string BookDescription { get; set; }

        [Required]
        public decimal BookPrice { get; set; }
        
        [Required]
        public int BookCopies { get; set; }
        
        [Required]
        public string BookEdition { get; set; }
        
        [Required]
        public string BookAgeRestriction { get; set; }
        
        public DateTime? BookReleaseDate { get; set; }
        
        [Required]
        public int AuthorId { get; set; }
    }
}