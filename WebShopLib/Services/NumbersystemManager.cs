using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopLib.Models;

namespace WebShopLib.Services
{
    public static class NumbersystemManager
    {
        /// <summary>
        /// Generates a unique order number based on the order date and the next available number.
        /// </summary>
        /// <param name="order">The order to generate the number for.</param>
        /// <returns>A unique order number.</returns>
        public static string NumberSystem(Order order)
        {
            string year = GetOrderYear(order.OrderDate);
            string month = GetOrderMonth(order.OrderDate);
            string monthletter = GiveMonthLetters(month);
            string number = GetNextNumber(order);

            return ($"B{monthletter}-{year}-{number}");
        }

        /// <summary>
        /// Generates a string in the format of "N[MonthLetter]-[Year]-[Number]" based on the OrderDate and the next available number.
        /// </summary>
        /// <param name="reOrder">The ReOrder object containing the OrderDate.</param>
        /// <returns>
        /// A string in the format of "N[MonthLetter]-[Year]-[Number]".
        /// </returns>
        public static string NumberSystem(ReOrder reOrder)
        {
            string year = GetOrderYear(reOrder.OrderDate);
            string month = GetOrderMonth(reOrder.OrderDate);
            string monthletter = GiveMonthLetters(month);
            string number = GetNextNumber(reOrder);

            return ($"N{monthletter}-{year}-{number}");
        }

        /// <summary>
        /// Generates a unique product number based on the current date and product type.
        /// </summary>
        /// <param name="product">The product type.</param>
        /// <returns>A unique product number.</returns>
        public static string NumberSystem(Product product)
        {
            string year = GetOrderYear(DateTime.Now);
            string month = GetOrderMonth(DateTime.Now);
            string monthletter = GiveMonthLetters(month);
            string number = GetNextNumber(product);

            return ($"P{monthletter}-{year}-{number}");
        }

        /// <summary>
        /// Generates a customer order number in the format K[Month Letter]-[Year]-[Number]
        /// </summary>
        /// <param name="customer">The customer for whom the order number is generated</param>
        /// <returns>A string containing the customer order number</returns>
        public static string NumberSystem(Customer customer)
        {
            string year = GetOrderYear(DateTime.Now);
            string month = GetOrderMonth(DateTime.Now);
            string monthletter = GiveMonthLetters(month);
            string number = GetNextNumber(customer);

            return ($"K{monthletter}-{year}-{number}");
        }
        /// <summary>
        /// Generates a unique order number based on the current date and delivery type.
        /// </summary>
        /// <param name="delivery">The delivery type.</param>
        /// <returns>A unique order number.</returns>

        public static string NumberSystem(Delivery delivery)
        {
            string year = GetOrderYear(DateTime.Now);
            string month = GetOrderMonth(DateTime.Now);
            string monthletter = GiveMonthLetters(month);
            string number = GetNextNumber(delivery);

            return ($"L{monthletter}-{year}-{number}");
        }

        public static string GiveMonthLetters(string month)
        {
            switch (month)
            {
                case "1":
                    return "J";

                case "2":
                    return "F";

                case "3":
                    return "M";

                case "4":
                    return "A";

                case "5":
                    return "Y";

                case "6":
                    return "N";

                case "7":
                    return "L";

                case "8":
                    return "T";

                case "9":
                    return "S";

                case "10":
                    return "0";

                case "11":
                    return "R";

                case "12":
                    return "D";
                default:
                    break;
            }

            return month;
        }

        /// <summary>
        /// Gets the last two digits of the year from a given DateTime object. 
        /// </summary>
        /// <param name="yearOrder">The DateTime object to get the year from.</param>
        /// <returns>A string containing the last two digits of the year.</returns>
        public static string GetOrderYear(DateTime yearOrder)
        {
            int year = yearOrder.Year;
            return year.ToString().Remove(0, 2);
        }

        /// <summary>
        /// Gets the month of a given DateTime object.
        /// </summary>
        /// <param name="monthOrder">The DateTime object to get the month from.</param>
        /// <returns>A string representation of the month.</returns>
        public static string GetOrderMonth(DateTime monthOrder)
        {
            int month = monthOrder.Month;
            return month.ToString();
        }

