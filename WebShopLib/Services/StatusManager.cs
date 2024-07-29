using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopLib.Models;

namespace WebShopLib.Services
{
    public static class StatusManager
    {
        public static ReOrderStatus reOrderStatus(ApplicationDbContext _context, ReOrder reOrder)
        {

            var deliveries = _context.Deliveries.Where(x => x.ReOrderId == reOrder.Id).ToList();

            if (reOrder.Sent && deliveries.All(x=>x.Completed))
            {
                return ReOrderStatus.Complete;
            }

            if (reOrder.Sent && deliveries.Any(x=>x.Completed))
            {
                return ReOrderStatus.PartiallyFulfilled;
            }

            if (reOrder.Sent)
            {
                return ReOrderStatus.Sent;
            }

            if (reOrder.Deliveries.Count() == 0)
            {
                return ReOrderStatus.Created;
            }

            return ReOrderStatus.Created;
        }
    }
}
