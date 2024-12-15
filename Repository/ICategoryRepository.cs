using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public interface ICategoryRepository
    {
        public void Add(Category category);
        public void Update(Category category);
        public void Delete(int id);
        public IEnumerable<Category> GetAll(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null);
        Category Get(Expression<Func<Category, bool>> filter, string? includeProperties = null, bool tracked = false);
        public Category GetById(int id);
        public void Save();
    }
}
