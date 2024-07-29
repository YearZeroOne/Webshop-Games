using AP14_AKT___Webshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NuGet.Protocol.Plugins;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using WebShopLib;
using WebShopLib.Models;
using WebShopLib.Services;

namespace AP14_AKT___Webshop.Controllers
{
    public class ProductController : Controller
    {

        ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;

        }


        public IActionResult PublicIndex()
        {
            // Retrieve the products from the database, including related entities
            var products = _context.Products
                            .Include(x => x.Genre)
                            .Include(p => p.ProductCategory)
                            .Include(p => p.Images)
                            .Include(p => p.Developer)
                            .ToList();

            // Pass the list of products to the view
            return View(products);
        }





        public IActionResult Index(string guid)
        {
            // Find the user based on the provided GUID
            var user = _context.Customers.Where(x => x.Guid == guid).FirstOrDefault();

            if (user == null)
            {
                // Delete the login cookie
                string loginCookie = CookieManager.LoginCookieGuidGet(HttpContext);
                HttpContext.Response.Cookies.Delete("LoginCookie");
                // Redirect to the Login action in the User controller
                return RedirectToAction("Login", "User");
            }

            // Retrieve the products from the database, including related entities
            var products = _context.Products
                            .Include(x => x.Genre)
                            .Include(p => p.ProductCategory)
                            .Include(p => p.Images)
                            .Include(p => p.Developer)
                            .ToList();

            // Pass the list of products to the view
            return View(products);
        }



