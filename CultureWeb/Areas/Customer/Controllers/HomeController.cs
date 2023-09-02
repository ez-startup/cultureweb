using CultureWeb;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CultureWeb.Areas.Admin.Models;

using CultureWeb.Data;
using CultureWeb.Models;
using CultureWeb.Utility;
using System.Net;
using X.PagedList;
using Newtonsoft.Json.Schema;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CultureWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;          
        }


        //searching product(AllProduct)
        [HttpGet]
        public ViewResult List(string search)
        {
            var subCategories = _db.SubCategories
                              .Include(s => s.MainCategories)
                              .Where(s => s.MainCategories.Name == "Product")
                              .Select(ur => new SubCategory()
                              {
                                  Id = ur.Id,
                                  Name = ur.Name,
                                  Name_kh = ur.Name_kh
                              })
                              .ToList();

            ViewBag.SubCategories = subCategories;

            var subCategoriesBlogs = _db.SubCategories
                              .Include(s => s.MainCategories)
                              .Where(s => s.MainCategories.Name == "Blog")
                              .Select(ur => new SubCategory()
                              {
                                  Id = ur.Id,
                                  Name = ur.Name,
                                  Name_kh = ur.Name_kh
                              })
                              .ToList();

            ViewBag.SubBlogs = subCategoriesBlogs;

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favoriteProductIds = _db.FavoriteProducts
                                            .Where(fp => fp.UserId == userId)
                                            .Select(fp => fp.ProductId)
                                            .ToList();
                ViewBag.FavoriteProductIds = favoriteProductIds;

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            var model = from m in _db.Products.Include(m => m.SubCategories).Include(m => m.Reviews)
                        select m;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.Name!.Contains(search) || s.Name_kh!.Contains(search) || s.SubCategories.Name!.Contains(search));

            }

            return View("AllProduct", model);
        }

        //searching product(Product-Category)
        [HttpGet]
        public ViewResult Search(int id, string search)
        {
            var subCategories = _db.SubCategories
                             .Include(s => s.MainCategories)
                             .Where(s => s.MainCategories.Name == "Product")
                             .Select(ur => new SubCategory()
                             {
                                 Id = ur.Id,
                                 Name = ur.Name,
                                 Name_kh = ur.Name_kh
                             })
                             .ToList();

            ViewBag.SubCategories = subCategories;

            var subCategoriesBlogs = _db.SubCategories
                              .Include(s => s.MainCategories)
                              .Where(s => s.MainCategories.Name == "Blog")
                              .Select(ur => new SubCategory()
                              {
                                  Id = ur.Id,
                                  Name = ur.Name,
                                  Name_kh = ur.Name_kh
                              })
                              .ToList();

            ViewBag.SubBlogs = subCategoriesBlogs;
            // Retrieve the subcategory with the specified id and include its associated products
            var subcategory = _db.SubCategories
                .Include(s => s.Products)
                .ThenInclude(m => m.Reviews)
                .FirstOrDefault(s => s.Id == id);

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favoriteProductIds = _db.FavoriteProducts
                                            .Where(fp => fp.UserId == userId)
                                            .Select(fp => fp.ProductId)
                                            .ToList();
                ViewBag.FavoriteProductIds = favoriteProductIds;

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            // Filter the products based on the search query
            if (!string.IsNullOrEmpty(search))
            {
                subcategory.Products = subcategory.Products
                    .Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || p.Name_kh.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View("Product-Category", subcategory);
        }



        [HttpGet]
        public async Task<IActionResult> Index(string id, string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var latestBlog = _db.Blogs
                              .Include(c => c.SubCategories)
                              .Where(p => p.SubCategories.Name == id)
                              .OrderByDescending(b => b.Id) // Assuming higher Id means a more recent blog
                              .FirstOrDefault();
            ViewBag.LatestBlog = latestBlog;


            var subCategoriesBlogs = _db.SubCategories
                             .Include(s => s.MainCategories)
                             .Where(s => s.MainCategories.Name == "Blog")
                             .Select(ur => new SubCategory()
                             {
                                 Id = ur.Id,
                                 Name = ur.Name,
                                 Name_kh = ur.Name_kh
                             })
                             .ToList();

            ViewBag.SubBlogs = subCategoriesBlogs;


           
            var Category = from ur in _db.SubCategories
                           select new SubCategory()
                           {
                               Id = ur.Id,
                               Name = ur.Name,
                               Name_kh = ur.Name_kh

                           };
            ViewBag.SubCategories = Category;

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favoriteProductIds = _db.FavoriteProducts
                                            .Where(fp => fp.UserId == userId)
                                            .Select(fp => fp.ProductId)
                                            .ToList();
                ViewBag.FavoriteProductIds = favoriteProductIds;

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            var products = _db.Products.Include(c => c.SubCategories).Include(c => c.Reviews);
            int pageSize = 30;
            return View(await PaginatedList<Products>.CreateAsync(products.AsNoTracking(), pageNumber ?? 1, pageSize));

            // return View(_db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag).ToList() /*.ToPagedList(page ?? 1, 2)*/);
        }



        //Get Product All
        [HttpGet]
        public async Task<IActionResult> AllProduct()
        {
          
            var subCategories = _db.SubCategories
                               .Include(s => s.MainCategories)
                               .Where(s => s.MainCategories.Name == "Product")
                               .Select(ur => new SubCategory()
                               {
                                   Id = ur.Id,
                                   Name = ur.Name,
                                   Name_kh = ur.Name_kh
                               })
                               .ToList();

            ViewBag.SubCategories = subCategories;

            var subCategoriesBlogs = _db.SubCategories
                              .Include(s => s.MainCategories)
                              .Where(s => s.MainCategories.Name == "Blog")
                              .Select(ur => new SubCategory()
                              {
                                  Id = ur.Id,
                                  Name = ur.Name,
                                  Name_kh = ur.Name_kh
                              })
                              .ToList();

            ViewBag.SubBlogs = subCategoriesBlogs;

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favoriteProductIds = _db.FavoriteProducts
                                            .Where(fp => fp.UserId == userId)
                                            .Select(fp => fp.ProductId)
                                            .ToList();
                ViewBag.FavoriteProductIds = favoriteProductIds;

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            var products = _db.Products
                        .Include(c => c.SubCategories)
                        .Include(c => c.Reviews) // Include the reviews for the product
                        .OrderByDescending(p => p.Id).ToList();


            int pageSize = 30;
            return View(products);
        }

        //Get Product All by category
        [HttpGet]
        [ActionName("Product-Category")]
        public async Task<IActionResult> ProductByCategory(int? id)
        {
          
            var subCategoriesBlogs = _db.SubCategories
                              .Include(s => s.MainCategories)
                              .Where(s => s.MainCategories.Name == "Blog")
                              .Select(ur => new SubCategory()
                              {
                                  Id = ur.Id,
                                  Name = ur.Name,
                                  Name_kh = ur.Name_kh,
                                  Image = ur.Image,
                                  Description = ur.Description,
                                  Description_kh = ur.Description_kh,
                              })
                              .ToList();


            var subs = _db.SubCategories
                .Include(s => s.Products)
                .ThenInclude(s => s.Reviews)
                .FirstOrDefault(s => s.Id == id);

            //return Json(filteredProducts);

            var subCategories = _db.SubCategories
                                .Include(s => s.MainCategories)
                                .Where(s => s.MainCategories.Name == "Product")
                                .Select(ur => new SubCategory()
                                {
                                    Id = ur.Id,
                                    Name = ur.Name,
                                    Name_kh = ur.Name_kh,
                                })
                                .ToList();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favoriteProductIds = _db.FavoriteProducts
                                            .Where(fp => fp.UserId == userId)
                                            .Select(fp => fp.ProductId)
                                            .ToList();
                ViewBag.FavoriteProductIds = favoriteProductIds;

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            ViewBag.SubCategories = subCategories;
            ViewBag.SubBlogs = subCategoriesBlogs;

            ////ViewBag.CategoryName = id;
            // Set the ViewBag values for the category names in the desired language
                      
            return View(subs);

            
        }

        //Get Product detail
        [HttpGet]
        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _db.Products
                 .Include(p => p.SubCategories)
                 .Include(p => p.ProductAttributes)
                 .Include(p => p.ProductAttributes) // Include the product attributes for the product
                 .ThenInclude(a => a.Attribute) // Include the attribute for each product attribute
                 .Include(p => p.Reviews) // Include the reviews for the product
                 .ThenInclude(r => r.User) // Include the user for each review
                 .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Retrieve only the approved reviews for the product
            var approvedReviews = _db.Reviews
                .Where(r => r.ProductId == id && r.IsApproved)
                .OrderByDescending(r => r.ReviewDate)
                .ToList();
            product.Reviews = approvedReviews;

            // Fetch related products from the database
            var relatedProducts = _db.Products
                                       .Include(c => c.Reviews)
                                       .Include(c => c.SubCategories)
                                       .Where(p => p.SubCategoryId == product.SubCategoryId).Take(8).ToList();

            ViewBag.RelateProducts = relatedProducts;

          
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isProductInFavorite = _db.FavoriteProducts.Any(fp => fp.UserId == userId && fp.ProductId == id);
            ViewBag.IsProductInFavorite = isProductInFavorite;

            if (User.Identity.IsAuthenticated)
            {
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favoriteProductIds = _db.FavoriteProducts
                                            .Where(fp => fp.UserId == userID)
                                            .Select(fp => fp.ProductId)
                                            .ToList();
                ViewBag.FavoriteProductIds = favoriteProductIds;


                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userID);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }                 
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
           

            
            


            return View(product);
        }


        //POST product detail acation method
        //[HttpPost]
        //[ActionName("Detail")]
        //public ActionResult ProductDetail(int? id)
        //{
        //    List<Products> products = new List<Products>();
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = _db.Products.Include(c => c.SubCategories).FirstOrDefault(c => c.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        product.Qty -= 1;

        //        _db.Update(product);
        //        _db.SaveChanges();

        //    }

        //    products = HttpContext.Session.Get<List<Products>>("products");
        //    if (products == null)
        //    {
        //        products = new List<Products>();
        //    }
        //    products.Add(product);
        //    HttpContext.Session.Set("products", products);
        //    // Set success message in TempData

        //    TempData["StatusMessage"] = "Successfully";
        //    //return View();
        //    return RedirectToAction(nameof(Detail));
        //}

        [HttpPost]
        [ActionName("Detail")]
        public ActionResult ProductDetail(int? id, int quantity)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _db.Products.Include(c => c.SubCategories).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                if (quantity > 0 && quantity <= product.Qty)
                {
                    // Store the original quantity in the session
                    HttpContext.Session.SetInt32($"originalQty_{id}", quantity);
                  
                    // Add the selected quantity of the product to the cart
                    List<Products> products = HttpContext.Session.Get<List<Products>>("products");
                    if (products == null)
                    {
                        products = new List<Products>();
                    }

                    // Add the selected quantity of the product to the cart
                    for (int i = 0; i < quantity; i++)
                    {
                        products.Add(product);
                    }

                    HttpContext.Session.Set("products", products);

                    // Set success message in TempData
                    TempData["StatusMessage"] = "Successfully added to cart.";

                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    TempData["StatusMessage"] = "Invalid quantity selected.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }
        }





        //Blog page
        [HttpGet]
        [ActionName("Blog")]
        public async Task<IActionResult> Blog(int? id)
        {

           
            var subs =  _db.SubCategories
                 .Include(s => s.Blogs)
                 .FirstOrDefault(s => s.Id == id);

            if (subs == null)
            {
                return NotFound();
            }

            var blog = _db.Blogs
             .Include(c => c.SubCategories)
             .Where(p => p.SubCategories.Id == id)
             .Select(b => new Blog
             {
                 // Map other properties
                 Title = b.SubCategories.Name,
                 Title_kh = b.SubCategories.Name_kh,
                 Image = b.SubCategories.Image,
                 Description = b.SubCategories.Description,
                 Description_kh = b.SubCategories.Description_kh,
             })
             .ToList();


            //return Json(filteredProducts);
            var lastBlogId = _db.Blogs
                .OrderByDescending(b => b.Id)
                .Select(b => b.Id)
                .FirstOrDefault();

            var latestBlog = _db.Blogs
                .Include(c => c.SubCategories)
                .FirstOrDefault(b => b.Id == lastBlogId);




            var subCategoriesBlogs = _db.SubCategories
                                .Include(s => s.MainCategories)
                                .Where(s => s.MainCategories.Name == "Blog")
                                .Select(ur => new SubCategory()
                                {
                                    Id = ur.Id,
                                    Name = ur.Name,
                                    Name_kh = ur.Name_kh,
                                    Image = ur.Image,                                   
                                    Description = ur.Description,
                                    Description_kh = ur.Description_kh,
                                })
                                .ToList();


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.Identity.IsAuthenticated)
            {

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }


            ViewBag.LatestBlog = latestBlog;
            ViewBag.SubBlogs = subCategoriesBlogs;
            ViewBag.Blog = blog;

            ViewBag.blogName = id;
            int pageSize = 30;
            return View(subs);

        //    return View("~/Areas/Customer/Views/Home/Blog.cshtml", blogs);
         }

        //Blog Details page
        [HttpGet]
        [ActionName("Blog-Details")]
        public ActionResult BlogDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var blog = _db.Blogs.Include(c => c.SubCategories).FirstOrDefault(c => c.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            var subCategoriesBlogs = _db.SubCategories
                                .Include(s => s.MainCategories)
                                .Where(s => s.MainCategories.Name == "Blog")
                                .Select(ur => new SubCategory()
                                {
                                    Id = ur.Id,
                                    Name = ur.Name,
                                    Name_kh = ur.Name_kh,
                                    Image = ur.Image,
                                    Description = ur.Description,
                                    Description_kh = ur.Description_kh,
                                })
                                .ToList();

          
            var blogs = _db.Blogs.Include(c => c.SubCategories).Where(p => p.SubCategoryId == blog.SubCategoryId).Take(3);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.Identity.IsAuthenticated)
            {
                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            ViewBag.RelateBlogs = blogs;
            ViewBag.SubBlogs = subCategoriesBlogs;

            return View(blog);
        }

        //The last Blog  page
        [HttpGet]
        [ActionName("Last-Blog")]
        public async Task<IActionResult> LastBlog(int? id, string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            // Set the current sort order for the view
            ViewData["CurrentSort"] = sortOrder;

            // If there is a search string, reset the page number to 1
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            // Query the subcategory and include the related blogs
            var blogs = _db.Blogs
                 .Include(s => s.SubCategories)
                 .FirstOrDefault(s => s.Id == id);

            // Query the list of subcategories for the dropdown menu
            var subCategoriesBlogs = _db.SubCategories
                .Include(s => s.MainCategories)
                .Where(s => s.MainCategories.Name == "Blog")
                .Select(ur => new SubCategory()
                {
                    Id = ur.Id,
                    Name = ur.Name,
                    Name_kh = ur.Name_kh,
                    Image = ur.Image,
                    Description = ur.Description,
                    Description_kh = ur.Description_kh,
                })
                .ToList();          

            var latestBlog = _db.Blogs
                .Include(c => c.SubCategories)
                .OrderByDescending(b => b.Id)
                .FirstOrDefault();

            // Query the blogs related to the selected subcategory
            var blogsInSubCategory = _db.Blogs
                .Include(c => c.SubCategories)
                .Where(p => p.SubCategories.Id == id)
                .Select(b => new Blog
                {
                    Id = b.Id,
                    Title = b.Title,
                    Title_kh = b.Title_kh,
                    Image = b.Image,
                    Description = b.Description,
                    Description_kh = b.Description_kh
                })
                .ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.Identity.IsAuthenticated)
            {

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }


            // Set the necessary ViewBag properties for the view
            ViewBag.LatestBlog = latestBlog;
            ViewBag.Blog = blogsInSubCategory;
            ViewBag.SubBlogs = subCategoriesBlogs;
            ViewBag.CategoryName = id;

            int pageSize = 30;

            // Return the view with the paginated list of blogs
            return View(blogs);

        }




        //GET product Cart action method
        [HttpGet]
        public IActionResult Cart()
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products == null)
            {
                products = new List<Products>();
            }

            Dictionary<int, int> productQuantities = new Dictionary<int, int>();

            foreach (var product in products)
            {
                if (productQuantities.ContainsKey(product.Id))
                {
                    productQuantities[product.Id] += 1; // Increment quantity for existing product
                }
                else
                {
                    productQuantities[product.Id] = 1; // Initialize quantity for new product
                }
            }

            List<Products> aggregatedProducts = new List<Products>();          
            foreach (var kvp in productQuantities)
            {

                int productId = kvp.Key;
                int quantity = kvp.Value;
                var product = products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    product.Qty = quantity; // Set the aggregated quantity
                    aggregatedProducts.Add(product);

                    
                }
                
               
            }

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            return View(aggregatedProducts);
        }

        [HttpPost]
        public IActionResult RemoveQtyCart(int? id, int removeQuantity)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    // Store the original quantity in the session
                    //int originalQuantity = HttpContext.Session.GetInt32($"originalQty_{id}") ?? 1;
                    HttpContext.Session.SetInt32($"originalQty_{id}", removeQuantity);                    

                    // Remove the product from the cart
                    products.Remove(product);                  
                    HttpContext.Session.Set("products", products);
                }
            }

            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public IActionResult AddQtyCart(int? id, int addQuantity)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.SubCategories).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                if (addQuantity > 0 && addQuantity < product.Qty)
                {
                    // Store the original quantity in the session
                    HttpContext.Session.SetInt32($"originalQty_{id}", addQuantity);
                   
                    // Add the selected quantity of the product to the cart
                    List<Products> products = HttpContext.Session.Get<List<Products>>("products");
                    if (products == null)
                    {
                        products = new List<Products>();
                    }

                    // Add the selected quantity of the product to the cart                    
                        products.Add(product);                   
                    HttpContext.Session.Set("products", products);
                  

                }
                else
                {
                    TempData["StatusMessage"] = "Invalid quantity selected.";
                    
                }
            }

            return RedirectToAction(nameof(Cart));
        }

        //GET Remove from cart action method
        [ActionName("Remove")]
        public IActionResult RemoveFromCart(int? id)
        {
            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    // Get the original quantity and add it back to the product
                    int originalQuantity = HttpContext.Session.GetInt32($"originalQty_{id}") ?? 1;
                    product.Qty += originalQuantity;

                    _db.Update(product);
                    _db.SaveChanges();

                    // Remove the product from the cart
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);
                }
            }
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public IActionResult RemoveAll(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == productId);
                if (product != null)
                {
                    // Remove the original quantity session data
                    HttpContext.Session.Remove($"originalQty_{productId}");

                    // Remove all quantities of the product from the cart
                    products.RemoveAll(p => p.Id == productId);

                    TempData["StatusMessage"] = "Successfully removed from cart.";
                    HttpContext.Session.Set("products", products);
                }
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }








        //Get About page
        [HttpGet]
        public IActionResult About()
        {
            var subCategoriesBlogs = _db.SubCategories
                               .Include(s => s.MainCategories)
                               .Where(s => s.MainCategories.Name == "Blog")
                               .Select(ur => new SubCategory()
                               {
                                   Name = ur.Name,
                                   Name_kh = ur.Name_kh,
                                   Image = ur.Image,
                                   Description = ur.Description,
                                   Description_kh = ur.Description_kh,
                               })
            .ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.Identity.IsAuthenticated)
            {

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            ViewBag.SubBlogs = subCategoriesBlogs;
            return View();
        }



        //Get User contact Action Method
        [HttpGet]
        public IActionResult Contact()
        {
            var subCategoriesBlogs = _db.SubCategories
                               .Include(s => s.MainCategories)
                               .Where(s => s.MainCategories.Name == "Blog")
                               .Select(ur => new SubCategory()
                               {
                                   Name = ur.Name,
                                   Name_kh = ur.Name_kh,
                                   Image = ur.Image,
                                   Description = ur.Description,
                                   Description_kh = ur.Description_kh,
                               })
                               .ToList();


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.Identity.IsAuthenticated)
            {

                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }


            ViewBag.SubBlogs = subCategoriesBlogs;
            return View();
        }

        //POST User contact Action Method
        [HttpPost]
        public IActionResult Contact(Contact model)
        {
            var subCategoriesBlogs = _db.SubCategories
                              .Include(s => s.MainCategories)
                              .Where(s => s.MainCategories.Name == "Blog")
                              .Select(ur => new SubCategory()
                              {
                                  Name = ur.Name,
                                  Name_kh = ur.Name_kh,
                                  Image = ur.Image,
                                  Description = ur.Description,
                                  Description_kh = ur.Description_kh,
                              })
                              .ToList();

            ViewBag.SubBlogs = subCategoriesBlogs;

            model.ContactDate = DateTime.Now;
            _db.Contacts.Add(model);
            _db.SaveChanges();
            TempData["StatusMessage"] = "YourSuccessfully";
            return RedirectToAction(nameof(Contact));



        }






        [HttpGet]
        public IActionResult FavoriteProduct(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });// Redirect to login if user is not authenticated

            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the product is already in favorites
            var favoriteProductIds = _db.FavoriteProducts
                .Where(fp => fp.UserId == userId)
                .Select(fp => fp.ProductId)
                .ToList();

            if (User.Identity.IsAuthenticated)
            {
                             
                int favoriteProductCount = _db.FavoriteProducts.Count(fp => fp.UserId == userId);
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }
            else
            {
                int favoriteProductCount = 0; // Set count to 0 for non-authenticated users
                ViewBag.FavoriteProductCount = favoriteProductCount;
            }

            return View(_db.Products.Where(p => favoriteProductIds.Contains(p.Id)).ToList());

        }


        [HttpPost]
        public IActionResult AddToFavorites(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the product is already in favorites
            var isProductInFavorites = _db.FavoriteProducts.Any(fp => fp.UserId == userId && fp.ProductId == productId);
            if (!isProductInFavorites)
            {
                // Add the product to favorites
                var favoriteProduct = new FavoriteProduct
                {
                    UserId = userId,
                    ProductId = productId,
                    FavoritedAt = DateTime.Now
                };
                _db.FavoriteProducts.Add(favoriteProduct);
                _db.SaveChanges();
                TempData["StatusMessage"] = "FavoriteAdd";
            }

            ViewBag.IsProductInFavorites = isProductInFavorites;
            //return RedirectToAction(nameof(Detail), new { id = productId });
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public IActionResult RemoveFromFavorites(int productId)
        {
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Remove the product from favorites
            var favoriteProduct = _db.FavoriteProducts.FirstOrDefault(fp => fp.UserId == userId && fp.ProductId == productId);
            if (favoriteProduct != null)
            {
                _db.FavoriteProducts.Remove(favoriteProduct);
                _db.SaveChanges();
                TempData["StatusMessage"] = "FavoriteRemove";
            }

            //return RedirectToAction(nameof(Detail), new { id = productId });
            return Redirect(Request.Headers["Referer"].ToString());
        }



        //Post product review action method
        [HttpPost]
        public IActionResult SubmitReview(int productId, int rating, string reviewTitle, string reviewText)
        {
            // Check if the user is authenticated
            //if (User.Identity.IsAuthenticated)
            //{
                // Get the current user ID using User.FindFirstValue(ClaimTypes.NameIdentifier)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Create a new review and set its properties
                var review = new Review
                {
                    ProductId = productId,
                    UserId = userId, // Set the UserId property with the current user's ID
                    Rating = rating,
                    ReviewTitle = reviewTitle,
                    ReviewText = reviewText,
                    IsApproved = false ,// Set the IsApproved property to false by default
                    ReviewDate = DateTime.Now
                };

                // Find the product by productId and add the review
                var product = _db.Products.Include(p => p.Reviews).FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    // Add the review to the product's Reviews collection
                    product.Reviews.Add(review);
                }
           
                // Save changes to the database
                _db.SaveChanges();

                TempData["StatusMessage"] = "Your review has been submitted successfully.";
            //}
            //else
            //{
            //    // Handle the case when the user is not authenticated
            //    TempData["ErrorMessage"] = "You must be logged in to submit a review.";
            //}

            return RedirectToAction(nameof(Detail), new { id = productId });
        }

      



        //ChangeLanguage
        [HttpPost]
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) });
            return LocalRedirect(returnUrl);
        }


    }
}
