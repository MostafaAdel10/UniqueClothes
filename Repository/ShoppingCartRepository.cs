using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        UniqueDbContext _context;
        public ShoppingCartRepository(UniqueDbContext context)
        {
            _context = context;
        }
        public void Add(ShoppingCart shoppingCart)
        {
            _context.Add(shoppingCart);
        }

        public void Delete(int id)
        {
            ShoppingCart shoppingCart = GetById(id);
            _context.Remove(shoppingCart);
        }

        public IEnumerable<ShoppingCart> GetAll(Expression<Func<ShoppingCart, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<ShoppingCart> query = _context.ShoppingCarts;
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
        public ShoppingCart Get(Expression<Func<ShoppingCart, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<ShoppingCart> query;
            if (tracked)
            {
                query = _context.ShoppingCarts;

            }
            else
            {
                query = _context.ShoppingCarts.AsNoTracking();
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
        public ShoppingCart GetById(int id)
        {
            return _context.ShoppingCarts.FirstOrDefault(s => s.CartID == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _context.Update(shoppingCart);
        }
    }
}
