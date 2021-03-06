using System.Runtime.CompilerServices;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineShop.Data.Migrations;
using OnlineShop.Models;

namespace OnlineShop.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class OnlineShopContext : IdentityDbContext<ApplicationUser>
    {
        
        public OnlineShopContext()
            : base("name=OnlineShopContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<OnlineShopContext, Configuration>());
        }

        public IDbSet<Ad> Ads { get; set; }
        public IDbSet<AdType> AdTypes { get; set; }
        public IDbSet<Category> Categories { get; set; }

        public static OnlineShopContext Create()
        {
            return new OnlineShopContext();
        }
    }
}