using System;

namespace BookShopSystem.Models.ViewModels
{
    public class PurchaseViewModel
    {
        public string BookTitle { get; set; }
        public decimal BookPrice { get; set; }
        public DateTime DateOfPurchase { get; set; }
        public bool IsRecalled { get; set; }
    }
}