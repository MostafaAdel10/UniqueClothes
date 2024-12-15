using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public interface IProductRepository
    {
        public void Add(Product product);
        public void Update(Product product);
        public void Delete(int id);
        public IEnumerable<Product> GetAll(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null);
        Product Get(Expression<Func<Product, bool>> filter, string? includeProperties = null, bool tracked = false);
        public IQueryable<Product> GetProductsQueryable(string? search);
        public IQueryable<Product> GetProductsQueryable(int? to);
        public IQueryable<Product> GetProductsColorQueryable(string? color);
        public Product GetById(int id);
        public void Save();
    }
}
