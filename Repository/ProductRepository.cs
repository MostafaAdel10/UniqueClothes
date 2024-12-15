using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public class ProductRepository : IProductRepository
    {
        UniqueDbContext _context;
        public ProductRepository(UniqueDbContext context)
        {
            _context = context;
        }
        public void Add(Product product)
        {
            _context.Add(product);
        }

        public void Delete(int id)
        {
            Product product = GetById(id);
            _context.Remove(product);
        }

        public Product Get(Expression<Func<Product, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<Product> query;
            if (tracked)
            {
                query = _context.Products;

            }
            else
            {
                query = _context.Products.AsNoTracking();
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

        public IEnumerable<Product> GetAll(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<Product> query = _context.Products;
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

        public Product GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.ProductID == id);
        }

        public IQueryable<Product> GetProductsColorQueryable(string? color)
        {
            var products = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(color))
            {
                products = products.Where(p => p.Color.Contains(color));
            }
            return products;
        }

        public IQueryable<Product> GetProductsQueryable(string? search)
        {
            var products = _context.Products.AsQueryable();
            if(!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }
            return products;
        }

        public IQueryable<Product> GetProductsQueryable(int? to)
        {
            var products = _context.Products.AsQueryable();
                products = products.Where(p => p.Price <= to);
            return products;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            _context.Update(product);
        }
    }
}
