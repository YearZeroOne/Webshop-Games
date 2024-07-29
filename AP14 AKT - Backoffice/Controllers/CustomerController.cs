using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using AP14_AKT___Backoffice.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using WebShopLib.Models;
using WebShopLib.Services;

namespace AP14_AKT___Backoffice.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer
        /// <summary>
        /// Retrieves all customers from the database and returns them to the view.
        /// </summary>
        /// <returns>A view containing all customers.</returns>
        public IActionResult Index()
        {

            var allCustomers = _context.Customers
                .Include(x => x.Addresses)
                .ToList();

            return View(allCustomers);
        }

        /// <summary>
        /// Filters customers based on locked and unpaid status
        /// </summary>
        /// <param name="locked">Whether the customer is locked or not</param>
        /// <param name="unpaid">Whether the customer has unpaid orders or not</param>
        /// <returns>View with filtered customers</returns>
        public IActionResult Filter(bool? locked, bool? unpaid)
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);
            var filteredCustomers = _context.Customers.Include(c => c.Orders).AsQueryable();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }

            if (locked != null)
            {
                if (locked == true)
                {
                    filteredCustomers = filteredCustomers.Where(c => c.IsLocked == true);
                }
                else
                {
                    filteredCustomers = filteredCustomers.Where(c => c.IsLocked == false);
                }
            }

            if (unpaid != null)
            {
                if (unpaid == true)
                {
                    filteredCustomers = filteredCustomers.Where(c => c.Orders.FirstOrDefault().PaymentPending == true);
                }
                else
                {
                    filteredCustomers = filteredCustomers.Where(c => c.Orders.FirstOrDefault().PaymentPending == false);
                }
            }

            return View("Index", filteredCustomers.ToList());
        }

        // GET: Customer/Details/5
        /// <summary>
        /// Retrieves the details of a customer and related orders, products, payment methods and product categories.
        /// </summary>
        /// <param name="id">The id of the customer.</param>
        /// <returns>The view with the customer details.</returns>
        public IActionResult Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var customer = _context.Customers.Find(id);
            var orders = _context.Orders.Where(x => x.CustomerId == customer.Id).ToList();

            var viewModelList = new List<MultiViewModel>();

            foreach (var item in orders)
            {
                var model = new MultiViewModel();

                var paymentMethod = _context.PaymentMethods.Include(x => x.Orders).Where(x => x.Orders.Contains(item)).FirstOrDefault();

                var orderProducts = _context.Orders.Include(x => x.OrderProducts).ThenInclude(x => x.Products).Where(x => x == item).FirstOrDefault().OrderProducts;

                List<(Product, ProductCategory)> products = new List<(Product, ProductCategory)>();

                foreach (var orderProduct in orderProducts)
                {
                //    var category = _context.ProductCategories.Include(x => x.Products).ThenInclude(x=>x.OrderProducts).Where(x => x.Products == product).FirstOrDefault();

                    var product = _context.Products.Include(x=>x.ProductCategory).Where(x=>x.OrderProducts.Contains(orderProduct)).FirstOrDefault();

                    products.Add((product, product.ProductCategory));
                }

                model.Customer = customer;
                model.Order = item;
                model.OrderProduct = _context.OrderProducts.Where(x => x.OrderId == item.Id).ToList();
                model.PaymentMethod = paymentMethod;
                model.Product = products;

                viewModelList.Add(model);

            }
            //var viewModel = new MultiViewModel();
            //viewModel.OrderProduct = new List<OrderProduct>();
            //viewModel.PaymentMethod = new List<PaymentMethod>();
            //viewModel.Product = new List<Product>();
            //viewModel.ProductCategories = new List<ProductCategory>();


            //viewModel.Customer = _context.Customers.Where(x => x.Id == id).FirstOrDefault();
            //viewModel.Order = _context.Orders.Where(x => x.CustomerId == viewModel.Customer.Id).ToList();


            //if (viewModel.Order != null)
            //{
            //    foreach (var item in viewModel.Order)
            //    {

            //        viewModel.OrderProduct.Add(_context.OrderProducts.Where(x => x.OrderId == item.Id).FirstOrDefault());
            //    }
            //    foreach (var item in viewModel.OrderProduct)
            //    {
            //        if (item != null)
            //        {
            //            viewModel.Product.Add(_context.Products.Where(x => x.Id == item.ProductsId).FirstOrDefault());
            //        }

            //    }
            //    foreach (var item in viewModel.Order)
            //    {
            //        viewModel.PaymentMethod.Add(_context.PaymentMethods.Where(x => x.Id == item.PaymentMethodId).FirstOrDefault());
            //    }
            //}


            return View(viewModelList);
        }

        /// <summary>
        /// Searches for customers based on search string.
        /// </summary>
        /// <param name="searchString">The string to search for.</param>
        /// <returns>The view with the search results.</returns>
        public IActionResult Search(string searchString)
        {

            var customers = _context.Customers.ToList();


            if (string.IsNullOrEmpty(searchString)) return View("Index", customers);

            var thisOne = _context.Customers
                .Where(x => x.FirstName.ToLower().Contains(searchString.ToLower()) || x.CustomerNumber.ToLower().Contains(searchString.ToLower()) || x.Email.ToLower().Contains(searchString.ToLower()))
                .ToList();

            var thisCustomer = _context.Customers.Where(x => x.CustomerNumber.ToLower().Equals(searchString.ToLower())).FirstOrDefault();
            if (thisCustomer != null)
            {
                if (thisCustomer.CustomerNumber.Equals(searchString))
                {
                    var id = thisCustomer.Id;
                    return RedirectToAction(nameof(Details), new { id });
                }
            }

            return View("Index", thisOne);
        }

        /// <summary>
        /// Changes the locked status of a customer.
        /// </summary>
        /// <returns>
        /// Redirects to the customer details page.
        /// </returns>
        public IActionResult ChangeLocked(int id)
        {
            string guid = HttpContext.Request.Cookies["guid"];
            var currentUser = _context.Customers.Where(x => x.Guid == guid).FirstOrDefault();
            var customer = _context.Customers.Find(id);

            if (currentUser != null && customer != null && customer == currentUser && !customer.IsLocked)
            {
                return RedirectToAction("Details", "Customer", new { id });
            }



            if (customer == null)
            {
                return RedirectToAction("Details", "Customer", new { id });
            }

            if (customer.IsLocked)
            {
                customer.IsLocked = false;
            }
            else
            {
                customer.IsLocked = true;
            }
            _context.SaveChanges();

            return RedirectToAction("Details", "Customer", new { id });
        }


        /// <summary>
        /// Verifies the password of the current employee and changes the locked status of the customer if the password is correct.
        /// </summary>
        /// <param name="currentEmployeeId">The ID of the current employee.</param>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="password">The password of the current employee.</param>
        /// <returns>Redirects to the customer details page if the password is incorrect, otherwise changes the locked status of the customer.</returns>
        public IActionResult ChangeLockedStatusPasswordConfirm(int currentEmployeeId, int customerId, string password)
        {
            var employee = _context.Employees.Where(x => x.Id == currentEmployeeId).FirstOrDefault();

            var passwordTrue = PasswordManager.VerifyPassword(password, employee.HashPassword, employee.Salt);

            if (!passwordTrue)
            {
                TempData["PasswordWrogn"] = "Password do not match";
                return RedirectToAction("Details", new { customerId });
            }
            else
            {
                return ChangeLocked(customerId);
            }


        }

    }




}
