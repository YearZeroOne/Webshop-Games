using WebShopLib.Models;

namespace AP14_AKT___Webshop.Models
{
    public class OrderViewModel
    {
        public Order? Order { get; set; }
        public Store? Store { get; set; }
        public Address? OrderAddress { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
