using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using WebShopLib.Models;

namespace WebShopLib.Services
{
    public static class PasswordManager
    {
        /// <summary>
        /// Generates a random salt of the specified length.
        /// </summary>
        /// <param name="nSalt">The length of the salt to generate.</param>
        /// <returns>A random salt of the specified length.</returns>
        public static string GenerateSalt(int nSalt)
        {
            var saltBytes = new byte[nSalt];

            using (var provider = RandomNumberGenerator.Create())
            {
                provider.GetNonZeroBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Generates a hash for the given password using the given salt and number of iterations and hash size.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt to use for the hash.</param>
        /// <param name="nIterations">The number of iterations to use for the hash.</param>
        /// <param name="nHash">The size of the hash.</param>
        /// <returns>
        /// A base64 encoded string representing the hash of the given password.
        /// </returns>
        public static string HashPassword(string password, string salt, int nIterations, int nHash)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, nIterations);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(nHash));
        }

        /// <summary>
        /// Verifies a given password against a hashed password and salt.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="hashed_password">The hashed password to compare against.</param>
        /// <param name="salt">The salt used to hash the password.</param>
        /// <returns>True if the passwords match, false otherwise.</returns>
        public static bool VerifyPassword(string password, string hashed_password, string salt)
        {
            string newHashed = HashPassword(password, salt, 100000, 16);
            return newHashed == hashed_password;
        }


        /// <summary>
        /// Saves a new customer to the database with the given parameters.
        /// </summary>
        /// <param name="firstname">The customer's first name.</param>
        /// <param name="lastname">The customer's last name.</param>
        /// <param name="email">The customer's email address.</param>
        /// <param name="phone">The customer's phone number.</param>
        /// <param name="gender">The customer's gender.</param>
        /// <param name="password">The customer's password.</param>
        /// <param name="addressline">The customer's address line.</param>
        /// <param name="country">The customer's country.</param>
        /// <param name="city">The customer's city.</param>
        /// <param name="zip">The customer's zip code.</param>
        /// <param name="salt">The customer's salt.</param>
        public static void SaveNewCustomerToDb(string firstname, string lastname, string email, string phone, int gender, string password, string addressline, string country, string city, int zip, string salt)
        {
            var hashedPassword = HashPassword(password, salt.ToString(), 100000, 16);
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");


            using (var db = new ApplicationDbContext())
            {
                // Create a new customer
                Customer customer = new Customer()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    Email = email,
                    Phone = phone,
                    HashPass = hashedPassword,
                    Gender = gender,
                    Salt = salt,
                    IsActive = true,
                    RegistrationDate = DateTime.Now


                };
                customer.CustomerNumber = NumbersystemManager.NumberSystem(customer);
                db.Customers.Add(customer);
                db.SaveChanges();


                // Create a new address for the customer
                Address address = new Address()
                {
                    LastName = firstname,
                    FirstName = firstname,
                    AddressLine = addressline,
                    Country = country,
                    City = city,
                    Zipcode = zip,
                    CustomerId = customer.Id // Join customer and address
                };

                // Add the customer and address to the database

                db.Addresses.Add(address);
                db.SaveChanges();
            }
        }




        /// <summary>
        /// Saves a new employee to the database with the given parameters.
        /// </summary>
        /// <param name="firstname">The first name of the employee.</param>
        /// <param name="lastname">The last name of the employee.</param>
        /// <param name="bussinesMail">The business email of the employee.</param>
        /// <param name="privatMail">The 
        public static void SaveNewEmployeeToDb(string firstname, string lastname, string bussinesMail, string privatMail, string privatePhone, int role, string password, string salt)
        {
            var hashedPassword = HashPassword(password, salt.ToString(), 100000, 16);

            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");


            using (var db = new ApplicationDbContext())
            {
                Employee employee = new Employee()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    BusinessEmail = bussinesMail,
                    PrivateEmail = privatMail,
                    PrivatePhone = privatePhone,
                    Role = role,
                    HashPassword = hashedPassword,
                    Salt = salt
                };

                db.Employees.Add(employee);
                db.SaveChanges();

            }
        }

    }
}
