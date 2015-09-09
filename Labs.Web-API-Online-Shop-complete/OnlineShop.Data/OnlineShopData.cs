namespace OnlineShop.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using Interfaces;
    using Models;
    using Repositories;

    public class OnlineShopData : IOnlineShopData
    {
        private DbContext context;
        private IDictionary<Type, object> repositories;

        public OnlineShopData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }
        public IRepository<Models.Ad> Ads
        {
            get { return this.GetRepository<Ad>(); }
        }

        public IRepository<Models.AdType> AdTypes
        {
            get { return this.GetRepository<AdType>(); }
        }

        public IRepository<Models.Category> Categories
        {
            get { return this.GetRepository<Category>(); }
        }

        public IRepository<Models.ApplicationUser> Users
        {
            get { return this.GetRepository<ApplicationUser>(); }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public int SaveChangesAsync() 
        {
            return this.context.SaveChanges(); 
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T); if (!this.repositories.ContainsKey(type))
            {
                //create repository and addit to the dictionary
                var typeOfRepository = typeof(GenericRepository<T>); 
                var repository = Activator.CreateInstance(typeOfRepository, this.context);
                this.repositories.Add(type, repository);
            }
            return (IRepository<T>)this.repositories[type];
        }
    }
}