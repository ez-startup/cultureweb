using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CultureWeb.Data;
using CultureWeb.Data;
using CultureWeb.Models;
using System.Data;

namespace CultureWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin , SuperUser")]
    public class MainCategoriesController : Controller
    {
        private ApplicationDbContext _db;

        public MainCategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ViewResult List(string search)
        {
            var model = from m in _db.MainCategories select m;


            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.Name!.Contains(search));

            }
            return View("Index", model);
        }

        //GET Index Action Method
        [HttpGet]
        public IActionResult Index()
        {
            return View(_db.MainCategories.ToList());
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
        public async Task<IActionResult> Create(MainCategory model)
        {
            if (ModelState.IsValid)
            {
                _db.MainCategories.Add(model);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        //GET Edit Action Method
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _db.MainCategories.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST Edit Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MainCategory model)
        {
            if (ModelState.IsValid)
            {
                _db.Update(model);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        //GET Details Action Method
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _db.MainCategories.Find(id);
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

            var model = _db.MainCategories.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST Delete Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, MainCategory model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            var models = _db.MainCategories.Find(id);
            if (models == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.Remove(models);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

    }
}
