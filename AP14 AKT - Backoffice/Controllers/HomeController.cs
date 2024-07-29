using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using AP14_AKT___Backoffice.Models;

using iText.Layout.Element;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;

using WebShopLib.Models;
using WebShopLib.Services;

namespace AP14_AKT___Backoffice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _conStr;



        public HomeController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _conStr = config.GetConnectionString("ConnectionString");
        }

        // GET: Home
        /// <summary>
        /// Checks if the user has a valid cookie and redirects them to the Hub page if they do.
        /// </summary>
        /// <returns>View or RedirectToAction</returns>
        public IActionResult Index()
        {
            //var userGuid = CookieManager.CookieGuidGet(HttpContext, "guid");

            //if (userGuid != null)
            //{
            //    if (_context.Employees.Any(x => x.Guid == userGuid))
            //    {
            //        return RedirectToAction("Hub");
            //    }
            //}

            return View();
        }

        /// <summary>
        /// Logs out the user by deleting the cookie and redirecting to the Index page.
        /// </summary>
        /// <returns>Redirects to the Index page.</returns>
        public IActionResult Logout()
        {

            if (Request.Cookies["guid"] != null)
            {
                HttpContext.Response.Cookies.Delete("guid");
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// This method is used to register a new user.
        /// </summary>
        /// <returns>Returns a view for the user to register.</returns>
        public IActionResult Register()
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        /// <summary>
        /// RegisterProc method is used to register a new employee in the database.
        /// </summary>
        /// <param name="firstname">The firstname of the employee.</param>
        /// <param name="lastname">The lastname of the employee.</param>
        /// <param name="bussinesMail">The business email of the employee.</param>
        /// <param name="privatMail">The 
        [HttpPost]
        public IActionResult RegisterProc(string firstname, string lastname, string bussinesMail, string privatMail, string privatePhone, string role, string password)
        {
            foreach (var userEmailInDb in _context.Employees)
            {
                if (userEmailInDb.BusinessEmail == bussinesMail)
                {
                    TempData["EmailFlag"] = "Email already taken";
                    return RedirectToAction(nameof(Register));
                }
            }

            var salt = PasswordManager.GenerateSalt(16);

            PasswordManager.SaveNewEmployeeToDb(firstname, lastname, bussinesMail, privatMail, privatePhone, int.Parse(role), password, salt);

            return RedirectToAction(nameof(Index));
        }



        /// <summary>
        /// This method is used to authenticate a user and redirect them to the Hub page if successful. If unsuccessful, the user will be redirected to the Index page and the number of unsuccessful login attempts will be tracked. If the number of unsuccessful login attempts exceeds 10, the user's account will be locked.
        /// </summary>
        /// <returns>Redirects to either the Hub page or the Index page.</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult LoginProc(string email, string password)
        {
            var user = _context.Employees.Where(x => x.BusinessEmail == email).FirstOrDefault();

            if (user != null)
            {
                var userGuid = CookieManager.CookieGuidGet(HttpContext, "guid");

                if (userGuid.Equals(user.Guid, StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Hub");
                }
                else if (userGuid != user.Guid)
                {
                    user.Guid = CookieManager.CookieGuidSet(HttpContext);
                    CookieManager.LoginCookieSet(HttpContext);
                    using SqlConnection connection = new(_conStr);
                    connection.Open();
                    string updateSql = "UPDATE Employees SET Guid = '" + user.Guid + "' WHERE Id = " + user.Id;
                    SqlCommand command = new(updateSql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                var passwordTrue = PasswordManager.VerifyPassword(
                    password,
                    user.HashPassword,
                    user.Salt);

                var employee = _context.Employees.Where(x => x.BusinessEmail == email).FirstOrDefault();

                if (passwordTrue)
                {
                    if (employee.IsLocked == true)
                    {
                        TempData["LoginFlag"] = "Your Account is locked";
                        return RedirectToAction(nameof(Index));
                    }
                    if (employee.IsActive == false)
                    {
                        TempData["LoginFlag"] = "Your Account is Inactive";
                        return RedirectToAction(nameof(Index));
                    }

                    CookieManager.LoginCookieSet(HttpContext);
                    employee.UnsuccessfullLogin = 0;
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Hub));
                }
                else
                {



                    if (!passwordTrue)
                    {
                        if (employee.UnsuccessfullLogin == null)
                        {
                            employee.UnsuccessfullLogin = 1;
                            _context.SaveChanges();
                        }
                        else
                        {
                            employee.UnsuccessfullLogin += 1;
                            _context.SaveChanges();
                        }
                    }

                    var unsuccessfullLoginNum = employee.UnsuccessfullLogin;

                    if (employee.UnsuccessfullLogin < 10)
                    {

                        TempData["LoginFlag"] = "Login attempts left " +
                            "" + (10 - unsuccessfullLoginNum);
                    }

                    if (employee.UnsuccessfullLogin >= 10)
                    {
                        employee.IsLocked = true;
                        _context.SaveChanges();
                    }


                }


            }

            // ON RedirectToAction always Use TempData

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Handles the forgot password request by sending an email with a reset link to the user.
        /// </summary>
        /// <param name="forgotPasswordModel">The model containing the user's email address.</param>
        /// <returns>The view for the confirmation page or the same view with the model if the model is invalid.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(forgotPasswordModel);

            var user = _context.Employees.Where(x => x.PrivateEmail == forgotPasswordModel.Email);
            if (user != null)
            {


                string emaillink = UrlManager.ResetPasswordLink(forgotPasswordModel.Email);
                string email = forgotPasswordModel.Email;
                var guidToken = Guid.NewGuid().ToString();



                emaillink += "&token=" + guidToken;

                string linktext = "Klicken Sie hier, um den Link zu öffnen";
                string subject = "Paaswort zurücksetzen";
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

                EmailManager.SendEmail(email, subject, body);

                var passwordReset = new PasswordReset()
                {
                    Guid = guidToken,
                    EmployeeId = _context.Employees.Where(x => x.PrivateEmail.Equals(forgotPasswordModel.Email)).FirstOrDefault().Id,
                    TimeCreator = DateTime.Now
                };
                _context.PasswordResets.Add(passwordReset);
                _context.SaveChanges();


                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }



            return View(forgotPasswordModel);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        /// <summary>
        /// This method is used to reset the user's password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="token">The token used to reset the password.</param>
        /// <returns>A view with the reset password model.</returns>
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var tokeInDb = _context.PasswordResets.Where(x => x.Guid.Equals(token)).FirstOrDefault().Guid;


            if (tokeInDb.Equals(token))
            {
                var model = new ResetPasswordModel { Token = token, Email = email };
                return View(model);
            }

            return View();
        }

        /// <summary>
        /// This method is used to reset the password of a user.
        /// </summary>
        /// <returns>
        /// Redirects to ResetPasswordConfirmation action.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPasswordProc(ResetPasswordModel resetPasswordModel)
        {


            var user = _context.Employees.Where(x => x.PrivateEmail == resetPasswordModel.Email).FirstOrDefault();

            var timeStamp = (DateTime)_context.PasswordResets.Where(x => x.EmployeeId == user.Id).FirstOrDefault().TimeCreator;

            var dataSetToRemove = _context.PasswordResets.Where(x => x.EmployeeId == user.Id).FirstOrDefault();




            DateTime now = DateTime.Now;

            if (now > timeStamp.AddHours(24))
            {
                TempData["LoginFlag"] = "Email Link expired, please request a new one";
                return RedirectToAction(nameof(Index));
            }

            if (user == null)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            else
            {
                string salt = PasswordManager.GenerateSalt(16);

                string hashPass = PasswordManager.HashPassword(resetPasswordModel.Password, salt, 100000, 16);

                //var updateEmployeePass = new Employee
                //{
                //	Id = user.Id,
                //	FirstName = user.FirstName,
                //	LastName = user.LastName,
                //	BusinessEmail = user.BusinessEmail,
                //	PrivateEmail = user.PrivateEmail,
                //	Guid = null,
                //	Role = user.Role,
                //	HashPassword = hashPass,
                //	Salt = salt,
                //	IsActive = user.IsActive,
                //	IsLocked = user.IsLocked,
                //	UnsuccessfullLogin = user.UnsuccessfullLogin,
                //	UserName = user.UserName
                //};

                user.HashPassword = hashPass;
                user.Salt = salt;
                if (dataSetToRemove != null)
                {
                    _context.PasswordResets.Remove(dataSetToRemove);
                }
                _context.SaveChanges();
            }








            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }



        /// <summary>
        /// This method is used to check if the user has permission to access the Hub page. 
        /// </summary>
        /// <returns>
        /// Returns the Hub page if the user has permission, otherwise redirects to the Index page.
        /// </returns>
        public IActionResult Hub()
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction(nameof(Index));
            }

            return View(_context.Employees.ToList());
        }



        /// <summary>
        /// Redirects the user to the Index page if the cookie is not valid. Otherwise, returns the Privacy page.
        /// </summary>
        /// <returns>The Privacy page or the Index page.</returns>
        public IActionResult Privacy()
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);

            if (!vaild)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }



    }
}