        [HttpPost]
        public IActionResult AddToCartWithQuantity(int productId, string guid, int quantity, decimal price)
        {
            // Find the customer based on the provided GUID
            var customer = _context.Customers.SingleOrDefault(c => c.Guid == guid);

            // Get the shopping cart for the current customer
            var shoppingCart = _context.ShoppingCarts
                .Include(c => c.CartItems)
                .SingleOrDefault(c => c.CustomerId == customer.Id);

            // If the shopping cart doesn't exist, create a new one
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    CustomerId = customer.Id
                };
                _context.ShoppingCarts.Add(shoppingCart);
                _context.SaveChanges();
            }

            // Find the cart item for the product, or create a new one
            var cartItem = shoppingCart.CartItems
                .SingleOrDefault(ci => ci.ProductId == productId);



            if (cartItem == null)
            {
                // Create a new cart item for the product
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = price
                };
                shoppingCart.CartItems.Add(cartItem);
            }


            else
            {
                int sumQuantity= cartItem.Quantity + quantity;

                if (sumQuantity > 99)
                {
                    TempData["sumQuantityError"] = "Maximale Anzahl - pro Spiel - im Warenkorb: 99 Stück";


                    return RedirectToAction("Details", new { id = productId });
                }

                else
                {
                    // Update the quantity and price of the existing cart item
                    cartItem.Quantity += quantity;
                    cartItem.Price = price;
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }

            }
                      
            // Save the changes to the database
            _context.SaveChanges();

            // Redirect to the index action
            return RedirectToAction("Index");
        }




        [HttpPost]
        public IActionResult AddToCart(int productId, string guid, decimal price)
        {

            // Find the customer based on the provided GUID
            var customer = _context.Customers.SingleOrDefault(c => c.Guid == guid);
            if (customer == null)
            {
                // handle the case where no customer with the provided GUID exists
                return NotFound();
            }

            // Get the shopping cart for the current customer
            var shoppingCart = _context.ShoppingCarts
                .Include(c => c.CartItems)
                .SingleOrDefault(c => c.CustomerId == customer.Id);

            // If the shopping cart doesn't exist, create a new one
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    CustomerId = customer.Id
                };
                _context.ShoppingCarts.Add(shoppingCart);
                _context.SaveChanges();
            }

            // Find the cart item for the product, or create a new one
            var cartItem = shoppingCart.CartItems
                .SingleOrDefault(ci => ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    Price = price
                };
                shoppingCart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
                cartItem.Price = price;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        public IActionResult Details(int id)
        {
            // Fetch the quantity of the product in the shopping cart
            int difference;

            // Find the corresponding cart item for the given product ID
            var shoppingCart = _context.CartItems.Where(x => x.ProductId == id).FirstOrDefault();

            // Check if the cart item exists
            if (shoppingCart == null)
            {
                // The product is not in the cart, so the difference is 0
                difference = 0;
            }
            else
            {
                // Get the quantity from the cart item
                difference = shoppingCart.Quantity;
            }

            // Retrieve the product with the specified ID from the database
            var product = _context.Products
                // Include the related entities
                .Include(p => p.ProductCategory)
                .Include(x => x.Developer)
                .Include(p => p.Images)
                .Include(p => p.Genre)
                // Retrieve the first product that matches the specified ID
                .FirstOrDefault(p => p.Id == id);

            // Update the stock of the product by subtracting the difference (quantity in the cart)
            product.Stock -= difference;

            // Set a temporary data to display a message about the maximum stock available
            TempData["MaximumStock"] = "Menge überschreitet Lagerbestand. Maximal " + product.Stock + " möglich.";



            // Pass the retrieved product object to the view
            return View(product);
        }



        public IActionResult PublicDetails(int id)
        {
            // Retrieve the product with the specified ID from the database
            var product = _context.Products.Include(p => p.ProductCategory)
                                          .Include(x => x.Developer)
                                          .Include(p => p.Images)
                                          .Include(p => p.Genre)
                                          .FirstOrDefault(p => p.Id == id);

            // Pass the retrieved product object to the view
            return View(product);
        }



        public IActionResult Search(string searchString)
        {
            // Retrieve all products from the database, including related entities
            var products = _context.Products.Include(p => p.ProductCategory).Include(p => p.Developer).Include(p => p.Images).ToList();

            // If the search string is empty or null, return all products to the "Index" view
            if (string.IsNullOrEmpty(searchString))
            {
                return View("Index", products);
            }

            // Perform a search for products based on the search string
            var games = _context.Products
                .Where(x => x.Name.ToLower().Contains(searchString.ToLower())
                            || x.ProductNumber.ToLower().Contains(searchString.ToLower())
                            || x.Description.ToLower().Contains(searchString.ToLower()))
                .ToList();

            // Return the search results to the "Index" view
            return View("Index", games);
        }


        public IActionResult Filter(string[] publishers, string[] categories, string[] productTypes, int? minPrice, int? maxPrice)
        {
            // Create a queryable object to store the filtered products
            var filteredProducts = _context.Products.Include(p => p.ProductCategory).Include(p => p.Genre).Include(p => p.Developer).Include(p => p.Images).AsQueryable();

            // Filter by publishers
            if (publishers != null && publishers.Length > 0)
            {
                // Filter the products based on the specified publishers
                filteredProducts = filteredProducts.Where(p => publishers.Contains(p.Developer.Name.ToLower()));
            }

            // Filter by categories
            if (categories != null && categories.Length > 0)
            {
                // Filter the products based on the specified categories
                filteredProducts = filteredProducts.Where(p => categories.Contains(p.Genre.Name.ToLower()));
            }

            // Filter by product types
            if (productTypes != null && productTypes.Length >= 0)
            {
                // Filter the products based on the specified product types
                filteredProducts = filteredProducts.Where(p => productTypes.Contains(p.ProductCategory.Name.ToLower()));
            }

            // Filter by minimum price
            if (minPrice != null)
            {
                // Filter the products based on the minimum price, considering a discount of 20%
                filteredProducts = filteredProducts.Where(p => p.NetUnitPrice >= minPrice * 0.8m);
            }

            // Filter by maximum price
            if (maxPrice != null)
            {
                // Filter the products based on the maximum price, considering a discount of 20%
                filteredProducts = filteredProducts.Where(p => p.NetUnitPrice <= maxPrice * 0.8m);
            }

            // Return the filtered products as a list to the "Index" view
            return View("Index", filteredProducts.ToList());
        }



    }
}
