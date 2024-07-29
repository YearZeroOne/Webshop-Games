using Azure.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopLib.Models;

namespace WebShopLib.Services
{
    public static class UrlManager
    {
        /// <summary>
        /// generates an Url to be sent to employee's E-Mail for registering    
        /// </summary>
        /// <param name="employeeEmail"></param>
        /// <returns>returns Link for registering employees</returns>
        public static string GetNewEmployeeUrl(string employeeEmail, string guid)
        {
            string url = "https://localhost:7131/";

            //TODO
            string path = "Employee/Register?guid=";

            path += guid.ToString();

            string toSendString = url + path;

            return toSendString;
        }


        /// <summary>
        /// Generates a link to reset the password for the given employee email.
        /// </summary>
        /// <param name="employeeEmail">The email of the employee.</param>
        /// <returns>The link to reset the password.</returns>
        public static string ResetPasswordLink(string employeeEmail)
        {
            Uri uri = new Uri("https://localhost:7131/");

            //TODO
            string path = "/Home/ResetPassword?email=";

            path += employeeEmail;

            Uri toSendUri = new Uri(uri, path);

            return toSendUri.ToString();
        }


        /// <summary>
        /// Generates a URL for a new customer registration process.
        /// </summary>
        /// <param name="customerEmail">The customer's email address.</param>
        /// <param name="guid">A unique identifier.</param>
        /// <returns>A URL for the new customer registration process.</returns>
        public static string GetNewCustomerUrl(string customerEmail, string guid)
        {
            string url = "https://localhost:7131/";

            //TODO
            string path = "/Home/RegisterProc/";

            path += guid.ToString();

            string toSendString = url + path;

            return toSendString;
        }


        /// <summary>
        /// Generates a reset password link for a customer with the given email address.
        /// </summary>
        /// <param name="customerEmail">The email address of the customer.</param>
        /// <returns>The reset password link.</returns>
        public static string ResetPasswordLinkCustomer(string customerEmail)
        {
            Uri uri = new Uri("https://localhost:7131/");

            string path = "/Home/ResetPassword?email=";

            path += customerEmail;

            Uri toSendUri = new Uri(uri, path);

            return toSendUri.ToString();
        }

        /// <summary>
        /// Creates a link to the employee's 
        public static string ChangePrivateEmailLink(string employeesEmail)
        {
            Uri uri = new Uri("https://localhost:7131/");

            string path = "/Employee/EditProc?email=";
            path += employeesEmail;

            Uri toSendUri = new Uri(uri, path);

            return toSendUri.ToString();
        }
    }
}
