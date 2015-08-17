using System;
using System.ComponentModel.DataAnnotations;

namespace BookShopSystem.Models.DbModels
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public Book Book { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime DateOfPurchase { get; set; }

        [Required]
        public bool IsRecalled { get; set; }
    }
}