using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public interface ICartItemRepository
    {
        public void Add(CartItem cartItem);
        public void Update(CartItem cartItem);
        public void Delete(int id);
        public IEnumerable<CartItem> GetAll(Expression<Func<CartItem, bool>>? filter = null, string? includeProperties = null);
        CartItem Get(Expression<Func<CartItem, bool>> filter, string? includeProperties = null, bool tracked = false);
        public CartItem GetById(int id);
        public void Save();

        void RemoveRange(IEnumerable<CartItem> cartItem);
    }
}
