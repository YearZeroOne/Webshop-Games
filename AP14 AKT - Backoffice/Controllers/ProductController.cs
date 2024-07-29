using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopLib.Models;
using WebShopLib.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Linq;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AP14_AKT___Backoffice.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string _conStr;
        public List<Product> productList;

        /// <summary>
        /// Constructor for ProductController class. Initializes the ApplicationDbContext and IConfiguration objects and updates the product list. 
        /// </summary>
        /// <returns>
        /// Nothing
        /// </returns>
        public ProductController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _conStr = config.GetConnectionString("ConnectionString");
            productList = new List<Product>();
            UpdateProductList();
        }

        private void UpdateProductList()
        {
            productList = _context.Products.Include(x => x.Genre).Include(x => x.Developer).ToList();
        }

        /// <summary>
        /// This method filters the list of products based on the filter parameters passed in.
        /// </summary>
        /// <param name="filter">The type of filter to be applied.</param>
        /// <param name="developer">The developer to filter by.</param>
        /// <param name="genre">The genre to filter by.</param>
        /// <param name="stock">The stock to filter by.</param>
        /// <param name="status">The status to filter by.</param>
        /// <param name="type">The type to filter by.</param>
        /// <returns>A filtered list of products.</returns>
        public IActionResult Index(int? filter, string? developer, int? genre, string? stock, bool? status, string? type)
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);
            List<Product> filteredList = new List<Product>();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }

            #region filter
            // Filter Developer
            if (filter == 1)
            {
                foreach (var product in productList)
                {
                    var productDeveloper = product.Developer;
                    if (productDeveloper.Name == developer)
                    {
                        filteredList.Add(product);
                    }
                }
                return View(filteredList);
            }

            // Filter Genre
            if (filter == 2)
            {
                foreach (var product in productList)
                {
                    var productGenre = product.Genre.Id;
                    if (productGenre == genre)
                    {
                        filteredList.Add(product);
                    }
                }
                return View(filteredList);
            }

            // Filter Stock
            if (filter == 3)
            {
                foreach (var product in productList)
                {
                    var productStock = product.Stock;
                    if (stock == "notavailable")
                    {
                        if (productStock == 0)
                        {
                            filteredList.Add(product);
                        }
                    }

                    if (stock == "low")
                    {
                        if (productStock <= 10)
                        {
                            filteredList.Add(product);
                        }
                    }

                    if (stock == "available")
                    {
                        if (productStock > 10)
                        {
                            filteredList.Add(product);
                        }
                    }
                }
                return View(filteredList);
            }

            // Filter Active/Inactive
            if (filter == 4)
            {
                foreach (var product in productList)
                {
                    var productStatus = product.IsActive;
                    if (productStatus == status)
                    {
                        filteredList.Add(product);
                    }
                }
                return View(filteredList);
            }

            // Filter Physical/Digital
            if (filter == 5)
            {
                foreach (var product in productList)
                {
                    var productCategory = _context.ProductCategories.Where(x => x.Id == product.ProductCategoryId).FirstOrDefault();

                    if (productCategory.Name.ToLower() == type.ToLower())
                    {
                        filteredList.Add(product);
                    }
                }
                return View(filteredList);
            }

            #endregion filter



            return View(productList);
        }

        /// <summary>
        /// Filters products based on the given parameters.
        /// </summary>
        /// <param name="developer">The developer of the product.</param>
        /// <param name="categories">The category of the product.</param>
        /// <param name="stock">The stock of the product.</param>
        /// <param name="status">The status of the product.</param>
        /// <param name="type">The type of the product.</param>
        /// <returns>A view with the filtered products.</returns>
        public IActionResult Filter(string[]? developer, string[]? categories, int? stock, bool status, string[]? type)
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);

            var filteredProducts = _context.Products.Include(p => p.ProductCategory).Include(p => p.Genre).Include(p => p.Developer).AsQueryable();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }


            if (developer != null && developer.Length > 0)
            {
                filteredProducts = filteredProducts.Where(p => developer.Contains(p.Developer.Name.ToLower()));
                //return View("Index", filteredList);
            }

            if (categories != null && categories.Length > 0)
            {
                filteredProducts = filteredProducts.Where(p => categories.Contains(p.Genre.Name.ToLower()));
                //return View("Index", filteredList);
            }

            if (stock != null)
            {
                foreach (var product in filteredProducts)
                {
                    if (stock == 1)
                    {
                        if (product.Stock == 0)
                        {
                            filteredProducts = filteredProducts.Where(p => p.Stock == 0);
                        }
                    }

                    if (stock == 2)
                    {
                        if (product.Stock <= 10 && product.Stock > 0)
                        {
                            filteredProducts = filteredProducts.Where(p => p.Stock <= 10 && p.Stock > 0);
                        }
                    }

                    if (stock == 3)
                    {
                        if (product.Stock > 10)
                        {
                            filteredProducts = filteredProducts.Where(p => p.Stock > 10);
                        }
                    }
                }
            }

            if (status == true)
            {
                filteredProducts = filteredProducts.Where(p => p.IsActive == true);
            }

            else
            {
                filteredProducts = filteredProducts.Where(p => p.IsActive == false);
            }

            if (type != null && type.Length >= 0)
            {
                filteredProducts = filteredProducts.Where(p => type.Contains(p.ProductCategory.Name.ToLower()));
            }

            return View("Index", filteredProducts.ToList());
        }

        public IActionResult NewProductGenre()
        {
            return View();
        }

        /// <summary>
        /// Creates a new product genre and adds it to the database.
        /// </summary>
        /// <param name="name">The name of the new product genre.</param>
        /// <param name="active">Whether the new product genre is active.</param>
        /// <returns>Redirects to the GenreList action.</returns>
        public IActionResult NewProductGenreProc(string name, bool active)
        {
            active = true;

            Genre genre = new()
            {
                Name = name,
                IsActive = active
            };

            var allGenre = _context.Genres.ToList();

            foreach (var item in allGenre)
            {
                if (item.Name.ToLower().Equals(name.ToLower()))
                {
                    TempData["GenreFlag"] = "Dieses Genre existiert bereits";
                    return RedirectToAction(nameof(NewProductGenre));
                }
            }

            _context.Genres.Add(genre);
            _context.SaveChanges();

            return RedirectToAction(nameof(GenreList));
        }

        public IActionResult NewProduct()
        {
            return View();
        }

        [HttpPost]
        /// <summary>
        /// Creates a new product and adds it to the database.
        /// </summary>
        /// <returns>Redirects to the Index page.</returns>
        public IActionResult NewProductProc(string name, string developer, string productCategory, decimal netUnitPrice, int stock, bool isActive, int genre, string description)
        {
            //using var ms = new MemoryStream();
            //Resize the image to a maximum width of 500 pixels (to save space in the database)
            //var image = SixLabors.ImageSharp.Image.Load(imageFile.OpenReadStream());
            //image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(500, 0), Mode = ResizeMode.Max }));
            //image.Save(ms, new JpegEncoder());

            //WebShopLib.Models.Image newImage = new WebShopLib.Models.Image()
            //{
            //	Name = imageName,
            //	Picture = ms.ToArray()
            //};
            //_context.Images.Add(newImage);
            //_context.SaveChanges();

            int productCategoryId = _context.ProductCategories.Where(x => x.Name.ToLower() == productCategory.ToLower()).FirstOrDefault().Id;

            var dev = _context.Developers.Where(x => x.Name == developer).FirstOrDefault();

            var guid = HttpContext.Request.Cookies["guid"].ToString();
            var currentUser = _context.Employees.Where(x => x.Guid == guid).FirstOrDefault();

            Product product = new Product()
            {
                Name = name,
                Developer = dev,
                NetUnitPrice = netUnitPrice,
                Stock = stock,
                IsActive = isActive,
                GenreId = genre,
                Description = description,
                ProductCategoryId = productCategoryId,
            };


            product.ProductNumber = NumbersystemManager.NumberSystem(product);

            _context.Products.Add(product);
            _context.SaveChanges();

            PriceHistory priceHistory = new PriceHistory()
            {
                ProductId = product.Id,
                Price = product.NetUnitPrice,
                SetOn = DateTime.Now,
                SetBy = currentUser.Id
            };

            _context.PriceHistories.Add(priceHistory);
            _context.SaveChanges();

            TempData["success"] = "added new Product";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves a product from the database based on the given id and returns it to the view.
        /// </summary>
        /// <param name="id">The id of the product to be retrieved.</param>
        /// <returns>The view containing the retrieved product.</returns>
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var product = productList.Where(x => x.Id == id).FirstOrDefault();

            return View(product);
        }

        [HttpPost]
        /// <summary>
        /// Edits the product information and stores the changes in the database.
        /// </summary>
        /// <param name="productEdited">The edited product.</param>
        /// <param name="productPicture">The product picture.</param>
        /// <param name="isPrimary">Whether the product picture is the primary picture.</param>
        /// <returns>Redirects to the Index page.</returns>
        public IActionResult EditProc(bool isPrimary, Product productEdited, IFormFile productPicture)
        {
            int productOriginalIndex = productList.FindIndex(x => x.Id == productEdited.Id);
            Product productOriginal = productList.Where(x => x.Id == productEdited.Id).FirstOrDefault();
            if (productOriginalIndex >= 0)
            {
                if (productPicture != null && productPicture.Length > 0)
                {
                    using var ms = new MemoryStream();
                    // Resize the image to a maximum width of 500 pixels (to save space in the database)
                    var image = SixLabors.ImageSharp.Image.Load(productPicture.OpenReadStream());
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(500, 0), Mode = ResizeMode.Max }));
                    image.Save(ms, new JpegEncoder());

                    var imageToSave = new WebShopLib.Models.Image()
                    {
                        Name = productPicture.FileName,
                        Picture = ms.ToArray(),
                        ProductId = productEdited.Id,
                        IsPrimaryPic = isPrimary
                    };


                    // Store the JPEG data in the database
                    _context.Images.Add(imageToSave);
                }

                productEdited.NetUnitPrice = productOriginal.NetUnitPrice;

                productList[productOriginalIndex] = productEdited;
                _context.Entry(productOriginal).CurrentValues.SetValues(productEdited);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Searches for a product by its product number or name/description.
        /// </summary>
        /// <param name="search">The product number or name/description to search for.</param>
        /// <returns>The Edit view for the product if found, or the Index view with the search results.</returns>
        public IActionResult Search(string search)
        {
            if (search == null)
            {
                return View(nameof(Index), productList);
            }

            var productNumber = _context.Products.Where(x => x.ProductNumber == search).FirstOrDefault();
            if (productNumber != null)
            {
                return RedirectToAction("Edit", productNumber);
            }

            else
            {
                var productName = _context.Products.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                var productDesc = _context.Products.Where(x => x.Description.ToLower().Contains(search.ToLower())).ToList();

                productName.AddRange(productDesc.Except(productName));

                return View("Index", productName);
            }

        }

        /// <summary>
        /// Retrieves a list of developers and their associated products from the database.
        /// </summary>
        /// <returns>A view containing the list of developers and their associated products.</returns>
        public IActionResult DeveloperList()
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);
            List<Developer> developerList = new List<Developer>();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }

            developerList = _context.Developers.Include(p => p.Products).ToList();

            return View(developerList);
        }

        /// <summary>
        /// Retrieves a list of genres and their associated products from the database.
        /// </summary>
        /// <returns>A view containing the list of genres and their associated products.</returns>
        public IActionResult GenreList()
        {
            var vaild = CookieManager.GUIDLogin(HttpContext, _context);
            List<Genre> genreList = new List<Genre>();

            if (!vaild)
            {
                TempData["LoginFlag"] = "You have no Permission for this";
                return RedirectToAction("Index", "Home");
            }

            genreList = _context.Genres.Include(p => p.Products).ToList();

            return View(genreList);
        }

        public IActionResult NewProductDeveloper()
        {
            return View();
        }

        [HttpPost]
        /// <summary>
        /// Creates a new developer and adds it to the database.
        /// </summary>
        /// <param name="name">The name of the new developer.</param>
        /// <param name="active">Whether the new developer is active.</param>
        /// <returns>Redirects to the DeveloperList page.</returns>
        public IActionResult NewProductDeveloperProc(string name, bool active)
        {
            active = true;

            Developer developer = new()
            {
                Name = name,
                IsActive = active
            };

            var allDevelopers = _context.Developers.ToList();

            foreach (var item in allDevelopers)
            {
                if (item.Name.ToLower().Equals(name.ToLower()))
                {
                    TempData["DeveloperFlag"] = "This new developer already exists";
                    return RedirectToAction(nameof(NewProductDeveloper));
                }
            }

            _context.Developers.Add(developer);
            _context.SaveChanges();

            return RedirectToAction(nameof(DeveloperList));
        }

        public IActionResult ChangePrice(int productId)
        {
            ViewBag.ProductId = productId;

            return View();
        }

        /// <summary>
        /// This method is used to change the price of a product with the given productId and newPrice.
        /// </summary>
        /// <param name="productId">The id of the product to be changed.</param>
        /// <param name="newPrice">The new price of the product.</param>
        /// <returns>A view to confirm the change.</returns>
        public IActionResult ChangePricePassword(int productId, decimal newPrice)
        {
            ViewBag.ProductId = productId;
            ViewData["NewPrice"] = newPrice;

            return View();
        }


        /// <summary>
        /// Changes the price of a product and adds a record to the PriceHistory table.
        /// </summary>
        /// <param name="userId">The ID of the user making the change.</param>
        /// <param name="productId">The ID of the product to be changed.</param>
        /// <param name="newPrice">The new price of the product.</param>
        /// <param name="password">The password of the user making the change.</param>
        /// <returns>Redirects to the Edit action.</returns>
        [HttpPost]
        public IActionResult ChangePriceProc(int userId, int productId, decimal newPrice, string password)
        {
            var user = _context.Employees.Find(userId);

            bool valid = PasswordManager.VerifyPassword(password, user.HashPassword, user.Salt);

            if (valid)
            {
                var product = _context.Products.Find(productId);

                product.NetUnitPrice = newPrice;

                PriceHistory priceHistory = new PriceHistory()
                {
                    ProductId = productId,
                    Price = newPrice,
                    SetOn = DateTime.Now,
                    SetBy = userId
                };

                _context.PriceHistories.Add(priceHistory);
                _context.SaveChanges();

            }

            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        /// <summary>
        /// Checks if the user has the required role to change the status of a developer and redirects to the PasswordRequiredStatusDev view if they do.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="itemId">The id of the item.</param>
        /// <param name="status">The status of the developer.</param>
        /// <returns>
        /// Redirects to the PasswordRequiredStatusDev view if the user has the required role, otherwise redirects to the DeveloperList view.
        /// </returns>
        public IActionResult ChangeStatusDev(int userId, int itemId, string status)
        {
            var user = _context.Employees.Find(userId);

            if (user.Role < 3 && status.Contains("Active") || user.Role < 3 && status.Contains("Inactive"))
            {
                ViewBag.ItemId = itemId;

                return View("PasswordRequiredStatusDev");
            }

            else
            {
                return RedirectToAction(nameof(DeveloperList));
            }
        }

        /// <summary>
        /// Checks if the user has the required role to change the status of an item and redirects to the PasswordRequiredStatusGen view if they do.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="itemId">The id of the item.</param>
        /// <param name="status">The status of the item.</param>
        /// <returns>
        /// Redirects to the PasswordRequiredStatusGen view if the user has the required role, otherwise redirects to the GenreList view.
        public IActionResult ChangeStatusGen(int userId, int itemId, string status)
        {
            var user = _context.Employees.Find(userId);

            if (user.Role < 3 && status.Contains("Active") || user.Role < 3 && status.Contains("Inactive"))
            {
                ViewBag.ItemId = itemId;

                return View("PasswordRequiredStatusGen");
            }

            else
            {
                return RedirectToAction(nameof(GenreList));
            }
        }



        /// <summary>
        /// Changes the status of a developer process based on the userId, itemId, and password provided.
        /// </summary>
        /// <param name="userId">The userId of the employee.</param>
        /// <param name="itemId">The itemId of the developer.</param>
        /// <param name="password">The password of the employee.</param>
        /// <returns>Redirects to the DeveloperList page.</returns>
        public IActionResult ChangeStatusDevProc(int userId, int itemId, string password)
        {
            var user = _context.Employees.Find(userId);

            bool valid = PasswordManager.VerifyPassword(password, user.HashPassword, user.Salt);

            if (valid == true)
            {
                var dev = _context.Developers.Find(itemId);

                if (dev.IsActive == true)
                {
                    dev.IsActive = false;
                }
                else
                {
                    dev.IsActive = true;
                }
                _context.SaveChanges();

                return RedirectToAction(nameof(DeveloperList));
            }
            else
            {
                return RedirectToAction(nameof(DeveloperList));
            }

        }

        /// <summary>
        /// Changes the status of a genre based on the userId, itemId, and password provided.
        /// </summary>
        /// <param name="userId">The userId of the user making the change.</param>
        /// <param name="itemId">The itemId of the genre to be changed.</param>
        /// <param name="password">The password of the user making the change.</param>
        /// <returns>Redirects to the GenreList page.</returns>
        public IActionResult ChangeStatusGenProc(int userId, int itemId, string password)
        {
            var user = _context.Employees.Find(userId);

            bool valid = PasswordManager.VerifyPassword(password, user.HashPassword, user.Salt);

            if (valid == true)
            {
                var gen = _context.Genres.Find(itemId);

                if (gen.IsActive == true)
                {
                    gen.IsActive = false;
                }
                else
                {
                    gen.IsActive = true;
                }
                _context.SaveChanges();

                return RedirectToAction(nameof(GenreList));
            }
            else
            {
                return RedirectToAction(nameof(GenreList));
            }

        }

        /// <summary>
        /// Checks if the user is an admin and if the developer has no products associated with them, then redirects to the PasswordRequiredDeleteDev view. Otherwise, redirects to the DeveloperList view.
        /// </summary>
        /// <param name="itemId">The id of the developer to be deleted</param>
        /// <param name="userId">The id of the user attempting to delete the developer</param>
        /// <returns>The PasswordRequiredDeleteDev view or the DeveloperList view</returns>
        public IActionResult DeleteDev(int itemId, int userId)
        {
            var user = _context.Employees.Find(userId);
            var dev = _context.Developers.Where(p => p.Products.Count == 0 && itemId == p.Id);

            if (user.Role == 1 && dev.Count() == 1)
            {
                ViewBag.ItemId = itemId;

                return View("PasswordRequiredDeleteDev");
            }

            else
            {
                return RedirectToAction(nameof(DeveloperList));
            }

        }

        /// <summary>
        /// Deletes a developer from the database based on the userId, itemId, and password provided.
        /// </summary>
        /// <returns>
        /// Redirects to the DeveloperList page.
        /// </returns>
        public IActionResult DeleteDevProc(int userId, int itemId, string password)
        {
            var user = _context.Employees.Find(userId);

            bool valid = PasswordManager.VerifyPassword(password, user.HashPassword, user.Salt);

            if (valid == true)
            {
                var dev = _context.Developers.Find(itemId);

                _context.Remove(dev);

                _context.SaveChanges();

                return RedirectToAction(nameof(DeveloperList));
            }
            else
            {
                return RedirectToAction(nameof(DeveloperList));
            }
        }

        /// <summary>
        /// Checks if the user is an admin and if the genre has no products associated with it, then redirects to the PasswordRequiredDeleteGen view. Otherwise, redirects to the GenreList view.
        /// </summary>
        /// <param name="itemId">The id of the genre to be deleted</param>
        /// <param name="userId">The id of the user</param>
        /// <returns>The PasswordRequiredDeleteGen view or the GenreList view</returns>
        public IActionResult DeleteGen(int itemId, int userId)
        {
            var user = _context.Employees.Find(userId);
            var dev = _context.Genres.Where(p => p.Products.Count == 0 && itemId == p.Id);

            if (user.Role == 1 && dev.Count() == 1)
            {
                ViewBag.ItemId = itemId;

                return View("PasswordRequiredDeleteGen");
            }

            else
            {
                return RedirectToAction(nameof(GenreList));
            }

        }

        /// <summary>
        /// Deletes a genre from the database based on the userId, itemId, and password provided.
        /// </summary>
        /// <param name="userId">The userId of the user attempting to delete the genre.</param>
        /// <param name="itemId">The itemId of the genre to be deleted.</param>
        /// <param name="password">The password of the user attempting to delete the genre.</param>
        /// <returns>Redirects to the GenreList page.</returns>
        public IActionResult DeleteGenProc(int userId, int itemId, string password)
        {
            var user = _context.Employees.Find(userId);

            bool valid = PasswordManager.VerifyPassword(password, user.HashPassword, user.Salt);

            if (valid == true)
            {
                var gen = _context.Genres.Find(itemId);

                _context.Remove(gen);

                _context.SaveChanges();

                return RedirectToAction(nameof(GenreList));
            }
            else
            {
                return RedirectToAction(nameof(GenreList));
            }
        }

        /// <summary>
        /// Checks if the user is an admin and if the product exists, then checks if the product has been ordered or reordered. If not, it will redirect to the PasswordRequiredDeleteProd view. Otherwise, it will redirect to the Index view.
        /// </summary>
        /// <returns>RedirectToAction</returns>
        public IActionResult DeleteProd(int userId, int productId)
        {
            var user = _context.Employees.Find(userId);
            var prod = _context.Products.Where(p => p.Id == productId);

            if (user.Role == 1 && prod.Count() == 1)
            {
                var order = _context.Products.Include(p => p.Deliveries).Include(p => p.OrderProducts).Where(p => p.Id == productId).FirstOrDefault();
                bool notReOrder = order.Deliveries.Count() == 0;
                bool notOrder = order.OrderProducts.Count() == 0;

                if (notReOrder && notOrder)
                {
                    ViewBag.ItemId = productId;

                    return View("PasswordRequiredDeleteProd");
                }

                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            else
            {
                return RedirectToAction(nameof(Index));
            }

        }

        /// <summary>
        /// Deletes a product from the database, given a userId, itemId, and password.
        /// </summary>
        /// <returns>
        /// Redirects to the Index page.
        /// </returns>
        public IActionResult DeleteProdProc(int userId, int itemId, string password)
        {
            var user = _context.Employees.Find(userId);

            bool valid = PasswordManager.VerifyPassword(password, user.HashPassword, user.Salt);

            if (valid == true)
            {
                var prod = _context.Products.Include(p => p.PriceHistories).Where(p => p.Id == itemId).FirstOrDefault();
                var img = _context.Images.Where(p => p.ProductId == prod.Id).FirstOrDefault();

                if (img != null)
                {
                    _context.Remove(img);
                }

                _context.Remove(prod);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }


    }
}