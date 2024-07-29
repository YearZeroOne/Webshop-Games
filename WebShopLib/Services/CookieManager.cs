using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using WebShopLib.Models;

namespace WebShopLib.Services
{
    public static class CookieManager
    {

        /// <summary>
        /// Sets a cookie with a new Guid and returns the Guid string.
        /// </summary>
        /// <param name="httpContext">The HttpContext of the request.</param>
        /// <returns>The Guid string.</returns>
        public static string CookieGuidSet(HttpContext httpContext)
        {
            var guidStr = Guid.NewGuid().ToString();

            CookieOptions co = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(400)
            };

            httpContext.Response.Cookies.Append("guid", guidStr, co);

            return guidStr;
        }


        /// <summary>
        /// Checks if a cookie with the name "guid" exists in the HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext to check.</param>
        /// <returns>True if the cookie exists, false otherwise.</returns>
        public static bool CookieGuidExists(HttpContext httpContext)
        {
            bool cookieExists = false;

            string guidFromCookie = httpContext.Request.Cookies["guid"];

            if (guidFromCookie != null)
            {
                cookieExists = true;
            }
            return cookieExists;
        }

        /// <summary>
        /// Gets the GUID from the cookie.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns>The GUID from the cookie.</returns>
        public static string CookieGuidGet(HttpContext httpContext, string cookie)
        {


            string guidFromCookie = httpContext.Request.Cookies[cookie];

            if (guidFromCookie != null)
            {
                return guidFromCookie;
            }
            return "";

        }


        /// <summary>
        /// Sets a cookie in the response with a unique login string.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The unique login string.</returns>
        public static string LoginCookieSet(HttpContext httpContext)
        {
            var loginStr = Guid.NewGuid().ToString();


            httpContext.Response.Cookies.Append("LoginCookie", loginStr);

            return loginStr;
        }


        /// <summary>
        /// Checks if the "LoginCookie" cookie is set in the request.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>True if the cookie is set, false otherwise.</returns>
        public static bool IsLoginCookieSet(HttpContext httpContext)
        {
            if (httpContext.Request.Cookies.TryGetValue("LoginCookie", out string cookieValue))
            {
                // The "LoginCookie" cookie exists and its value is not null or empty
                return !string.IsNullOrEmpty(cookieValue);
            }

            // The "LoginCookie" cookie does not exist
            return false;
        }


        /// <summary>
        /// Gets the GUID from the LoginCookie.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The GUID from the LoginCookie.</returns>
        public static string LoginCookieGuidGet(HttpContext httpContext)
        {
            string guidFromLoginCookie = httpContext.Request.Cookies["LoginCookie"];

            if (guidFromLoginCookie != null)
            {
                return guidFromLoginCookie;
            }
            return "";
        }


        /// <summary>
        /// Checks if the user is logged in using a GUID cookie.
        /// </summary>
        /// <param name="httpcontext">The HTTP context.</param>
        /// <param name="_context">The application database context.</param>
        /// <returns>True if the user is logged in, false otherwise.</returns>
        public static bool GUIDLogin(HttpContext httpcontext, ApplicationDbContext _context)
        {
            string browserGuid = CookieManager.CookieGuidGet(httpcontext, "guid");

            if (browserGuid == "")
            {
                return false;
            }

            var userGuid = _context.Employees.Where(x => x.Guid == browserGuid).FirstOrDefault();

            if (userGuid == null)
            {
                return false;
            }

            if (browserGuid.Equals(userGuid.Guid))
            {
                return true;
            }
            return false;
        }


    }
}
