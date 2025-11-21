using Microsoft.EntityFrameworkCore;
using PetShop.Context;
using PetShop.Repositories.Interfaces;
using System.Linq.Expressions;

namespace PetShop.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected PetShopContext PetShopContext { get; set; }
        public RepositoryBase(PetShopContext petShopContext)
        {
            PetShopContext = petShopContext;
        }
        public IQueryable<T> FindAll() =>
            PetShopContext.Set<T>().AsNoTracking();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
            PetShopContext.Set<T>().Where(expression).AsNoTracking();

        public void Create(T entity) =>
            PetShopContext.Set<T>().Add(entity);

        public void Update(T entity) =>
            PetShopContext.Set<T>().Update(entity);

        public void Delete(T entity) =>
            PetShopContext.Set<T>().Remove(entity);

    }
}
