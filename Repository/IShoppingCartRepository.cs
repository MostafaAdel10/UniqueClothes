using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public interface IShoppingCartRepository
    {
        public void Add(ShoppingCart shoppingCart);
        public void Update(ShoppingCart shoppingCart);
        public void Delete(int id);
        public IEnumerable<ShoppingCart> GetAll(Expression<Func<ShoppingCart, bool>>? filter = null, string? includeProperties = null);
        ShoppingCart Get(Expression<Func<ShoppingCart, bool>> filter, string? includeProperties = null, bool tracked = false);
        public ShoppingCart GetById(int id);
        public void Save();
    }
}
