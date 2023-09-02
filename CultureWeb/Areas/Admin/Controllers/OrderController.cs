using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CultureWeb.Data;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CultureWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public OrderController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
      

        [HttpGet]
        public ViewResult List(string search)
        {
            var model = from m in _context.Orders select m;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.OrderNo!.Contains(search) || s.PhoneNo!.Contains(search) || s.Name!.Contains(search));

            }
            return View("Index", model);
        }
        public ActionResult Index()
        {
            return View(_context.Orders.OrderByDescending(o => o.Id).ToList());
        }

        // GET: Order Details/
        public ActionResult Details(int id)
        {            

            var order = _context.Orders.Include(o => o.OrderDetails)
                                        .ThenInclude(p => p.Product)
                                        .FirstOrDefault(o => o.Id == id);

            
            return View(order);
        }
       

        // GET: OrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            var order = _context.Orders.Include(o => o.OrderDetails)
                                         .ThenInclude(p => p.Product)
                                         .FirstOrDefault(o => o.Id == id);
            return View(order);
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null)
                {
                    return NotFound();
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Invoice(int id)
        {


            var order = _context.Orders.Include(o => o.OrderDetails)
                                        .ThenInclude(p => p.Product)
                                        .FirstOrDefault(o => o.Id == id);


            return View(order);
        }
    }
}
