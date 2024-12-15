using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        UniqueDbContext _context;
        public OrderDetailRepository(UniqueDbContext context)
        {
            _context = context;
        }
        public void Add(OrderDetail orderDetail)
        {
            _context.Add(orderDetail);
        }

        public void Delete(int id)
        {
            OrderDetail orderDetail = GetById(id);
            _context.Remove(orderDetail);
        }

        public OrderDetail Get(Expression<Func<OrderDetail, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<OrderDetail> query;
            if (tracked)
            {
                query = _context.OrderDetails;

            }
            else
            {
                query = _context.OrderDetails.AsNoTracking();
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

        public IEnumerable<OrderDetail> GetAll(Expression<Func<OrderDetail, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<OrderDetail> query = _context.OrderDetails;
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

        public OrderDetail GetById(int id)
        {
            return _context.OrderDetails.FirstOrDefault(o => o.OrderDetailID == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(OrderDetail orderDetail)
        {
            _context.Update(orderDetail);
        }
    }
}
