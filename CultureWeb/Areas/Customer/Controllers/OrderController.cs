using Microsoft.AspNetCore.Mvc;
using CultureWeb.Data;
using CultureWeb.Models;
using CultureWeb.Utility;
using System.Security.Claims;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace CultureWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;       
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
           
        }

        //GET Checkout actioin method
        [HttpGet]
        public IActionResult Checkout()
        {
            ViewBag.SuccessOrder = false;

           

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

                // Find the first occurrence of the product in the list
                var product = products.FirstOrDefault(p => p.Id == productId);

                if (product != null)
                {
                    product.Qty = quantity; // Set the aggregated quantity

                    // Check if the product is already added to the aggregated list
                    if (!aggregatedProducts.Any(p => p.Id == productId))
                    {
                        aggregatedProducts.Add(product);
                    }
                }
            }         

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

            return View(aggregatedProducts);
        }


        //POST Checkout action method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order anOrder)
        {
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


            List<Products> products = HttpContext.Session.Get<List<Products>>("products");
            if (products != null)
            {
               
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

                //List<Products> aggregatedProducts = new List<Products>();
                foreach (var kvp in productQuantities)
                {

                    int productId = kvp.Key;
                    int Quantity = kvp.Value;
                    var product = products.FirstOrDefault(p => p.Id == productId);
                    if (product != null)
                    {
                        OrderDetails orderDetails = new OrderDetails();
                        orderDetails.PorductId = product.Id;
                        orderDetails.QtyOrder = Quantity;
                        anOrder.OrderDetails.Add(orderDetails);
                        //anOrder.OrderDetails.Add(orderDetails);

                        var databaseProduct = _db.Products.FirstOrDefault(p => p.Id == product.Id);
                        if (databaseProduct != null)
                        {
                            databaseProduct.Qty -= Quantity; // Subtract the ordered quantity
                            _db.Update(databaseProduct);
                        }

                    }


                }

            }


            // Set success message in TempData
            anOrder.UserId = userId;
            anOrder.OrderNo = GetOrderNo();
            anOrder.OrderDate = DateTime.Now;
            _db.Orders.Add(anOrder);
            await _db.SaveChangesAsync();
            HttpContext.Session.Set("products", new List<Products>());
            ViewBag.SuccessOrder = true;
            return View();

        }


        public string GetOrderNo()
        {
            int rowCount = _db.Orders.ToList().Count() + 1;
            return rowCount.ToString("000");
        }

        public int GetOrdersForTodayCount()
        {
            DateTime today = DateTime.Today;
            int orderCount = _db.Orders.Count(order => order.OrderDate.Date == today);
            return orderCount;
        }

    }
}
