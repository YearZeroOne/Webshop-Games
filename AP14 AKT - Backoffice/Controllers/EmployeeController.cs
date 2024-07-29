using iText.Kernel.Pdf.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using WebShopLib.Models;
using WebShopLib.Services;

using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace AP14_AKT___Backoffice.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string _conStr;



        /// <summary>
        /// Constructor for EmployeeController class.
        /// </summary>
        /// <param name="context">ApplicationDbContext object.</param>
        /// <param name="config">IConfiguration object.</param>
        /// <returns>
        /// No return value.
        /// </returns>
        public EmployeeController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _conStr = config.GetConnectionString("ConnectionString");
        }

        [HttpGet]
        public IActionResult NewEmployee()
        {
            return View();
        }

        /// <summary>
        /// Creates a new employee and sends an email to the employee's 
        [HttpPost]
        public IActionResult NewEmployeeProc(TempEmployee employee)
        {
            if (_context.Employees.Where(x => x.UserName == employee.UserName).Count() > 0)
            {
                TempData["UsernameTaken"] = "UserName already in use.";

                return RedirectToAction(nameof(NewEmployee));
            }

            Guid guid = Guid.NewGuid();

            employee.Guid = guid.ToString();
            employee.TimeCreated = DateTime.Now;
            employee.BusinessEmail += "@gamehub.com";
            _context.TempEmployees.Add(employee);
            _context.SaveChanges();


            string emailLink = UrlManager.GetNewEmployeeUrl(employee.BusinessEmail, guid.ToString());

            string email = employee.PrivateEmail;

            EmailManager.SendEmail(email, "Create Account Authentification", emailLink);



            TempData["NewEmployee"] = "E-Mail was sent.";

            return RedirectToAction("Hub", "Home");
        }


        /// <summary>
        /// This method sets the guid value in the TempData and returns the Register view.
        /// </summary>
        /// <param name="guid">The guid value to be set in the TempData.</param>
        /// <returns>The Register view.</returns>
        public IActionResult Register(string guid)
        {
            TempData["guid"] = guid.ToString();

            return View();
        }

        /// <summary>
        /// Registers a new employee in the system.
        /// </summary>
        /// <param name="guid">The unique identifier of the employee.</param>
        /// <param name="password">The password of the employee.</param>
        /// <returns>Redirects to the home page.</returns>
        [HttpPost]
        public IActionResult RegisterProc(string guid, string password)
        {
            var tempEmployee = _context.TempEmployees.Where(x => x.Guid == guid).FirstOrDefault();

            Employee employee = new Employee();

            var salt = PasswordManager.GenerateSalt(16);
            var hashpass = PasswordManager.HashPassword(password, salt, 100000, 16);

            if (tempEmployee != null)
            {
                employee.FirstName = tempEmployee.FirstName;
                employee.LastName = tempEmployee.LastName;
                employee.UserName = tempEmployee.UserName;
                employee.BusinessEmail = tempEmployee.BusinessEmail;
                employee.PrivateEmail = tempEmployee.PrivateEmail;
                employee.PrivatePhone = tempEmployee.PrivatePhone;
                employee.HashPassword = hashpass;
                employee.Salt = salt;
                employee.Role = tempEmployee.Role;
                employee.IsActive = true;
                employee.IsLocked = false;
                employee.UnsuccessfullLogin = 0;
            }

            _context.Employees.Add(employee);
            _context.TempEmployees.Remove(tempEmployee);
            _context.SaveChanges();

            var creator = _context.Employees.Find(tempEmployee.CreatorId);

            string infoCSV = "";

            infoCSV += tempEmployee.FirstName + ";";
            infoCSV += tempEmployee.LastName + ";";
            infoCSV += tempEmployee.UserName + ";";
            infoCSV += tempEmployee.BusinessEmail + ";";
            infoCSV += tempEmployee.PrivateEmail + ";";
            infoCSV += tempEmployee.PrivatePhone + ";";
            infoCSV += tempEmployee.CreatorId + ";";
            infoCSV += tempEmployee.Role;

            EmployeeChange changes = new()
            {
                ExecutingUser = creator.Id,
                ExecutingUserRole = creator.Role,
                Action = (int)Models.Action.CreatedAccount,
                ActionInformation = infoCSV,
                AffectedUser = employee.Id,
                TimeOfChange = DateTime.Now
            };

            _context.EmployeeChanges.Add(changes);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Retrieves the details of an employee with the given id.
        /// </summary>
        /// <param name="id">The id of the employee to retrieve.</param>
        /// <returns>The view containing the details of the employee.</returns>
        public IActionResult Details(int id)
        {
            var model = _context.Employees.Where(x => x.Id == id).FirstOrDefault();

            return View(model);
        }


        /// <summary>
        /// This method is used to edit the email of an employee.
        /// </summary>
        /// <param name="userId">The user id of the employee.</param>
        /// <param name="id">The id of the employee.</param>
        /// <param name="employeeEmail">The new email of the employee.</param>
        /// <returns>Returns the view for editing.</returns>
        public IActionResult EditEmail(int userId, int id, string employeeEmail)
        {

            var model = _context.Employees.Where(x => x.Id == id).FirstOrDefault();
            if (model != null)
            {
                string emaillink = UrlManager.ChangePrivateEmailLink(employeeEmail);

                string email = employeeEmail;

                emaillink += "&userId=" + userId.ToString() + "&id=" + id.ToString();

                EmailManager.SendEmail(email, "Confirm Mail change", emaillink);

            }

            return View("Edit");
        }

        /// <summary>
        /// This method is used to edit the role of an employee.
        /// </summary>
        /// <param name="userId">The ID of the user making the change.</param>
        /// <param name="employeeId">The ID of the employee whose role is being changed.</param>
        /// <param name="role">The new role of the employee.</param>
        /// <returns>Redirects to the home page.</returns>
        [HttpPost]
        public IActionResult EditRole(int userId, int employeeId, int role)
        {
            var model = _context.Employees.Find(employeeId);
            if (model != null && role == 1)
            {
                return View("PasswordRequired", model);
            }
            else if (model != null)
            {
                model.Role = role;

                string newRole;

                switch (role)
                {
                    case 1:
                        newRole = "Admin";
                        break;
                    case 2:
                        newRole = "Manager";
                        break;
                    default:
                        newRole = "Employee";
                        break;
                }

                var user = _context.Employees.Find(userId);

                EmployeeChange changes = new EmployeeChange();
                changes.AffectedUser = model.Id;
                changes.ExecutingUser = user.Id;
                changes.ExecutingUserRole = user.Role;
                changes.Action = (int)Models.Action.RoleUpdated;
                changes.ActionInformation = newRole;
                changes.TimeOfChange = DateTime.Now;

                _context.EmployeeChanges.Add(changes);
                _context.SaveChanges();
            }

            return RedirectToAction("Hub", "Home");
        }

        /// <summary>
        /// This method is used to edit the role of an employee. It takes in the userId, employeeId, and password of the admin. It verifies the password and if it is valid, it updates the role of the employee and adds a record to the EmployeeChanges table. 
        /// </summary>
        /// <returns>
        /// Returns a redirect to the Hub page if the role is successfully changed, or a redirect to the Details page if the role is not changed. 
        /// </returns>
        [HttpPost]
        public IActionResult EditRoleProc(int userId, int employeeId, string password)
        {
            var admin = _context.Employees.Find(userId);
            var employee = _context.Employees.Find(employeeId);

            if (employee != null && admin != null)
            {
                bool validPassWord = PasswordManager.VerifyPassword(password, admin.HashPassword, admin.Salt);

                if (validPassWord)
                {
                    employee.Role = 1;

                    EmployeeChange changes = new EmployeeChange();
                    changes.AffectedUser = employeeId;
                    changes.ExecutingUser = admin.Id;
                    changes.ExecutingUserRole = admin.Role;
                    changes.Action = (int)Models.Action.RoleUpdated;
                    changes.ActionInformation = "Admin";
                    changes.TimeOfChange = DateTime.Now;

                    _context.EmployeeChanges.Add(changes);
                    _context.SaveChanges();

                    TempData["roleChanged"] = "Role has been successfully changed.";

                    return RedirectToAction("Hub", "Home");
                }
            }
            TempData["roleChanged"] = "Role was not changed";

            return RedirectToAction(nameof(Details), employeeId);
        }

        /// <summary>
        /// Updates the 
        public IActionResult EditProc(string email, int userId, int id)
        {
            var user = _context.Employees.Find(userId);
            var model = _context.Employees.Where(x => x.Id.Equals(id)).FirstOrDefault();

            if (model != null)
            {
                model.PrivateEmail = email;

                EmployeeChange changes = new()
                {
                    ExecutingUser = user.Id,
                    AffectedUser = id,
                    ExecutingUserRole = user.Role,
                    Action = (int)Models.Action.PrivatePhoneNumberUpdated,
                    ActionInformation = email,
                    TimeOfChange = DateTime.Now
                };

                _context.EmployeeChanges.Add(changes);
                _context.SaveChanges();
            }
            return RedirectToAction("Hub", "Home");
        }




 
        public IActionResult EditTelNr(int userId, int id, string telNum)
        {
            var user = _context.Employees.Find(userId);
            var model = _context.Employees.Where(x => x.Id.Equals(id)).FirstOrDefault();

            if (model != null)
            {
                model.PrivatePhone = telNum;

                EmployeeChange changes = new()
                {
                    ExecutingUser = user.Id,
                    AffectedUser = id,
                    ExecutingUserRole = user.Role,
                    Action = (int)Models.Action.PrivatePhoneNumberUpdated,
                    ActionInformation = telNum,
                    TimeOfChange = DateTime.Now
                };

                _context.EmployeeChanges.Add(changes);
                _context.SaveChanges();
            }
            return RedirectToAction("Hub", "Home");
        }


        /// <summary>
        /// Updates the business email of an employee and logs the change in the EmployeeChanges table.
        /// </summary>
        /// <param name="userId">The ID of the user making the change.</param>
        /// <param name="id">The ID of the employee whose business email is being changed.</param>
        /// <param name="bussinesMail">The new business email.</param>
        /// <returns>Redirects to the Hub page.</returns>
        public IActionResult EditBussinesMail(int userId, int id, string bussinesMail)
        {
            var user = _context.Employees.Find(userId);
            var model = _context.Employees.Where(x => x.Id.Equals(id)).FirstOrDefault();

            if (model != null && user != null)
            {
                model.BusinessEmail = bussinesMail;

                EmployeeChange changes = new()
                {
                    ExecutingUser = user.Id,
                    AffectedUser = id,
                    ExecutingUserRole = user.Role,
                    Action = (int)Models.Action.BusinessEmailUpdated,
                    ActionInformation = bussinesMail,
                    TimeOfChange = DateTime.Now
                };

                _context.EmployeeChanges.Add(changes);
                _context.SaveChanges();

            }
            return RedirectToAction("Hub", "Home");
        }

        /// <summary>
        /// Updates the username of an employee and adds a record of the change to the EmployeeChanges table.
        /// </summary>
        /// <param name="userId">The ID of the user making the change.</param>
        /// <param name="id">The ID of the employee whose username is being changed.</param>
        /// <param name="userName">The new username.</param>
        /// <returns>Redirects to the Hub page.</returns>
        public IActionResult EditUserName(int userId, int id, string userName)
        {
            var user = _context.Employees.Find(userId);
            var model = _context.Employees.Where(x => x.Id.Equals(id)).FirstOrDefault();

            if (model != null && user != null)
            {
                model.UserName = userName;

                EmployeeChange changes = new()
                {
                    ExecutingUser = user.Id,
                    AffectedUser = id,
                    ExecutingUserRole = user.Role,
                    Action = (int)Models.Action.NameUpdated,
                    ActionInformation = userName,
                    TimeOfChange = DateTime.Now
                };

                _context.EmployeeChanges.Add(changes);
                _context.SaveChanges();
            }

            return RedirectToAction("Hub", "Home");
        }


        /// <summary>
        /// Changes the active status of an employee based on the user's role.
        /// </summary>
        /// <param name="userId">The ID of the user making the change.</param>
        /// <param name="id">The ID of the employee to be changed.</param>
        /// <returns>Redirects to the Hub page.</returns>
        public IActionResult ChangeActive(int userId, int id)
        {
            var currentUser = _context.Employees.Find(userId);
            var employee = _context.Employees.Find(id);

            var adminCheck = _context.Employees.Where(x => x.Role == 1 && x.IsActive == true).ToList();

            if (employee == null)
            {
                return RedirectToAction("Hub", "Home");
            }

            EmployeeChange changes = new()
            {
                ExecutingUser = userId,
                ExecutingUserRole = currentUser.Role,
                AffectedUser = employee.Id,
                TimeOfChange = DateTime.Now
            };

            if (currentUser.Role == 2 && employee.Role == 3)
            {
                if (employee.IsActive)
                {
                    employee.IsActive = false;
                    changes.Action = (int)Models.Action.DeactivatedAccount;
                }
                else
                {
                    employee.IsActive = true;
                    changes.Action = (int)Models.Action.ActivatedAccount;
                }
            }

            if (adminCheck.Count() == 1)
            {
                TempData["AdminFlag"] = "There must always be an admin";
                return RedirectToAction("Hub", "Home");

            }

            if (currentUser.Id == id)
            {
                TempData["AdminFlag"] = "You cannot lock yourself";
                return RedirectToAction("Hub", "Home");
            }


            if (employee.IsActive)
            {
                employee.IsActive = false;
                changes.Action = (int)Models.Action.DeactivatedAccount;
            }
            else
            {
                employee.IsActive = true;
                changes.Action = (int)Models.Action.ActivatedAccount;
            }

            _context.EmployeeChanges.Add(changes);
            _context.SaveChanges();

            return RedirectToAction("Hub", "Home");
        }

        /// <summary>
        /// Changes the locked status of an employee account.
        /// </summary>
        /// <returns>
        /// Redirects to the Hub page.
        /// </returns>
        public IActionResult ChangeLocked(int id)
        {
            string guid = HttpContext.Request.Cookies["guid"];
            var currentUser = _context.Employees.Where(x => x.Guid == guid).FirstOrDefault();
            var employee = _context.Employees.Find(id);

            if (currentUser != null && employee != null && employee == currentUser && !employee.IsLocked)
            {
                return RedirectToAction("Hub", "Home");
            }

            var adminCheck = _context.Employees.Where(x => x.Role == 1).Where(x => x.IsLocked == false).ToList();

            if (adminCheck.Count() == 1)
            {
                TempData["AdminFlag"] = "There must always be an admin";
                return RedirectToAction("Index", "Home");

            }

            if (currentUser.Id == id)
            {
                TempData["AdminFlag"] = "You cannot lock yourself";
                return RedirectToAction("Hub", "Home");
            }

            if (employee == null || currentUser == null)
            {
                return RedirectToAction("Hub", "Home");
            }

            EmployeeChange changes = new()
            {
                ExecutingUser = currentUser.Id,
                ExecutingUserRole = currentUser.Role,
                AffectedUser = employee.Id,
                TimeOfChange = DateTime.Now
            };

            if (employee.IsLocked)
            {
                employee.IsLocked = false;
                changes.Action = (int)Models.Action.UnlockedAccount;
            }
            else
            {
                employee.IsLocked = true;
                changes.Action = (int)Models.Action.LockedAccount;
            }

            _context.EmployeeChanges.Add(changes);
            _context.SaveChanges();

            return RedirectToAction("Hub", "Home");
        }





    }

}