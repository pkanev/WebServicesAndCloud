using System.Data.Entity;
using BookShopSystem.Data.Migrations;
using BookShopSystem.Models;
using BookShopSystem.Models.DbModels;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookShopSystem.Data
{
    public class BookShopContext : IdentityDbContext<ApplicationUser>
    {
        public BookShopContext()
            : base("name=BookShopContext")
        {
            var migrationStrategy = new MigrateDatabaseToLatestVersion<BookShopContext, Configuration>();
            Database.SetInitializer(migrationStrategy);
        }

        public static BookShopContext Create()
        {
            return new BookShopContext();
        }

        public IDbSet<Category> Categories { get; set; }
        public IDbSet<Author> Authors { get; set; }
        public IDbSet<Book> Books { get; set; }
        public IDbSet<Purchase> Purchasses { get; set; }


    }
}