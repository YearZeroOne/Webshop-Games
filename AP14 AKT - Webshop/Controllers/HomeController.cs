using AP14_AKT___Webshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System.Diagnostics;
using WebShopLib.Models;
using WebShopLib.Services;

namespace AP14_AKT___Webshop.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        private string _conStr;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/PublicIndex
        public IActionResult PublicIndex()
        {
            return View();
        }

        // GET: /Home/Imprint
        public IActionResult Imprint()
        {
            return View();
        }

        // GET: /Home/TermsAndCondition
        public IActionResult TermsAndCondition()
        {
            return View();
        }

        // GET: /Home/PrivacyNotice
        public IActionResult PrivacyNotice()
        {
            return View();
        }

        // GET: /Home/CookiesNotice
        public IActionResult CookiesNotice()
        {
            return View();
        }

        // GET: /Home/ForgotPasswordCustomer
        public IActionResult ForgotPasswordCustomer()
        {
            return View();
        }

        // POST: /Home/ForgotPasswordCustomerProc
        [HttpPost]
        public IActionResult ForgotPasswordCustomerProc(string email)
        {
            // Retrieve the user based on the email
            var user = _context.Customers.Where(x => x.Email == email).FirstOrDefault();

            // Check if the user exists
            if (user == null)
            {
                return RedirectToAction("WrongEmailAddress", "Home");
            }
            else
            {
                string emaillink = UrlManager.ResetPasswordLinkCustomer(user.Email);
                string emailold = user.Email;

                // Generate a new GUID for password reset

                string guidnew = Guid.NewGuid().ToString();

                emaillink += "&guid=" + guidnew;



                string linktext = "";
                string subject = "Passwort zurücksetzen";
                // Construct the password reset email body
                string body = $@"Sehr geehrter Kunde!

        Sie haben für unser Unternehmen 'Game Hub' ein neues Passwort angefordert.

        Sollten Sie kein 'Passwort zurücksetzen' beantragt haben, könnte ein Missbrauch vorliegen. 
        Kontaktieren Sie in diesem Fall bitte dringend unseren Support.  

        Um ein neues Passwort für unser Unternehmen zu setzen, klicken Sie bitte folgenden Link:
        {emaillink} {linktext}

        Bitte beachten Sie, dass dieser Link nur 24 Stunden gültig ist. Nach Ablauf der Frist müssen Sie erneut die Passwortzurücksetzung beantragen.  

        Nach dem Zurücksetzen können Sie sich mit Ihrem neuen Passwort anmelden.

        Mit freundlichen Grüßen

        Ihr 'Game Hub' Team";

                // Send the password reset email
                EmailManager.SendEmail(email, subject, body);

                // Create a new PasswordReset object and save it to the database
                var passwordReset = new PasswordReset()
                {
                    Guid = guidnew,
                    CustomerId = user.Id,
                    TimeCreator = DateTime.Now
                };
                _context.PasswordResets.Add(passwordReset);
                _context.SaveChanges();
                
            }
            return RedirectToAction(nameof(ForgotPasswordConfirmation)); 
        }

        // GET: /Home/WrongEmailAddress
        public IActionResult WrongEmailAddress()
        {
            TempData["Message"] = "Die eingegebene E-Mail-Adresse ist nicht vorhanden";

            return View();
        }

        // GET: /Home/ForgotPasswordConfirmation
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Home/ResetPassword
        public IActionResult ResetPassword(string email, string guid)     
        {
            var timeStamp = (DateTime)_context.PasswordResets.Where(x => x.Guid == guid).FirstOrDefault().TimeCreator;

            DateTime now = DateTime.Now;

            if (now < timeStamp.AddHours(24))
            {
                var guidold = _context.PasswordResets.Where(x => x.Guid == guid).FirstOrDefault();

                ViewBag.email = email;

                return View(guidold);
            }
            else
            {
                return RedirectToAction(nameof(LinkExpired));
            }
        }

        public IActionResult LinkExpired()
        {
            TempData["Message"] = "Der Link ist leider abgelaufen. Sie müssen die Passwortzurücksetzung erneut ausführen.";

            return View();
        }

        // POST: /Home/ResetPasswordCustomer
        [HttpPost]
        public IActionResult ResetPasswordCustomer(string email, string guid, string password)  
        {
            // Retrieve the user based on the email
            var user = _context.Customers.Where(x => x.Email == email).FirstOrDefault();

            // Generate a new salt and hash the password
            string salt = PasswordManager.GenerateSalt(16);
            string hashPass = PasswordManager.HashPassword(password, salt, 100000, 16);

            // Update the user's password and save changes to the database
            user.HashPass = hashPass;
            user.Salt = salt;
            _context.SaveChanges();

            // Remove the password reset entry from the database
            var dataSetToRemove = _context.PasswordResets.Where(x => x.CustomerId == user.Id).FirstOrDefault();

            if (dataSetToRemove != null)
            {
                _context.PasswordResets.Remove(dataSetToRemove);
            }

            _context.SaveChanges();

            return View();
        }

        // POST: /Home/LoginProc
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult LoginProc(string email, string password)
        {
            var user = _context.Customers.Where(x => x.Email == email).FirstOrDefault();

            if (user != null)
            {
                var userGuid = CookieManager.CookieGuidGet(HttpContext, "guid");

                if (userGuid.Equals(user.Guid, StringComparison.OrdinalIgnoreCase))
                {
                    RedirectToAction("Hub");
                }
                else if (userGuid != user.Guid)
                {
                    user.Guid = CookieManager.CookieGuidSet(HttpContext);
                    CookieManager.LoginCookieSet(HttpContext);
                    using SqlConnection connection = new SqlConnection(_conStr);
                    connection.Open();
                    string updateSql = "UPDATE Customers SET Guid = '" + user.Guid + "' WHERE Id = " + user.Id;
                    SqlCommand command = new SqlCommand(updateSql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                var passwordTrue = PasswordManager.VerifyPassword(
                    password,
        user.HashPass,
        user.Salt);

                var customers = _context.Customers.Where(x => x.Email == email).FirstOrDefault();




            }

            // ON RedirectToAction always Use TempData
            TempData["LoginFlag"] = "Invalid Login";
            return RedirectToAction(nameof(Index));
        }




        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }




    }
}