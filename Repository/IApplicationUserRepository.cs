using System.Linq.Expressions;
using Unique.Models;

namespace Unique.Repository
{
    public interface IApplicationUserRepository
    {
        public void Add(ApplicationUser user);
        public void Update(ApplicationUser user);
        public void Delete(string id);
        public IEnumerable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>>? filter = null, string? includeProperties = null);
        ApplicationUser Get(Expression<Func<ApplicationUser, bool>> filter, string? includeProperties = null, bool tracked = false);
        public ApplicationUser GetById(string id);
        public void Save();

    }
}
