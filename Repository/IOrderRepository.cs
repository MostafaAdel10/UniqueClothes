using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public interface IOrderRepository
    {
        public void Add(Order order);
        public void Update(Order order);
        public void Delete(int id);
        public IEnumerable<Order> GetAll(Expression<Func<Order, bool>>? filter = null, string? includeProperties = null);
        Order Get(Expression<Func<Order, bool>> filter, string? includeProperties = null, bool tracked = false);
        public Order GetById(int id);
        public void Save();

        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
    }
}
