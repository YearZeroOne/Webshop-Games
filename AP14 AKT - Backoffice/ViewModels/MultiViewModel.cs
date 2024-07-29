using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopLib.Models;

namespace AP14_AKT___Backoffice.ViewModels
{
    public class MultiViewModel
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public List<OrderProduct> OrderProduct { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<(Product, ProductCategory)> Product { get; set; }
    }
}


