using WebShopLib.Models;

namespace AP14_AKT___Webshop.Models
{
    public class OrderConfirmationViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public List<Address> Addresses { get; set; }
    }

}
