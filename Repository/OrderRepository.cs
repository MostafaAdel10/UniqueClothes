using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public class OrderRepository : IOrderRepository
    {
        UniqueDbContext _context;
        public OrderRepository(UniqueDbContext context)
        {
            _context = context;
        }
        public void Add(Order order)
        {
            _context.Add(order);
        }

        public void Delete(int id)
        {
            Order order = GetById(id);
            _context.Remove(order);
        }

        public Order Get(Expression<Func<Order, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<Order> query;
            if (tracked)
            {
                query = _context.Orders;

            }
            else
            {
                query = _context.Orders.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();

        }

        public IEnumerable<Order> GetAll(Expression<Func<Order, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<Order> query = _context.Orders;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public Order GetById(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.OrderID == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Order order)
        {
            _context.Update(order);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _context.Orders.FirstOrDefault(u => u.OrderID == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _context.Orders.FirstOrDefault(u => u.OrderID == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }
    }
}
