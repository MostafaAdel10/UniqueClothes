using Unique.Models;

namespace Unique.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<CartItem> ShoppingCartList { get; set; }
        public Order OrderHeader { get; set; }
    }
}
