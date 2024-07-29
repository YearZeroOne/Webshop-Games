using AP14_AKT___Webshop.Models;
using iText.Commons.Actions.Contexts;
using iText.Layout.Splitting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Security.Claims;
using WebShopLib.Models;
using WebShopLib.Services;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static iTextSharp.text.pdf.AcroFields;

namespace AP14_AKT___Webshop.Controllers
{
    public class ShoppingCartController : Controller
    {


        ApplicationDbContext _context;
        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;

        }

        public IActionResult UpdateCart(string guid, int id, int quantity, decimal netprice)
        {
            // Find the cart item based on the provided GUID and item ID
            var cartItem = _context.Customers
                .Where(c => c.Guid == guid)
                .SelectMany(c => c.ShoppingCart.CartItems)
                .Include(ci => ci.Product)
                .SingleOrDefault(ci => ci.Id == id);

            if (quantity == 0)
            {
                // If the quantity is set to 0, remove the cart item from the context and save changes
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
            else if (quantity > cartItem.Product.Stock)
            {
                // If the requested quantity exceeds the available stock, update the cart item quantity to the maximum stock available
                cartItem.Quantity = cartItem.Product.Stock;
                cartItem.Price = netprice;

                _context.SaveChanges();
                TempData["CartFlag"] = "Menge überschreitet Lagerbestand. Maximal " + cartItem.Product.Stock + " möglich.";
                // Redirect to the Index action in the ShoppingCart controller with the provided GUID
                return RedirectToAction("Index", "ShoppingCart", new { guid });
            }
            else
            {
                // Update the cart item quantity and price
                cartItem.Quantity = quantity;
                cartItem.Price = netprice;
                _context.SaveChanges();
            }

            // Redirect to the Index action in the ShoppingCart controller with the provided GUID
            return RedirectToAction("Index", "ShoppingCart", new { guid });
        }




        public IActionResult RemoveItem(string guid, int id)
        {
            var cartItem = _context.Customers
    .Where(c => c.Guid == guid)
    .SelectMany(c => c.ShoppingCart.CartItems)
    .Include(ci => ci.Product)
    .SingleOrDefault(ci => ci.Id == id);


            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }


            // new { guid } is needed to pass a route value or it will cause System.InvalidOperationException: 'Sequence contains more than one element'
            return RedirectToAction("Index", "ShoppingCart", new { guid });
        }



