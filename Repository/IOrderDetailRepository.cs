using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public interface IOrderDetailRepository
    {
        public void Add(OrderDetail orderDetail);
        public void Update(OrderDetail orderDetail);
        public void Delete(int id);
        public IEnumerable<OrderDetail> GetAll(Expression<Func<OrderDetail, bool>>? filter = null, string? includeProperties = null);
        OrderDetail Get(Expression<Func<OrderDetail, bool>> filter, string? includeProperties = null, bool tracked = false);
        public OrderDetail GetById(int id);
        public void Save();
    }
}
