using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CultureWeb.Data;
using CultureWeb.Models;
using System.Data;
using Attribute = CultureWeb.Models.Attribute;
using Microsoft.Extensions.Localization;

namespace CultureWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin , SuperUser")]
    public class AttributeController : Controller
    {
        private ApplicationDbContext _db;
        private readonly IStringLocalizer<AttributeController> _localizer;

        public AttributeController(ApplicationDbContext db, IStringLocalizer<AttributeController> localizer)
        {
            _db = db;
            _localizer = localizer;
        }

        [HttpGet]
        public ViewResult List(string search)
        {
            // Retrieve all attributes from the database
            var model = from m in _db.Attributes select m;

            // Check if there is a search string provided
            if (!string.IsNullOrEmpty(search))
            {
                // Filter the attributes based on the search string
                model = model.Where(s => s.Name!.Contains(search) || s.Name_kh!.Contains(search));
            }

            // Return the filtered attributes to the "Index" view
            return View("Index", model);
        }

        //GET Index Action Method
        public IActionResult Index()
        {
            return View(_db.Attributes.ToList());
        }

        //GET Create Action Method

        public ActionResult Create()
        {
            return View();
        }

        //POST Create Action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Attribute model)
        {           
                _db.Attributes.Add(model);
                await _db.SaveChangesAsync();    
                TempData["StatusMessage"] = "YourAttributeCreatedSuccessfully";
                return RedirectToAction(nameof(Index));    
        }


        //post create attribute in create product page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAttribute(Attribute model)
        {
            _db.Attributes.Add(model);
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = "YourAttributeCreatedSuccessfully";
            return RedirectToAction("Create", "Product");
        }

        //post create attribute in edit product page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAtProEdit(Attribute model, int productId)
        {
            _db.Attributes.Add(model);
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = "YourAttributeCreatedSuccessfully";
            return RedirectToAction("Edit", "Product", new { id = productId});
        }

        //post create attribute in details product page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAtProDetails(Attribute model, int productId)
        {
            _db.Attributes.Add(model);
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = "YourAttributeCreatedSuccessfully";
            TempData["ErrorMessage"] = "YourAttributeCreatedSuccessfully";          
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        //GET Edit Action Method
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _db.Attributes.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST Edit Action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Attribute model)
        {         
                _db.Update(model);
                await _db.SaveChangesAsync();
                TempData["StatusMessage"] = "YourAttributeEditedSuccessfully";
            return RedirectToAction(nameof(Index));                   
        }

        //GET Details Action Method

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = _db.Attributes.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST Edit Action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(Attribute model)
        {
            return RedirectToAction(nameof(Index));

        }

        //GET Delete Action Method

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _db.Attributes.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST Delete Action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, Attribute model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            var models = _db.Attributes.Find(id);
            if (models == null)
            {
                return NotFound();
            }         
            _db.Remove(models);
            await _db.SaveChangesAsync();
            TempData["StatusMessage"] = "YourAttributeDeletedSuccessfully";
            return RedirectToAction(nameof(Index));                  
        }
    }
}
