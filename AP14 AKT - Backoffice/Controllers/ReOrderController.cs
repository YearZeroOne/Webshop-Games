using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebShopLib.Models;
using WebShopLib.Services;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static iTextSharp.text.pdf.AcroFields;

namespace AP14_AKT___Backoffice.Controllers
{
    public class ReOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string _conStr;
        public List<ReOrder> reorderList;

        public ReOrderController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _conStr = config.GetConnectionString("ConnectionString");
            reorderList = new List<ReOrder>();
            UpdateOrderList();
        }

        private void UpdateOrderList()
        {
            reorderList = _context.ReOrders.ToList();
        }

        /// <summary>
        /// Retrieves a list of ReOrders from the database and filters them based on the given parameters.
        /// </summary>
        /// <param name="filter">The filter to apply to the list of ReOrders.</param>
        /// <param name="status">The status to filter the ReOrders by.</param>
        /// <returns>A view containing the filtered list of ReOrders.</returns>
        public IActionResult Index(int? filter, ReOrderStatus? status)
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);
            var reOrderList = _context.ReOrders.ToList();
            reOrderList.OrderBy(reOrderStatus => reOrderStatus.LastStatusChange);
            List<ReOrder> filteredList = new List<ReOrder>();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }

            // Filter
            if (filter == 1)
            {
                foreach (var reOrder in reOrderList)
                {
                    var reOrderStatus = reOrder.Status;
                    if (reOrderStatus == status)
                    {
                        filteredList.Add(reOrder);
                    }
                }
                filteredList.OrderByDescending(status => status.LastStatusChange);
                return View(filteredList);
            }

            return View(reOrderList);
        }

        public IActionResult Filter(int[]? status)
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);
            var filteredReOrders = _context.ReOrders.ToList();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }

            if (status != null)
            {
                if (status.Length == 1)
                {
                    if (status.Contains(0))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Created).ToList();
                    }

                    if (status.Contains(1))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();
                    }

                    if (status.Contains(2))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.PartiallyFulfilled).ToList();
                    }

                    if (status.Contains(3))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Complete).ToList();
                    }

                }


                if (status.Length == 2)
                {
                    if (status.Contains(0) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Created).ToList();

                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(1) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(2) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(3) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                    }

                }


                if (status.Length == 3)
                {
                    if (status.Contains(0) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Created).ToList();

                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(1) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(2) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(3) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                    }

                }


                if (status.Length == 4)
                {
                    if (status.Contains(0) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Created).ToList();

                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(1) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(2) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(3)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Complete)); }
                    }

                    if (status.Contains(3) && filteredReOrders.Count == reorderList.Count || filteredReOrders.Equals(reorderList))
                    {
                        filteredReOrders = filteredReOrders.Where(s => s.Status == ReOrderStatus.Sent).ToList();

                        if (status.Contains(0)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Created)); }
                        if (status.Contains(1)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.Sent)); }
                        if (status.Contains(2)) { filteredReOrders.AddRange(reorderList.Where(r => r.Status == ReOrderStatus.PartiallyFulfilled)); }
                    }

                }
            }

            filteredReOrders.Sort((a, b) => a.LastStatusChange.CompareTo(b.LastStatusChange));

            return View("Index", filteredReOrders.ToList());
        }

        public IActionResult Edit(int id)
        {
            var reorder = _context.ReOrders.Find(id);

            return View(reorder);
        }

        public IActionResult Details(int id)
        {
            var reOrder = _context.ReOrders.Find(id);

            return View(reOrder);
        }

        /// <summary>
        /// Creates a new ReOrder object and adds it to the database.
        /// </summary>
        /// <returns>
        /// Redirects to the Index action.
        /// </returns>
        public IActionResult NewReOrderProc(int id)
        {
            ReOrder reOrder = new ReOrder(_context);

            reOrder.CreatorId = id;
            reOrder.LastEditorId = id;
            reOrder.OrderDate = DateTime.Now;
            reOrder.LastStatusChange = DateTime.Now;
            reOrder.ReOrderNumber = NumbersystemManager.NumberSystem(reOrder);
            reOrder.Sent = false;

            _context.ReOrders.Add(reOrder);
            _context.SaveChanges();

            TempData["NewReOrder"] = "New Reorder was added.";
            return RedirectToAction(nameof(Index));

        }

        public IActionResult AddDelivery(int id)
        {
            return View(id);
        }

        /// <summary>
        /// This method adds a delivery to the database and generates a delivery number for it.
        /// </summary>
        /// <param name="delivery">The delivery to be added.</param>
        /// <returns>Redirects to the details page of the delivery.</returns>
        public IActionResult AddDeliveryProc(Delivery delivery)
        {
            delivery.DeliveryNumber = NumbersystemManager.NumberSystem(delivery);

            _context.Deliveries.Add(delivery);
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { Id = delivery.ReOrderId });
        }

        /// <summary>
        /// Retrieves a product from the database and returns it to the view.
        /// </summary>
        /// <param name="id">The id of the product to be retrieved.</param>
        /// <returns>The view containing the retrieved product.</returns>
        public IActionResult NewReOrderFromProd(int id)
        {
            Product prod = _context.Products.Where(x => x.Id == id).FirstOrDefault();

            return View(prod);
        }

        /// <summary>
        /// This method returns a view for adding product quantity.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns>
        /// A view for adding product quantity.
        /// </returns>
        public IActionResult NewReOrderWithProd(int productId)
        {
            ViewBag.ProductId = productId;

            return View("AddProductQuantity");
        }
        /// <summary>
        /// Creates a new ReOrder with a Delivery for a given Product and User.
        /// </summary>
        /// <param name="productId">The ID of the Product.</param>
        /// <param name="userId">The ID of the User.</param>
        /// <param name="quantity">The quantity of the Product.</param>
        /// <returns>Redirects to the Index action.</returns>

        public IActionResult NewReOrderWithProdProc(int productId, int userId, int quantity)
        {
            ReOrder reOrder = new ReOrder(_context);
            Delivery delivery = new Delivery();

            reOrder.CreatorId = userId;
            reOrder.LastEditorId = userId;
            reOrder.OrderDate = DateTime.Now;
            reOrder.LastStatusChange = DateTime.Now;
            reOrder.ReOrderNumber = NumbersystemManager.NumberSystem(reOrder);
            reOrder.Sent = false;

            _context.ReOrders.Add(reOrder);
            _context.SaveChanges();

            delivery.ProductId = productId;
            delivery.ReOrderId = reOrder.Id;
            delivery.Quantity = quantity;
            delivery.DeliveryNumber = NumbersystemManager.NumberSystem(delivery);

            _context.Deliveries.Add(delivery);
            _context.SaveChanges();

            TempData["NewReOrder"] = "New Reorder was added.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// This method adds a product to a reorder.
        /// </summary>
        /// <param name="productId">The id of the product to add.</param>
        /// <param name="reOrderId">The id of the reorder.</param>
        /// <returns>
        /// Returns a view to add the product quantity to the reorder.
        /// </returns>
        public IActionResult AddProdToReOrder(int productId, int reOrderId)
        {
            ViewBag.ProductId = productId;
            ViewBag.ReOrderId = reOrderId;

            return View("AddProductQuantityReOrder");
        }
        /// <summary>
        /// Adds a product to a reorder process.
        /// </summary>
        /// <param name="productId">The ID of the product to be added.</param>
        /// <param name="reOrderId">The ID of the reorder process.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="quantity">The quantity of the product to be added.</param>
        /// <returns>Redirects to the Index action.</returns>

        [HttpPost]
        public IActionResult AddProdToReOrderProc(int productId, int reOrderId, int userId, int quantity)
        {
            userId = _context.Employees.Where(x => x.Guid == HttpContext.Request.Cookies["guid"]).FirstOrDefault().Id;

            var reOrder = _context.ReOrders.Find(reOrderId);
            Delivery delivery = new Delivery();

            delivery.CreatorId = userId;
            delivery.ProductId = productId;
            delivery.ReOrderId = reOrderId;
            delivery.Quantity = quantity;
            delivery.DeliveryNumber = NumbersystemManager.NumberSystem(delivery);

            reOrder.LastEditorId = userId;

            _context.Deliveries.Add(delivery);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Reorders a surrender request and redirects to the index page.
        /// </summary>
        /// <param name="Id">The ID of the reorder.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A redirect
        public IActionResult ReOrderSurrender(int Id, int userId)
        {
            var user = _context.Employees.Find(userId);

            if (user.Role == 3)
            {
                return RedirectToAction(nameof(Index));
            }

            var reOrder = _context.ReOrders.Find(Id);

            reOrder.Sent = true;
            reOrder.SentDate = DateTime.Now;
            reOrder.LastEditorId = userId;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves the ReOrder object with the specified reOrderId and returns a View with the ReOrder object.
        /// </summary>
        /// <param name="reOrderId">The Id of the ReOrder object to be retrieved.</param>
        /// <returns>A View with the ReOrder object.</returns>
        public IActionResult ReOrderDelivered(int reOrderId)
        {
            var reOrder = _context.ReOrders.Find(reOrderId);

            return View("Delivered", reOrder);
        }

        /// <summary>
        /// Reorders a delivery and updates the stock of the product.
        /// </summary>
        /// <param name="deliveryId">The ID of the delivery to be reordered.</param>
        /// <param name="userId">The ID of the user who is reordering the delivery.</param>
        /// <param name="stock">The amount of stock to be added to the product.</param>
        /// <returns>Redirects to the Index action.</returns>
        public IActionResult ReOrderDeliveredProc(int deliveryId, int userId, int stock)
        {
            var deliv = _context.Deliveries.Include(x => x.Product).Where(x => x.Id == deliveryId).FirstOrDefault();
            var reOrder = _context.ReOrders.Include(x => x.Deliveries).Where(x => x.Deliveries.FirstOrDefault().Id == deliveryId).FirstOrDefault();

            reOrder.LastEditorId = userId;
            deliv.Completed = true;
            deliv.DeliveryDate = DateTime.Now;
            deliv.RecieverId = userId;
            deliv.Product.Stock += stock;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }





    }
}