        /// <summary>
        /// Generates the next order number based on the current order numbers in the database.
        /// </summary>
        /// <param name="order">The current order.</param>
        /// <returns>The next order number.</returns>
        internal static string GetNextNumber(Order order)
        {
            using (var db = new ApplicationDbContext())
            {
                var numbersAsString = db.Orders.Select(x => x.OrderNumber.Substring(6, 6)).ToArray();

                List<double> numbersAsInt = new List<double>();

                foreach (var item in numbersAsString)
                {
                    numbersAsInt.Add(double.Parse(item));
                }

                double newNumber = numbersAsInt.Max() + 1;

                if (newNumber / 100000 >= 1)
                {
                    return newNumber.ToString();
                }
                else if (newNumber / 10000 >= 1)
                {
                    string resultString = "0";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else if (newNumber / 1000 >= 1)
                {
                    string resultString = "00";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else if (newNumber / 100 >= 1)
                {
                    string resultString = "000";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else if (newNumber / 10 >= 1)
                {
                    string resultString = "0000";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else
                {
                    string resultString = "00000";

                    resultString += newNumber.ToString();

                    return resultString;
                }

            }
        }
        /// <summary>
        /// Generates the next number for a ReOrder object
        /// </summary>
        /// <param name="reOrder">The ReOrder object</param>
        /// <returns>The next number for the ReOrder object</returns>
        internal static string GetNextNumber(ReOrder reOrder)
        {
            using (var db = new ApplicationDbContext())
            {
                var numbersAsString = db.ReOrders.Select(x => x.ReOrderNumber.Substring(6, 4)).ToArray();

                List<double> numbersAsInt = new List<double>();

                foreach (var item in numbersAsString)
                {
                    numbersAsInt.Add(double.Parse(item));
                }

                double newNumber;

                if (numbersAsInt.Count() != null)
                {
                    newNumber = numbersAsInt.Max() + 1;
                }
                else
                {
                    newNumber = 1;
                }

                if (newNumber / 1000 >= 1)
                {
                    return newNumber.ToString();
                }
                else if (newNumber / 100 >= 1)
                {
                    string resultString = "0";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else if (newNumber / 10 >= 1)
                {
                    string resultString = "00";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else
                {
                    string resultString = "000";

                    resultString += newNumber.ToString();

                    return resultString;
                }
            }
        }

        /// <summary>
        /// Generates a unique delivery number based on the existing delivery numbers in the database.
        /// </summary>
        /// <returns>
        /// A unique delivery number as a string.
        /// </returns>
        internal static string GetNextNumber(Delivery delivery)
        {
            using (var db = new ApplicationDbContext())
            {
                var numbersAsString = db.Deliveries.Select(x => x.DeliveryNumber.Substring(6, 4)).ToArray();

                List<double> numbersAsInt = new List<double>();

                foreach (var item in numbersAsString)
                {
                    numbersAsInt.Add(double.Parse(item));
                }

                double newNumber;

                if (numbersAsInt.Count() != null)
                {
                    newNumber = numbersAsInt.Max() + 1;
                }
                else
                {
                    newNumber = 1;
                }


                if (newNumber / 1000 >= 1)
                {
                    return newNumber.ToString();
                }
                else if (newNumber / 100 >= 1)
                {
                    string resultString = "0";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else if (newNumber / 10 >= 1)
                {
                    string resultString = "00";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else
                {
                    string resultString = "000";

                    resultString += newNumber.ToString();

                    return resultString;
                }
            }
        }

        /// <summary>
        /// Generates a customer number based on the maximum customer number in the database.
        /// </summary>
        /// <returns>
        /// A string representing the customer number.
        /// </returns>
        internal static string GetNextNumber(Customer customer)
        {
            using (var db = new ApplicationDbContext())
            {
                var numbersAsString = db.Customers.Select(x => x.CustomerNumber.Substring(6, 4)).ToArray();

                List<double> numbersAsInt = new List<double>();

                foreach (var item in numbersAsString)
                {
                    numbersAsInt.Add(double.Parse(item));
                }

                double newNumber;

                if (numbersAsInt.Count() != null)
                {
                    newNumber = numbersAsInt.Max() + 1;
                }
                else
                {
                    newNumber = 1;
                }


                if (newNumber / 1000 >= 1)
                {
                    return newNumber.ToString();
                }
                else if (newNumber / 100 >= 1)
                {
                    string resultString = "0";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else if (newNumber / 10 >= 1)
                {
                    string resultString = "00";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else
                {
                    string resultString = "000";

                    resultString += newNumber.ToString();

                    return resultString;
                }
            }
        }

        /// <summary>
        /// Generates the next product number based on the existing product numbers in the database.
        /// </summary>
        /// <returns>
        /// A string representing the next product number.
        /// </returns>
        internal static string GetNextNumber(Product product)
        {
            using (var db = new ApplicationDbContext())
            {
                var numbersAsString = db.Products.Select(x => x.ProductNumber.Substring(6, 4)).ToArray();

                List<double> numbersAsInt = new List<double>();

                foreach (var item in numbersAsString)
                {
                    numbersAsInt.Add(double.Parse(item));
                }

                double newNumber;

                if (numbersAsInt.Count() != null)
                {
                    newNumber = numbersAsInt.Max() + 1;
                }
                else
                {
                    newNumber = 1;
                }


                if (newNumber / 1000 >= 1)
                {
                    return newNumber.ToString();
                }
                else if (newNumber / 100 >= 1)
                {
                    string resultString = "0";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else if (newNumber / 10 >= 1)
                {
                    string resultString = "00";

                    resultString += newNumber.ToString();

                    return resultString;
                }
                else
                {
                    string resultString = "000";

                    resultString += newNumber.ToString();

                    return resultString;
                }
            }
        }

    }
}
