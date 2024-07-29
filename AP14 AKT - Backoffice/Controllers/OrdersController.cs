using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using WebShopLib.Models;
using WebShopLib.Services;

namespace AP14_AKT___Backoffice.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string _conStr;
        public List<Order> orderList;

        /// <summary>
        /// Constructor for OrdersController class. Initializes the ApplicationDbContext and IConfiguration objects, and creates a new List of Orders. 
        /// </summary>
        /// <returns>
        /// Nothing.
        /// </returns>
        public OrdersController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _conStr = config.GetConnectionString("ConnectionString");
            orderList = new List<Order>();
            UpdateOrderList();
        }

        private void UpdateOrderList()
        {
            orderList = _context.Orders.ToList();
        }

        // GET: Orders
        /// <summary>
        /// This method filters orders based on filter, orderyear, ordermonth, orderdate1, orderdate2, status, cancel and paymentMethod parameters.
        /// </summary>
        /// <returns>
        /// Returns a list of filtered orders.
        /// </returns>
        public IActionResult Index(int? filter, int? orderyear, int? ordermonth, DateTime? orderdate1, DateTime? orderdate2, bool? status, bool? cancel, int? paymentMethod)
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);
            var orders = _context.Orders.ToList();
            List<Order> filteredList = new List<Order>();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }

            #region filter
            // Filter Date
            if (filter == 1)
            {
                foreach (var order in orders)
                {
                    var year = _context.Orders.Where(x => x.Id == order.Id).FirstOrDefault().OrderDate.Year;
                    var month = _context.Orders.Where(x => x.Id == order.Id).FirstOrDefault().OrderDate.Month;
                    var date = _context.Orders.Where(x => x.Id == order.Id).FirstOrDefault().OrderDate;

                    if (year == orderyear && month == ordermonth)
                    {
                        filteredList.Add(order);
                    }
                    if (orderdate1 <= date && date <= orderdate2)
                    {
                        filteredList.Add(order);
                    }
                }
                return View(filteredList);
            }

            // Filter Status
            if (filter == 2)
            {
                foreach (var order in orders)
                {
                    var orderStatus = order.PaymentPending;
                    var cancelstat = order.Cancelled;

                    if (status == orderStatus && !cancelstat)
                    {
                        filteredList.Add(order);
                    }
                    if (cancel == cancelstat)
                    {
                        filteredList.Add(order);
                    }
                }
                return View(filteredList);
            }

            // Filter PaymentMethod
            if (filter == 3)
            {
                foreach (var order in orders)
                {
                    var paymethod = _context.PaymentMethods.Where(x => x.Id == order.PaymentMethodId);

                    if (paymethod.Where(x => x.Id == order.PaymentMethodId).FirstOrDefault().Id == paymentMethod)
                    {
                        filteredList.Add(order);
                    }
                }
                return View(filteredList);
            }

            #endregion filter

            return View(orders);
        }

        // GET: Orders/Details/5
        /// <summary>
        /// Retrieves the details of an order from the database.
        /// </summary>
        /// <param name="id">The id of the order to be retrieved.</param>
        /// <returns>The view of the order details.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        /// <summary>
        /// Creates a new view for creating a new payment.
        /// </summary>
        /// <returns>
        /// Returns a view with a list of customers and payment methods.
        /// </returns>
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CustomerNumber");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Creates a new Order object and adds it to the database.
        /// </summary>
        /// <param name="order">The Order object to be added.</param>
        /// <returns>Redirects to the Index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,OrderDate,DeliveryDate,OrderNumber,PaymentPending,PaymentMethodId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CustomerNumber", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", order.PaymentMethodId);
            return View(order);
        }

        // GET: Orders/Edit/5
        /// <summary>
        /// Retrieves an Order from the database and populates the ViewData with the associated Customer and PaymentMethod.
        /// </summary>
        /// <param name="id">The id of the Order to be retrieved.</param>
        /// <returns>The View associated with the Order.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CustomerNumber", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", order.PaymentMethodId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Updates an existing Order in the database.
        /// </summary>
        /// <param name="id">The ID of the Order to be updated.</param>
        /// <param name="order">The Order object containing the updated information.</param>
        /// <returns>The View for the updated Order.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,OrderDate,DeliveryDate,OrderNumber,PaymentPending,PaymentMethodId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CustomerNumber", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", order.PaymentMethodId);
            return View(order);
        }

        // GET: Orders/Delete/5
        /// <summary>
        /// Retrieves an order from the database and returns it to the view.
        /// </summary>
        /// <param name="id">The id of the order to be retrieved.</param>
        /// <returns>The view with the retrieved order.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        /// <summary>
        /// Deletes an order from the database.
        /// </summary>
        /// <param name="id">The id of the order to delete.</param>
        /// <returns>Redirects to the Index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if an Order exists with the given Id.
        /// </summary>
        /// <param name="id">The Id of the Order to check.</param>
        /// <returns>True if the Order exists, false otherwise.</returns>
        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Searches for an order by its order number and returns the details of the order.
        /// </summary>
        /// <param name="search">The order number to search for.</param>
        /// <returns>The details of the order.</returns>
        public IActionResult Search(string search)
        {

            
            if (string.IsNullOrEmpty(search)) return RedirectToAction(nameof(Index));

            var thisOne = _context.Orders.Include(x => x.Customer).Where(x => x.OrderNumber.Equals(search)).FirstOrDefault();

            if (thisOne == null)
            {
                return RedirectToAction(nameof(Index));
            }


            return View("Details", thisOne);
        }

        public IActionResult ShowReceipt(int id)
        {
            return View("Receipt", id);
        }

        //public IActionResult DownloadReceipt(int id)
        //{
        //    string guid = HttpContext.Request.Cookies["guid"].ToString();

        //    if (guid != null)
        //    {
        //        var user = _context.Employees.Where(x => x.Guid == guid).FirstOrDefault();

        //        if (user == null) { return RedirectToAction(nameof(Index)); }
        //    }
        //    else
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }


        //    var order = _context.Orders.Include(x => x.Customer).Include(x => x.OrderProducts).ThenInclude(x => x.Products).ThenInclude(x => x.ProductCategory).Where(x => x.Id == id).FirstOrDefault();

        //    List<(string, int, decimal)> productList = new List<(string, int, decimal)>();

        //    foreach (var item in order.OrderProducts)
        //    {
        //        productList.Add((item.Products.Name, item.Quantity, (item.Products.NetUnitPrice) * (1 + item.Products.ProductCategory.TaxRate)));
        //    }

        //    string receipt = ReceiptGenerator.GenerateReceiptHtml(order.OrderDate.ToShortTimeString(), order.Customer.FirstName + " " + order.Customer.LastName, productList);

        //    var pdf = ReceiptGenerator.GenerateReceiptPdf(receipt);


        //    return new FileStreamResult(new MemoryStream(pdf), "application/pdf")
        //    {
        //        FileDownloadName = "Rechnung_" + id.ToString()
        //    };
        //}

        /// <summary>
        /// Downloads an invoice as a PDF file for the specified order.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <param name="html">The HTML content of the invoice.</param>
        /// <returns>A PDF file containing the invoice.</returns>
        public IActionResult DownloadInvoice(int id, string html)
        {
            var order = _context.Orders.Find(id);

            var customer = _context.Customers.Find(order.CustomerId);

            MemoryStream stream = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
            pdfDoc.Open();

            // HTML-String in PDF umwandeln
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, new StringReader(html));

            pdfDoc.Close();
            return File(stream.ToArray(), "application/pdf", $"Rechnung_{customer.FirstName}_{customer.LastName}_{DateTime.Now}.pdf");
        }

        public IActionResult PrepareInvoice(int orderId)
        {

        //    var order = _context.Orders.Include(x=>x.Customer).Where(x=>x.Id == orderId).FirstOrDefault();

            var order = _context.Orders.Include(x => x.Customer).Include(x => x.OrderProducts).ThenInclude(x => x.Products).ThenInclude(x => x.ProductCategory).Where(x => x.Id == orderId).FirstOrDefault();

            if (order == null)
            {
                return RedirectToAction("Index", "Customer");
            }

            List<(string, int, decimal)> itemList = new List<(string, int, decimal)>();

            foreach (var item in order.OrderProducts)
            {
                itemList.Add((item.Products.Name, item.Quantity, (item.Products.NetUnitPrice) * (1 + (item.Products.ProductCategory.TaxRate) / 100)));
            }

            string html = ReceiptGenerator.GenerateReceiptHtml(order.OrderDate.ToShortDateString(), order.Customer.FirstName + " " + order.Customer.LastName, itemList);

            return RedirectToAction(nameof(DownloadInvoice), new { id = order.Id, html = html });
        }

        /// <summary>
        /// Filters orders based on year, month, date, status, cancel and payment method.
        /// </summary>
        /// <returns>
        /// Returns a view with the filtered orders.
        /// </returns>
        public IActionResult Filter(int? orderyear, int? ordermonth, DateTime? orderdate1, DateTime? orderdate2, bool[]? status, bool? cancel, string[]? paymentMethod)
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);
            var filteredOrders = _context.Orders.Include(o => o.PaymentMethod).ToList();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }


            // Year and Month
            if (orderyear != null && ordermonth != null)
            {
                filteredOrders = filteredOrders.Where(o => o.OrderDate.Year == orderyear && o.OrderDate.Month == ordermonth).ToList();
            }

            // Specific date
            if (orderdate1 != null && orderdate2 != null)
            {
                DateTime date = (DateTime)orderdate1;
                DateTime date2 = (DateTime)orderdate2;
                filteredOrders = filteredOrders.Where(o => o.OrderDate.Day >= date.Day && o.OrderDate.Day <= date2.Day).ToList();
            }

            // Status
            if (status.Length != 0 || cancel != null)
            {
                if (status.Contains(true))
                {
                    filteredOrders = filteredOrders.Where(p => p.PaymentPending && !p.Cancelled).ToList();

                    if (cancel == true)
                    {
                        filteredOrders.AddRange(orderList.Where(p => p.Cancelled));
                    }
                    if (status.Contains(false))
                    {
                        filteredOrders.AddRange(orderList.Where(p => !p.PaymentPending && !p.Cancelled));
                    }
                }

                if (status.Contains(false))
                {
                    filteredOrders = filteredOrders.Where(p => !p.PaymentPending && !p.Cancelled).ToList();

                    if (cancel == true)
                    {
                        filteredOrders.AddRange(orderList.Where(p => p.Cancelled));
                    }
                    if (status.Contains(true))
                    {
                        filteredOrders.AddRange(orderList.Where(p => p.PaymentPending && !p.Cancelled));
                    }
                }

                if (cancel == true && status.Length == 0)
                {
                    filteredOrders = filteredOrders.Where(p => p.Cancelled).ToList();
                }

            }

            // Paymentmethod
            if (paymentMethod != null && paymentMethod.Length > 0)
            {
                filteredOrders = filteredOrders.Where(p => paymentMethod.Contains(p.PaymentMethod.Name)).ToList();
            }

            return View("Index", filteredOrders.ToList());
        }


        /// <summary>
        /// Changes the status of an order based on the user's role and the status provided.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="status">The status to set.</param>
        /// <returns>A redirect to the Index action.</returns>
        public IActionResult ChangeStatus(int userId, int itemId, int status)
        {
            var user = _context.Employees.Find(userId);

            if (user.Role < 3 && status == 1)
            {
                ViewBag.OrderId = itemId;

                return View("PasswordRequiredStatus");
            }
            else if (status == 1)
            {
                return RedirectToAction(nameof(Index));
            }


            var order = _context.Orders.Find(itemId);

            if (status == 2)
            {
                order.PaymentPending = false;
            }
            else if (status == 3)
            {
                order.PaymentPending = true;
            }
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));

        }

        /// <summary>
        /// Changes the status of an order based on the user's input.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A redirect to the Index page.</returns>
        public IActionResult ChangeStatusProc(int userId, int orderId, string password)
        {
            var user = _context.Employees.Find(userId);

            bool valid = PasswordManager.VerifyPassword(password, user.HashPassword, user.Salt);

            if (valid == true)
            {
                var item = _context.Orders.Find(orderId);

                if (item != null)
                {
                    if (item.Cancelled)
                    {
                        item.Cancelled = false;
                    }
                    else
                    {
                        item.Cancelled = true;
                    }
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Cancels an order with the given orderId if the payment is pending.
        /// </summary>
        /// <param name="orderId">The id of the order to cancel.</param>
        /// <returns>Redirects to the Index action if the payment is not pending, otherwise returns the view.</returns>
        public IActionResult Cancel(int orderId)
        {
            var order = _context.Orders.Find(orderId);

            if (order != null)
            {
                if (order.PaymentPending == false)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(orderId);
        }

        /// <summary>
        /// Cancels a given order if the user is authorized to do so.
        /// </summary>
        /// <param name="orderId">The ID of the order to be cancelled.</param>
        /// <param name="userId">The ID of the user attempting to cancel the order.</param>
        /// <param name="password">The password of the user attempting to cancel the order.</param>
        /// <returns>Redirects to the Index action.</returns>
        public IActionResult CancelProc(int orderId, int userId, string password)
        {
            var user = _context.Employees.Find(userId);

            if (user != null)
            {
                bool valid = PasswordManager.VerifyPassword(password, user.HashPassword, user.Salt);

                if (!valid)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (user.Role < 3)
                {
                    var order = _context.Orders.Find(orderId);

                    if (order != null)
                    {
                        if (order.Cancelled)
                        {
                            order.Cancelled = false;
                        }
                        else
                        {
                            order.Cancelled = true;
                        }

                        _context.SaveChanges();
                    }
                }
            }


            return RedirectToAction(nameof(Index));
        }

    }
}
