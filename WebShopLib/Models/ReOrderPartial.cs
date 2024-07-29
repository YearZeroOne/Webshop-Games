using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopLib.Services;

namespace WebShopLib.Models
{
    public partial class ReOrder
    {
        public ReOrderStatus Status
        {
            get { return StatusManager.reOrderStatus(_context, this); }
        }

        private readonly ApplicationDbContext _context;

        public ReOrder(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
