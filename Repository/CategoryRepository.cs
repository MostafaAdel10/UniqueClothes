using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        UniqueDbContext _context;
        public CategoryRepository(UniqueDbContext context)
        {
            _context = context;
        }
        public void Add(Category category)
        {
            _context.Add(category);
        }

        public void Delete(int id)
        {
            Category category = GetById(id);
            _context.Remove(category);
        }
        public Category Get(Expression<Func<Category, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<Category> query;
            if (tracked)
            {
                query = _context.Categories;

            }
            else
            {
                query = _context.Categories.AsNoTracking();
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
        public IEnumerable<Category> GetAll(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<Category> query = _context.Categories;
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

        public Category GetById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryID == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            _context.Update(category);
        }

    }
}
