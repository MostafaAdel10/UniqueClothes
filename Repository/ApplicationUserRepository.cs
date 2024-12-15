using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        UniqueDbContext _context;
        public ApplicationUserRepository(UniqueDbContext context)
        {
            _context = context;
        }
        public void Add(ApplicationUser user)
        {
            _context.Add(user);
        }

        public void Delete(string id)
        {
            ApplicationUser user = GetById(id);
            _context.Remove(user);
        }

        public ApplicationUser Get(Expression<Func<ApplicationUser, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<ApplicationUser> query;
            if (tracked)
            {
                query = _context.Users;

            }
            else
            {
                query = _context.Users.AsNoTracking();
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

        public IEnumerable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<ApplicationUser> query = _context.Users;
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

        public ApplicationUser GetById(string id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(ApplicationUser user)
        {
            _context.Update(user);
        }
    }
}