        public IActionResult FinishPayment(string guid, int id, int customerid, bool deliveryOption, decimal shippingCost, int shippingAddress, string paymentOption, string? cardholderName, string? cardNumber, string? expirationDate, int? cvc)
        {
            var products = _context.CartItems
                 .Where(ci => ci.ShoppingCartId == id)
                 .Include(ci => ci.Product)
                 .Include(ci => ci.Product.ProductCategory)
                 .ToList();
            var email = _context.Customers
     .Where(u => u.Guid == guid)
     .Select(u => u.Email)
     .FirstOrDefault();
            if (deliveryOption)
            {
                if (paymentOption == "bankTransfer")
                {
                    TempData["bankTransferFlag"] = "Überweisung ist nur für Lieferung verfügbar.";
                    return RedirectToAction("OrderConfirmation", "ShoppingCart", new { guid, id });

                }
            }
            else if (deliveryOption == false)
            {
                if (paymentOption == "cash")
                {
                    TempData["cashFlag"] = "Barzahlung ist nur für Abholung verfügbar.";
                    return RedirectToAction("OrderConfirmation", "ShoppingCart", new { guid, id });

                }
            }
         

                var tempDataMessages = new Dictionary<string, string>(); // create dictionary object to store all temp data messages

                foreach (var item in products)
                {
                    // Checks if price still matches
                    if (item.Price != item.Product.NetUnitPrice)
                    {
                        tempDataMessages["PriceFlag_" + item.Id] = "Der Preis von " + item.Product.Name + " wurde auf " + ((item.Product.NetUnitPrice * (1 + item.Product.ProductCategory.TaxRate / 100)).ToString("0.00")) + " angepasst.";
                        item.Price = item.Product.NetUnitPrice;
                        _context.SaveChanges();
                    }

                    // check if quantity is okay
                    if (item.Quantity > item.Product.Stock)
                    {
                        tempDataMessages["StockFlag_" + item.Id] = item.Product.Name + ": Gewünschte Menge nicht mehr verfügbar. Maximal " + item.Product.Stock + " möglich.";
                    }

                    if (item.Product.IsActive == false)
                    {
                        tempDataMessages["IsActiveFlag_" + item.Id] = item.Product.Name + " wurde auf inaktiv gesetzt.";
                        _context.CartItems.Remove(item);
                        _context.SaveChanges();
                    }

                    if (item.Product.Stock == 0)
                    {
                        tempDataMessages["StockZeroFlag_" + item.Id] = item.Product.Name + " ist ausverkauft.";
                        _context.CartItems.Remove(item);
                        _context.SaveChanges();
                    }
                }

                // set all the temp data messages
                foreach (var message in tempDataMessages)
                {
                    TempData[message.Key] = message.Value;
                }

                if (tempDataMessages.Count == 0)
                {
                    var orders = _context.Orders.Include(x => x.PaymentMethod).ToList();
                    var creditCard = _context.CreditCards.ToList();




                    int paymentMethodId;

                    if (paymentOption == "creditCard")
                    {

                        if (string.IsNullOrEmpty(cardholderName) || string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(expirationDate) || cvc == null)
                        {

                            TempData["PaymentError"] = "Bitte geben Sie ihre Kreditkarte ein.";

                            return RedirectToAction("OrderConfirmation", "ShoppingCart", new { guid, id });
                        }
                        else
                        {
                            paymentMethodId = 1; // Credit Card payment method number

                        }
                    }
                    else if (paymentOption == "bankTransfer")
                    {
                        paymentMethodId = 2; // Bank Transfer payment method number
                    }
                    else if (paymentOption == "cash")
                    {
                        paymentMethodId = 3; // Cash payment method number
                    }

                    else
                    {
                        paymentMethodId = 0;
                    }


                    var newOrder = new Order
                    {
                        CustomerId = customerid,
                        OrderDate = DateTime.Now,
                        DeliveryDate = null,
                        PaymentPending = true,
                        PickUp = deliveryOption,
                        PaymentMethodId = paymentMethodId,
                        Cancelled = false,

                    };


                    

                    newOrder.OrderNumber = NumbersystemManager.NumberSystem(newOrder);
                    if (paymentOption != "creditCard")
                    {
                        newOrder.PaymentPending = false;
                    }

                    _context.Orders.Add(newOrder);
                    _context.SaveChanges();



                    foreach (var cartItem in products)
                    {
                        OrderProduct newOrderProduct = new OrderProduct
                        {
                            ProductsId = cartItem.ProductId,
                            OrderId = newOrder.Id,
                            Quantity = cartItem.Quantity,
                            Price = cartItem.Price,

                        };
                        Product product = _context.Products.Find(cartItem.ProductId);

                        if (product != null)
                        {
                            product.Stock -= cartItem.Quantity;

                            _context.SaveChanges();
                        }
                        _context.OrderProducts.Add(newOrderProduct);
                        _context.SaveChanges();



                    }
                    // Retrieve the cart items to delete
                    List<CartItem> cartItemsToDelete = _context.CartItems
                        .Where(ci => ci.ShoppingCartId == id)
                        .ToList();

                    // Delete the cart items from the database
                    _context.CartItems.RemoveRange(cartItemsToDelete);

                    _context.SaveChanges();
                    var orderId = newOrder.Id;

                    return RedirectToAction("ThankYouPage", "Order", new { orderId, shippingCost, email });

                }
            
            return RedirectToAction("Index", "ShoppingCart", new { guid, id });

           

        }



