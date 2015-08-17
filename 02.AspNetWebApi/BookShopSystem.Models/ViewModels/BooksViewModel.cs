using System;
using System.Collections.Generic;

namespace BookShopSystem.Models.ViewModels
{
    public class BooksViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookDescription { get; set; }
        public string BookEdition { get; set; }
        public decimal BookPrice { get; set; }
        public int BookCopies { get; set; }
        public DateTime? BookRelease { get; set; }
        public string BookAgeRestriction { get; set; }
        public string BookAuthorFirstName { get; set; }
        public string BookAuthorLastName { get; set; }
        public IEnumerable<string> BookGenres { get; set; }
    }
}