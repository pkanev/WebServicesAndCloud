using System;
using System.ComponentModel.DataAnnotations;

namespace BookShopSystem.Models.BindingModels
{
    public class CreateBookBindingModel
    {
        [Required]
        public string BookTitle { get; set; }

        [Required]
        public string BookDescription { get; set; }

        [Required]
        public decimal BookPrice { get; set; }

        [Required]
        public int BookCopies { get; set; }

        [Required]
        public string BookEdition { get; set; }

        [Required]
        public string AgeRestriction { get; set; }

        [Required]
        public DateTime BookReleaseDate { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string BookCategories { get; set; }
    }
}