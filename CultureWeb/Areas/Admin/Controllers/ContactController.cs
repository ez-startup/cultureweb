using CultureWeb.Data;
using CultureWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CultureWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        private ApplicationDbContext _db;

        public ContactController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_db.Contacts.ToList());
        }

        //GET Details Action Method
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _db.Contacts.Find(id);
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

            var model = _db.Contacts.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //POST Delete Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, Contact model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            var models = _db.Contacts.Find(id);
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
