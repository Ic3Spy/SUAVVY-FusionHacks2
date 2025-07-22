using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Models
{
    public class CartViewModel : BaseViewModel
    {
        public bool SelectMode { get; set; } = false;
        public Cart SelectedCart { get; set; }
        public List<Cart> Carts { get; set; }
        public List<Cart> UserCarts { get; set; }
        public List<Product> CartItems { get; set; }
        public Product CartItem { get; set; }
        public CartViewModel()
        {
            Carts = new List<Cart>();
            CartItems = new List<Product>();
            CartItem = new Product();
            SelectedCart = new Cart();
            SelectMode = false;
        }
        public void ClearCart()
        {
            SelectedCart = new Cart();
            Carts.Clear();
            Status = "success";
            StatusMessage = "Cart has been cleared successfully!";
        }
    }
}