        public IActionResult ProceedPayment(string guid, int id)
        {
            var products = _context.CartItems
                .Where(ci => ci.ShoppingCartId == id)
                .Include(ci => ci.Product)
                .ThenInclude(ci => ci.ProductCategory)
                .ToList();

            var tempDataMessages = new Dictionary<string, string>(); // create dictionary object to store all temp data messages

            foreach (var item in products)
            {
                var gross = Math.Round(item.Product.NetUnitPrice * (1 + item.Product.ProductCategory.TaxRate / 100), 2);

                // Checks if price still matches
                if (item.Price != item.Product.NetUnitPrice)
                {
                    tempDataMessages["PriceFlag_" + item.Id] = "Der Preis von " + item.Product.Name + " wurde auf Euro " + gross + " geändert.";
                    item.Price = item.Product.NetUnitPrice;
                    _context.SaveChanges();
                }

                // check if quantity is okay
                if (item.Quantity > item.Product.Stock)
                {
                    tempDataMessages["StockFlag_" + item.Id] = item.Product.Name + ": Gewünschte Menge nicht mehr verfügbar. Maximal " + item.Product.Stock + " möglich.";
                }

                if (item.Product.IsActive == false)
                {
                    tempDataMessages["IsActiveFlag_" + item.Id] = item.Product.Name + " wurde auf inaktiv gesetzt.";
                    _context.CartItems.Remove(item);
                    _context.SaveChanges();
                }

                if (item.Product.Stock == 0)
                {
                    tempDataMessages["StockZeroFlag_" + item.Id] = item.Product.Name + " ist ausverkauft.";
                    _context.CartItems.Remove(item);
                    _context.SaveChanges();
                }
            }

            // set all the temp data messages
            foreach (var message in tempDataMessages)
            {
                TempData[message.Key] = message.Value;
            }

            if (tempDataMessages.Count == 0)
            {
                return RedirectToAction("OrderConfirmation", "ShoppingCart", new { guid, id });
            }

            return RedirectToAction("Index", "ShoppingCart", new { guid, id });
        }

        public IActionResult OrderConfirmation(string guid)
        {
            var customer = _context.Customers.Include(c => c.ShoppingCart)
                                             .Include(c => c.Addresses)
                                             .SingleOrDefault(c => c.Guid == guid);

            var shoppingCart = _context.ShoppingCarts.Where(x => x.CustomerId == customer.Id).FirstOrDefault();

            List<CartItem> cartItems = _context.CartItems
                                                .Include(x => x.Product)
                                                    .ThenInclude(x => x.ProductCategory)
                                                .Include(x => x.Product.Images)
                                                .Where(x => x.ShoppingCartId == shoppingCart.Id)

                                                .ToList();

            var addresses = customer.Addresses.ToList();

            // Pass cart items and addresses to the view
            var model = new OrderConfirmationViewModel
            {
                CartItems = cartItems,
                Addresses = addresses
            };

            return View(model);
        }




        public IActionResult Index(string guid)
        {
            var user = _context.Customers.Where(x => x.Guid == guid).FirstOrDefault();

            if (user == null)
            {
                // Delete the login cookie
                string loginCookie = CookieManager.LoginCookieGuidGet(HttpContext);
                HttpContext.Response.Cookies.Delete("LoginCookie");
                return RedirectToAction("Login", "User");
            }


            var customer = _context.Customers.Include(c => c.ShoppingCart)
                .SingleOrDefault(c => c.Guid == guid);


            if (customer.ShoppingCart == null)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    CustomerId = customer.Id
                };
                _context.ShoppingCarts.Add(shoppingCart);
                _context.SaveChanges();


                var newShoppingCart = _context.ShoppingCarts.Where(x => x.CustomerId == customer.Id).FirstOrDefault();

                List<CartItem> cartItems = new List<CartItem>();
                cartItems = _context.CartItems
    .Include(x => x.Product)
        .ThenInclude(x => x.ProductCategory)
    .Include(x => x.Product.Images)
    .Where(x => x.ShoppingCartId == shoppingCart.Id)
    .ToList();


                // return the newly created shopping cart to the view
                return View(cartItems);
            }
            else
            {
                // retrieve the customer's shopping cart from the database

                var shoppingCart = _context.ShoppingCarts.Where(x => x.CustomerId == customer.Id).FirstOrDefault();

                List<CartItem> cartItems = new List<CartItem>();
                cartItems = _context.CartItems
    .Include(x => x.Product)
        .ThenInclude(x => x.ProductCategory)
    .Include(x => x.Product.Images)
    .Where(x => x.ShoppingCartId == shoppingCart.Id)
    .ToList();



                // return the shopping cart with cart items to the view
                return View(cartItems);
            }
        }

    }
}







