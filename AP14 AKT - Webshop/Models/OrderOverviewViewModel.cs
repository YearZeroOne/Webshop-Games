using WebShopLib.Models;

namespace AP14_AKT___Webshop.Models
{
    public class OrderOverviewViewModel : OrderViewModel
    {
        public ICollection<Address>? Addresses { get; set; }
        public decimal PhisicalProductsAmount { get; set; }

    }
}
