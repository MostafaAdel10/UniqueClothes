using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        UniqueDbContext _context;
        public CartItemRepository(UniqueDbContext context)
        {
            _context = context;
        }
        public void Add(CartItem cartItem)
        {
            _context.Add(cartItem);
        }

        public void Delete(int id)
        {
            CartItem cartItem = GetById(id);
            _context.Remove(cartItem);
        }

        public CartItem Get(Expression<Func<CartItem, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<CartItem> query;
            if (tracked)
            {
                query = _context.CartItems;

            }
            else
            {
                query = _context.CartItems.AsNoTracking();
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

        public IEnumerable<CartItem> GetAll(Expression<Func<CartItem, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<CartItem> query = _context.CartItems;
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

        public CartItem GetById(int id)
        {
            return _context.CartItems.FirstOrDefault(c => c.CartItemID == id);
        }

        public void RemoveRange(IEnumerable<CartItem> cartItem)
        {
            _context.RemoveRange(cartItem);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(CartItem cartItem)
        {
            _context.Update(cartItem);
        }
    }
}
