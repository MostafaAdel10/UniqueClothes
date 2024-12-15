using Unique.Models;

namespace Unique.Repository
{
    public interface IReviewRepository
    {
        public void Add(Review review);
        public void Update(Review review);
        public void Delete(int id);
        public List<Review> GetAll();
        public Review GetById(int id);
        public void Save();
    }
}
