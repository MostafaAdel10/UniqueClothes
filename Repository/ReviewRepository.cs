using Unique.Models;

namespace Unique.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        UniqueDbContext _context;
        public ReviewRepository(UniqueDbContext context)
        {
            _context = context;
        }
        public void Add(Review review)
        {
            _context.Add(review);
        }

        public void Delete(int id)
        {
            Review review = GetById(id);
            _context.Remove(review);
        }

        public List<Review> GetAll()
        {
            return _context.Reviews.ToList();
        }

        public Review GetById(int id)
        {
            return _context.Reviews.FirstOrDefault(r => r.ReviewID == id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Review review)
        {
            _context.Update(review);
        }
    }
}
