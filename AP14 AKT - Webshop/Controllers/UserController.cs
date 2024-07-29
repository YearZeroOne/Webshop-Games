using AP14_AKT___Webshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using WebShopLib.Models;
using WebShopLib.Services;
using static iText.Kernel.Pdf.Colorspace.PdfPattern;

namespace AP14_AKT___Webshop.Controllers
{
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;
        private string _conStr;

        public UserController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _conStr = config.GetConnectionString("ConnectionString");
        }

        // GET: /User/Login
        // Displays the login view
        public IActionResult Login()
        {
            return View();
        }


        // POST: /User/LoginProc
        // Handles the login form submission
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult LoginProc(string email, string password, bool checkbox, string guid)
        {

            // Retrieve the user from the database based on the provided email
            var user = _context.Customers.Where(x => x.Email == email).FirstOrDefault();


            if (user != null)
            {
                // Retrieve the user's GUID from the login cookie
                var userGuid = CookieManager.CookieGuidGet(HttpContext, "guid");

                // Check if the user's GUID matches the one stored in the database
                if (userGuid.Equals(user.Guid, StringComparison.OrdinalIgnoreCase))
                {
                    RedirectToAction("Index");
                }
                // 
                else if (userGuid != user.Guid)
                {
                    // Update the user's GUID in the database and set a new login cookie
                    user.Guid = CookieManager.CookieGuidSet(HttpContext);

                    // Update the user's GUID in the database using SQL
                    using SqlConnection connection = new SqlConnection(_conStr);
                    connection.Open();
                    string updateSql = "UPDATE Customer SET Guid = '" + user.Guid + "' WHERE Id = " + user.Id;
                    SqlCommand command = new SqlCommand(updateSql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                // Verify the provided password against the user's hashed password and salt
                var passwordTrue = PasswordManager.VerifyPassword(
                password,
                user.HashPass,
                user.Salt);

                // Retrieve the customer from the database based on the provided email
                var customers = _context.Customers.Where(x => x.Email == email).FirstOrDefault();

                if (passwordTrue)
                {

                    if (customers.IsLocked)
                    {
                        // Display a message indicating that the account is locked
                        TempData["LoginFlag"] = "Ihr Konto wurde gesperrt. Melden Sie sich beim Kundendienst.";
                        return RedirectToAction(nameof(Login));
                    }

                    if (customers.IsActive == false)
                    {
                        // Display a message indicating that the account is inactive
                        TempData["LoginFlag"] = "Ihr Konto ist inaktiv";
                        return RedirectToAction(nameof(Login));
                    }

                    // Set login cookie if the customer is not locked and active
                    CookieManager.LoginCookieSet(HttpContext);

                 
                    if (checkbox == true)
                    {
                        // Set login cookie that expires in 30 days
                        CookieOptions option = new CookieOptions();
                        option.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Append("LoginCookie", user.Guid, option);
                    }

                    if (passwordTrue)
                    {
                        // Update last login date for the customer
                        customers.LastLogin = DateTime.Now;
                        _context.SaveChanges();                  
                    }
                }

                else
                {

                    if (!passwordTrue)
                    {
                     
                        if (customers.UnsuccessfullLogin == null)
                        {
                            // If this is the first unsuccessful login attempt, set the counter to 1
                            customers.UnsuccessfullLogin = 1;
                            _context.SaveChanges();
                        }
                        else
                        {
                            // Increment the unsuccessful login counter
                            customers.UnsuccessfullLogin += 1;
                            _context.SaveChanges();
                        }


                        if (customers.UnsuccessfullLogin >= 10)
                        {
                            // Lock the customer's account if there have been 10 or more unsuccessful login attempts
                            customers.IsLocked = true;
                            _context.SaveChanges();
                        }

                        // Display a message indicating that the login failed
                        TempData["LoginFlag"] = "Login fehlgeschlagen";
                        return RedirectToAction("Login");
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }

        // GET: /User/Registry
        // Displays the Registry view
        public IActionResult Registry()
        {

            return View();
        }


        // POST: /User/RegisterProc
        // Handles the registry form
        [HttpPost]
            public IActionResult RegisterProc(string firstname, string lastname, string email, string phone, int gender, string password, string addressline, string country, string city, int zipcode)
            {
                // run through each customer in the database to check if the email is already in use
                foreach (var userEmailInDb in _context.Customers)
                {
                    // If the email is found in the database, set a flag and return to the registration view
                    if (userEmailInDb.Email == email)
                    {
                        TempData["EmailFlag"] = "Email bereits verwendet";
                        return View("Registry");
                    }               
                }

                // Generate a salt for password hashing
                var salt = PasswordManager.GenerateSalt(16);

                // Save the new customer's information, including the hashed password, to the database
                PasswordManager.SaveNewCustomerToDb(firstname, lastname, email, phone, gender, password, addressline, country, city, zipcode, salt);

            return RedirectToAction(nameof(Success));
            }


        public IActionResult AccountManagement(string guid)
        {
            // Get the Customer object from the database using the provided guid
            Customer user = _context.Customers
            .Include(c => c.Orders)                             // Include the customer's orders in the query
                .ThenInclude(o => o.OrderProducts)              // Include the products within each order
                    .ThenInclude(op => op.Products)             // Include the detailed information of each product
                        .ThenInclude(p => p.ProductCategory)    // Include the category of each product
            .Include(c => c.Addresses)                      // Include the customer's addresses
            .FirstOrDefault(c => c.Guid == guid);           // Retrieve the customer with the matching guid


            // Pass the Customer object to the view for rendering and processing
            return View(user);
        }


        public IActionResult AddressManagement(string guid)
        {
            // Retrieve the Customer object from the database using the provided guid, including related orders and addresses
            Customer user = _context.Customers.Include(c => c.Orders).Include(c => c.Addresses).FirstOrDefault(c => c.Guid == guid);

            // Check if there's a warning message to be displayed
            if (TempData["LastAddressWarning"] != null)
            {
                // Pass the warning message to the view via ViewData
                ViewData["LastAddressWarning"] = TempData["LastAddressWarning"].ToString();
            }

            // Pass the Customer object to the view
            return View(user);
        }


        // POST: /User/AddAddress
        [HttpPost]
        public IActionResult AddAddress(int customerId, string firstName, string lastName, string addressLine, string city, int zipcode, string country, string guid) // to refactor, model Address is better
        {
            // Create a new Address object with the provided information
            Address newAddress = new()
            {
                CustomerId = customerId,
                FirstName = firstName,
                LastName = lastName,
                AddressLine = addressLine,
                City = city,
                Zipcode = zipcode,
                Country = country,
                IsBillingAddress = false,
                IsDeliveryAddress = false
            };

            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Add the new address to the database and save changes
                _context.Addresses.Add(newAddress);
                _context.SaveChanges();

                return RedirectToAction(nameof(AddressManagement), "User", new { guid });
            }

            return RedirectToAction(nameof(AddressManagement), "User", new { guid });
        }


        // POST: /User/UpdateAddresses
        [HttpPost]
        // the method can also be called using this name
        [ActionName("UpdateShippingAndBillAddress")]
        public IActionResult UpdateAddresses(int shippingAddressId, int billAddressId, string guid) 
        {
            // Retrieve the shipping and billing addresses from the database based on the provided IDs
            Address shippingAddress = _context.Addresses.FirstOrDefault(a => a.Id == shippingAddressId); 
            Address billAddress = _context.Addresses.FirstOrDefault(a => a.Id == billAddressId);

            if (shippingAddress != null && billAddress != null)
            {
                // Set the IsDeliveryAddress property to true for the shipping address
                shippingAddress.IsDeliveryAddress = true;

                // Set the IsBillingAddress property to true for the billing address
                billAddress.IsBillingAddress = true;

                // Update other addresses of the same customer to ensure only one address is marked as the delivery address and one as the billing address
                foreach (var address in _context.Addresses.Where(a => a.CustomerId == shippingAddress.CustomerId))
                {
                    if (address.Id != shippingAddressId)
                    {
                        address.IsDeliveryAddress = false;
                    }
                    if (address.Id != billAddressId)
                    {
                        address.IsBillingAddress = false;
                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(AddressManagement), "User", new { guid });
        }


        // POST: /User/UpdateAddresses
        [HttpPost]
        // the method can also be called using this name
        [ActionName("UpdateWholeAddress")]
        public IActionResult UpdateAddresses(Address address)
        {
            // Remove the "Customer" key from ModelState to prevent validation errors
            ModelState.Remove("Customer");

            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Check if the address is marked as the billing address
                if (address.IsBillingAddress)
                {
                    // Get other billing addresses for the same customer
                    var otherBillingAddresses = _context.Addresses.Where(a => a.CustomerId == address.CustomerId && a.IsBillingAddress && a.Id != address.Id);
                    
                    foreach (var otherAddress in otherBillingAddresses)
                    {
                        // all other billing addresses are set to false
                        otherAddress.IsBillingAddress = false;
                    }
                }

                // Check if the address is marked as the delivery address
                if (address.IsDeliveryAddress == true)
                {
                    // Get other delivery addresses for the same customer 
                    var otherDeliveryAddresses = _context.Addresses.Where(a => a.CustomerId == address.CustomerId && a.IsDeliveryAddress == true && a.Id != address.Id);
                    foreach (var otherAddress in otherDeliveryAddresses)
                    {
                        // all other delivery addresses are set to false
                        otherAddress.IsDeliveryAddress = false;
                    }
                }

                // Update the address in the database
                _context.Addresses.Update(address);
                _context.SaveChanges();

                // Get the guid of the associated customer
                var guid = _context.Customers.FirstOrDefault(c => c.Id == address.CustomerId)?.Guid;

                return RedirectToAction(nameof(AddressManagement), "User", new { guid });
            }
            else
            {
                // If the model state is not valid, redirect to the Error action in the Home controller
                return RedirectToAction("Error", "Home");
            }
        }


        // display AddressEdit View
        public IActionResult AddressEdit(int id, string guid)
        {
            // Retrieve the addresses from the database based on the provided IDs
            Address address = _context.Addresses.FirstOrDefault(a => a.Id == id);
            ViewData["guid"] = guid;
            return View(address);
        }

      
        public IActionResult AddressDelete(int id, string guid)
        {
            // Retrieve the addresses from the database based on the provided IDs
            Address address = _context.Addresses.FirstOrDefault(a => a.Id == id);

            // Find the user who owns this address
            Customer user = _context.Customers.Include(c => c.Addresses).FirstOrDefault(c => c.Guid == guid);

            // Check if the user has more than one address
            if (user.Addresses.Count > 1)
            {
                // If the user has more than one address, proceed to delete the address
                ViewData["guid"] = guid;
                return View(address);
            }
            else
            {
                // If the address is the last one, redirect the user to the AddressManagement view with an appropriate message
                TempData["LastAddressWarning"] = "Sie können Ihre letzte Adresse nicht löschen.";
                return RedirectToAction("AddressManagement", new { guid });
            }
        }


        // POST: /User/AddressDeletePost
        [HttpPost]
        public IActionResult AddressDeletePost(int id, string guid)
        {
            // Retrieve the addresses from the database based on the provided IDs
            Address address = _context.Addresses.FirstOrDefault(a => a.Id == id);
            if (address != null)
            {
                // remove the address
                _context.Addresses.Remove(address);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(AddressManagement), new { guid });
        }


        // this View indicates successful registration
        public IActionResult Success()
        {
             return View();
        }


        // This method is used to log out the user by deleting the login cookie and redirecting them to the login page.
        public IActionResult Logout()
        {
            // Get the login cookie value from the HttpContext using the CookieManager.
            string loginCookie = CookieManager.LoginCookieGuidGet(HttpContext);

            // Delete the "LoginCookie" from the response cookies.
            HttpContext.Response.Cookies.Delete("LoginCookie");

            return RedirectToAction(nameof(Login));
        }


        // This method is used to edit personal data for a user based on the provided GUID.
        public IActionResult EditPersonalData(string guid)
        {
            // Find the customer in the database using the provided GUID.
            var user = _context.Customers.FirstOrDefault(u => u.Guid == guid);

            // If the user is not found, return a NotFound HTTP response.
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // POST: /User/UpdatePersonalData
        [HttpPost]
        // This method is used to update the personal data of a customer based on the provided updatedCustomer object.
        public IActionResult UpdatePersonalData(Customer updatedCustomer)
        {
            // Find the customer in the database using the customer's ID.
            var user = _context.Customers.FirstOrDefault(u => u.Id == updatedCustomer.Id);

            // If the customer is not found, return a NotFound HTTP response.
            if (user == null)
            {
                return NotFound();
            }

            // Update the customer's properties with the values from the updatedCustomer object.
            user.FirstName = updatedCustomer.FirstName;
            user.LastName = updatedCustomer.LastName;
            user.Email = updatedCustomer.Email;
            user.Gender = updatedCustomer.Gender;
            user.Phone = updatedCustomer.Phone;

            // Update the customer data in the database
            _context.Customers.Update(user);
            _context.SaveChanges();

            return RedirectToAction("AccountManagement", new { guid = user.Guid });
        }


        // POST: /User/ChangePassword
        [HttpPost]
        // This method is used to change the password for a customer.
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmNewPassword, string guid)
        {
            // Find the customer in the database based on the provided GUID.
            var user = _context.Customers.Where(c => c.Guid == guid).FirstOrDefault();


            // If the customer is not found, redirect to the "AccountManagement" action method.
            if (user == null)
            {
                return RedirectToAction("AccountManagement", new { guid = user?.Guid });
            }

            // Hash the current password using the customer's salt and compare it with the stored hash password.
            string currentHashPass = PasswordManager.HashPassword(currentPassword, user.Salt, 100000, 16);
            if (currentHashPass != user.HashPass)
            {
                // If the current password is incorrect, add a model error and redirect to the "AccountManagement" action method.
                TempData["OldPasswordDoNotMatch"] = "Aktuelles Passwort ist nicht korrekt.";
                return RedirectToAction("AccountManagement", new { guid = user.Guid });
            }

            // Check if the new password and the confirmed new password match.
            if (newPassword != confirmNewPassword)
            {
                // If the new password and confirmed new password do not match, add a model error and redirect to the "AccountManagement" action method.
                TempData["PasswordDoNotMatch"] = "Neues Passwort und Passwort-Bestätigung stimmen nicht überein.";
                return RedirectToAction("AccountManagement", new { guid = user.Guid });
            }

            // Generate a new salt for the customer.
            string newSalt = PasswordManager.GenerateSalt(16);

            // Hash the new password using the new salt.
            string newHashPass = PasswordManager.HashPassword(newPassword, newSalt, 100000, 16);

            // Update the customer's salt and hash password with the new values.
            user.Salt = newSalt;
            user.HashPass = newHashPass;

            _context.SaveChanges();

            // Set a success message to be displayed on the next page.
            TempData["SuccessMessage"] = "Passwort wurde erfolgreich geändert.";

            return RedirectToAction("AccountManagement", new { guid = user.Guid });
        }
    }
}

