using System.Collections.Generic;

namespace BookShopSystem.Models.ViewModels
{
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }
        public string AuthorFirstname { get; set; }
        public string AuthorLastName { get; set; }
        public IEnumerable<string> BooksByAuthor { get; set; }
    }
}