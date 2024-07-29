using AP14_AKT___Webshop.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using MimeKit;
using WebShopLib.Models;
using MailKit.Net.Smtp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace AP14_AKT___Webshop.Controllers
{
    public class OrderController : Controller
    {

        // DbContext used for database operations
        readonly ApplicationDbContext _context;

        // Inject the dbContext in the controller's constructor
        public OrderController(ApplicationDbContext context)
        {
            _context = context;

        }

        // Helper method to create OrderViewModel from Order entity
        private OrderViewModel? GetOrderOrderViewModel(int orderId)
        {

            var order = _context.Orders.Include(o => o.Customer)
                                  .Include(o => o.PaymentMethod)
                                  .Include(o => o.OrderProducts)
                                  .ThenInclude(op => op.Products)
                                  .ThenInclude(p => p.ProductCategory)
                                  .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            // Fill the view model with data
            OrderViewModel orderViewModel = new()
            {
                Order = order,
                OrderAddress = _context.Addresses.FirstOrDefault(a => a.CustomerId == order.CustomerId && a.IsDeliveryAddress == true),
                Store = _context.Stores.FirstOrDefault(),
                OrderAmount = order.OrderProducts.Sum(op => op.Quantity * op.Products.NetUnitPrice * (1 + op.Products.ProductCategory.TaxRate / 100)),
            };

            return orderViewModel;
        }

        // Helper method to retrieve all addresses related to a specific customer
        private ICollection<Address> GetCustomerAddresses(int customerId) //I don't know how to get all the customer's addresses without using a separate method to get the addresses.
        {

            var customer = _context.Customers.Include(c => c.Addresses)
                                            .FirstOrDefault(c => c.Id == customerId);
            if (customer != null)
            {
                return customer.Addresses;
            }
            else
            {
                return (ICollection<Address>)NotFound();
            }

        }

        // Default method that renders the index view
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ThankYouPage(int orderId, decimal shippingCost, string email)
        {
            // Fetch the order from the database
            var order = _context.Orders.Include(x => x.OrderProducts).ThenInclude(x => x.Products).ThenInclude(x => x.ProductCategory).Where(x => x.Id == orderId).FirstOrDefault();

            // Format the shipping cost
            string formattedShippingCost = shippingCost.ToString("0.00");

            // Set formattedShippingCost to the ViewBag
            ViewBag.ShippingCost = formattedShippingCost;

            // Create a new mail message
            var message = new MimeMessage();

            // Set up the From and To addresses
            message.From.Add(MailboxAddress.Parse("ap14webshop@outlook.com"));
            message.To.Add(MailboxAddress.Parse(email));

            // Set the subject of the email
            message.Subject = "Rechnung";


            // Initialize the body builder for the email content
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<h2>Vielen Dank für Ihre Bestellung!</h2>" +
                                   "<p><strong>Order ID:</strong> " + order.OrderNumber + "</p>" +
                                   "<p><strong>Datum:</strong> " + order.OrderDate + "</p>" +
                                   "<p><strong>Versandkosten:</strong> " + formattedShippingCost + "</p>" +
                                   "<h3>Bestellte Produkte:</h3>" +
                                   "<table>" +
                                   "<tr>" +
                                   "<th>Produktname</th>" +
                                   "<th>Preis</th>" +
                                   "<th>Menge</th>" +
                                   "<th>Gesamt</th>" +
                                   "</tr>";

            // the total price of all the ordered products
            decimal totalPrice = 0;

            // Loop through each product in the order
            foreach (var orderProduct in order.OrderProducts)
            {
                // Calculate the total price of all the ordered products
                decimal productTotal = orderProduct.Price * (1 + orderProduct.Products.ProductCategory.TaxRate / 100) * orderProduct.Quantity;
                totalPrice += productTotal;

                // Append each product's information to the HTML body
                bodyBuilder.HtmlBody += "<tr>" +
                                        "<td>" + orderProduct.Products.Name + "</td>" +
                                        "<td>" + (orderProduct.Price * (1+orderProduct.Products.ProductCategory.TaxRate / 100)).ToString("0.00")+ "</td>" +
                                        "<td>" + orderProduct.Quantity + "</td>" +
                                        "<td>" + productTotal.ToString("0.00") + "</td>" +
                                        "</tr>";
            }
            // Append the total price to the HTML body
            bodyBuilder.HtmlBody += "</table>" +
                                    "<p><strong>Gesamtpreis:</strong> " + ((totalPrice + shippingCost).ToString("0.00")) + "</p>";
            bodyBuilder.HtmlBody += "_____________________________";

            // Append bank transfer and address information to the HTML body
            bodyBuilder.HtmlBody += "<p>Daten für Überweisung</p>" +
                                    "<p>GameHub GmbH</p>" +
                                    "<p>IBAN: AT1241251289512958</p>";
            bodyBuilder.HtmlBody += "_____________________________";


            bodyBuilder.HtmlBody += "<p>Anschrift für Abholung</p>" +
                                    "<p>GameHub GmbH</p>" +
                                    "<p>Musterstraße 123</p>" +
                                    "<p>12345 Musterstadt</p>";
            bodyBuilder.HtmlBody += "_____________________________";

            // Check if payment is pending or not
            if (order.PaymentPending == false)
            {

                bodyBuilder.HtmlBody += "<p>Status: Offen </p>";
            }
            else
            {
                bodyBuilder.HtmlBody += "<p>Status: Abgeschlossen </p>";

            }

            // Check the payment method
            bodyBuilder.HtmlBody += "_____________________________";
            if (order.PaymentMethodId == 1)
            {
                bodyBuilder.HtmlBody += "<p>Zahlungsart: Kreditkarte</p>";

                }
            else if (order.PaymentMethodId == 2)
            {
                bodyBuilder.HtmlBody += "<p>Zahlungsart: Überweisung</p>";
            }
            else
            {
                bodyBuilder.HtmlBody += "<p>Zahlungsart: Bargeld </p>";
            }
            bodyBuilder.HtmlBody += "_____________________________";

            // Check if the order is for pickup or delivery
            if (order.PickUp == false)
            {
                bodyBuilder.HtmlBody += "<p>Versandart: Versand</p>";
                }

            else
            {
                bodyBuilder.HtmlBody += "<p>Versandart: Abholung</p>";
             }

            // Set the body of the email
            message.Body = bodyBuilder.ToMessageBody();

            // Convert the HTML body to a PDF file
            byte[] pdfBytes;
            using (var memoryStream = new MemoryStream())
            {
                var document = new Document();
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                var htmlWorker = new HTMLWorker(document);
                htmlWorker.Parse(new StringReader(bodyBuilder.HtmlBody));

                document.Close();

                pdfBytes = memoryStream.ToArray();
            }

            // Attach the PDF file to the email message
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(new MemoryStream(pdfBytes)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "order_invoice.pdf"
            };

            // Create a multipart message to include both the attachment and the regular email body
            var multipart = new Multipart("mixed");

            // Add a text part with the custom message
            var textPart = new TextPart("plain")
            {
                Text = "Vielen Dank für Ihre Bestellung! Sie finden als Anhang Ihre Rechnung!"
            };
            multipart.Add(textPart);

            multipart.Add(attachment);

            // Assign the multipart message as the body of the email
            message.Body = multipart;

            // Send the email with the PDF attachment
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("ap14webshop@outlook.com", "Akt14webshop#");
                client.Send(message);
                client.Disconnect(true);
            }
            return View(order);
        }


        // Method that retrieves the order details and renders the OrderOverview view
        public IActionResult OrderOverview(int orderId)
        {
            var orderViewModel = GetOrderOrderViewModel(orderId);
            if (orderViewModel == null || orderViewModel.Order == null || orderViewModel.Order.Customer == null)
            {
                return NotFound();
            }

            // Generate the OrderOverviewViewModel object
            var orderOverviewModel = new OrderOverviewViewModel
            {
                Order = orderViewModel.Order,
                OrderAddress = orderViewModel.OrderAddress,
                Store = orderViewModel.Store,
                OrderAmount = orderViewModel.OrderAmount,
                Addresses = GetCustomerAddresses(orderViewModel.Order.CustomerId),
                PhisicalProductsAmount = orderViewModel.Order.OrderProducts
                    .Where(op => op.Products.ProductCategory.Id == 11)
                    .Sum(op => op.Quantity * op.Products.NetUnitPrice * (1 + op.Products.ProductCategory.TaxRate / 100))
            };

            return View(orderOverviewModel);
        }

        // Method to add a new address for a customer
        [HttpPost]
        public IActionResult AddAddress(int orderId, int customerId, string addressLine, string city, int zipcode, string country, string lastName, string firstName)

        {
            // Create the new Address
            Address newAddress = new()
            {
                LastName = lastName,
                FirstName = firstName,
                CustomerId = customerId,
                AddressLine = addressLine,
                City = city,
                Zipcode = zipcode,
                Country = country,
                IsBillingAddress = false,
                IsDeliveryAddress = false
            };

            // Save the new address in the database and return to the OrderOverview view
            if (ModelState.IsValid)
            {
                _context.Addresses.Add(newAddress);
                _context.SaveChanges();

                return RedirectToAction(nameof(OrderOverview), "Order", new { orderId });
            }

            return RedirectToAction(nameof(OrderOverview), "Order", new { orderId });
        }


        // Method that retrieves the order details and renders the OrderDetails view
        public IActionResult OrderDetails(int orderId)
        {

            var orderViewModel = GetOrderOrderViewModel(orderId);
            if (orderViewModel == null || orderViewModel.Order == null || orderViewModel.Order.Customer == null)
            {
                return NotFound();
            }

            // Generate the OrderOverviewViewModel object
            var orderOverviewModel = new OrderOverviewViewModel
            {
                Order = orderViewModel.Order,
                OrderAddress = orderViewModel.OrderAddress,
                Store = orderViewModel.Store,
                OrderAmount = orderViewModel.OrderAmount,
                Addresses = GetCustomerAddresses(orderViewModel.Order.CustomerId),
                PhisicalProductsAmount = orderViewModel.Order.OrderProducts
                    .Where(op => op.Products.ProductCategory.Id == 11)
                    .Sum(op => op.Quantity * op.Products.NetUnitPrice * (1 + op.Products.ProductCategory.TaxRate / 100))
            };
            return View(orderOverviewModel);
        }
    }
}
