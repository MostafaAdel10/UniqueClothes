using Unique.Models;

namespace Unique.ViewModels
{
    public class OrderVM
    {
        public Order OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
