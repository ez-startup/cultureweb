using CultureWeb.Data;
using CultureWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CultureWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin , SuperUser")]
    public class SuppliersController : Controller
    {
        private ApplicationDbContext _db;
        private readonly IStringLocalizer<SuppliersController> _localizer;

        public SuppliersController(ApplicationDbContext db, IStringLocalizer<SuppliersController> localizer)
        {
            _db = db;
            _localizer = localizer;
        }

        public ViewResult List(string search)
        {
            var model = from m in _db.Suppliers select m;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.Phone!.Contains(search) || s.SupplierName!.Contains(search) || s.SupplierName_kh!.Contains(search));

            }
            return View("Index", model);
        }

        public IActionResult Index()
        {
            var supplier = _db.Suppliers.ToList();
            return View(supplier);
        }

        //GET Create Action Method
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //POST Create Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier model)
        {
            _db.Suppliers.Add(model);
            _db.SaveChanges();
            TempData["StatusMessage"] = "YourCreatedSuccessfully";
            return RedirectToAction(nameof(Index));
        }


        //post create attribute in create product page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAtPurchase(Supplier model)
        {
            _db.Suppliers.Add(model);
            _db.SaveChanges();
            TempData["StatusMessage"] = "YourSupplierCreatedSuccessfully";
            return RedirectToAction("Create", "Purchase");
        }

        //GET Edit Action Method
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _db.Suppliers.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST Edit Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supplier model)
        {
            _db.Update(model);
            _db.SaveChanges();
            TempData["StatusMessage"] = "YourEditedSuccessfully";
            return RedirectToAction(nameof(Index));
        }

        //GET Details Action Method
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = _db.Suppliers.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }


        //GET Delete Action Method
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _db.Suppliers.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST Delete Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id, Supplier model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            var models = _db.Suppliers.Find(id);
            if (models == null)
            {
                return NotFound();
            }
            _db.Remove(models);
            _db.SaveChanges();
            TempData["StatusMessage"] = "YourDeletedSuccessfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
