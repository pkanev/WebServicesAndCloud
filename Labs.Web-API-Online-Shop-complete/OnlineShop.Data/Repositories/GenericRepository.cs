namespace OnlineShop.Data.Repositories
{
    using System.Data.Entity;
    using System.Linq;
    using Interfaces;

    public class GenericRepository<T> : IRepository<T>
        where T : class
    {
        
        public GenericRepository(DbContext context)
        {
            this.Context = context;
            this.Set = context.Set<T>();
        }

        protected DbContext Context { get; private set; }
        protected IDbSet<T> Set { get; private set; }

        public System.Linq.IQueryable<T> All()
        {
            return this.Set.AsQueryable();
        }

        public T Find(object id)
        {
            return this.Set.Find(id);
        }

        public void Add(T entity)
        {
            this.ChangeState(entity, EntityState.Added);
        }

        public void Update(T entity)
        {
            this.ChangeState(entity, EntityState.Modified);
        }

        public void Delete(T entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public void Delete(object id)
        {
            var entity = this.Find(id);
            this.Delete(entity);
        }

        private void ChangeState(T entity, EntityState state)
        {
            var entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.Set.Attach(entity);
            }

            entry.State = state;
        }
    }
}