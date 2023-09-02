using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using CultureWeb.Data;
using CultureWeb.Models;
using System.Data;

namespace CultureWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin , SuperUser")]
    public class SubCategoriesController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SubCategoriesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public ViewResult List(string search)
        {
            var model = from m in _context.SubCategories.Include(c => c.MainCategories) select m;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.Name!.Contains(search) || s.Name_kh!.Contains(search) || s.MainCategories.Name!.Contains(search));

            }
            return View("Index", model);
        }

        public IActionResult Index()
        {
            return View(_context.SubCategories.Include(c => c.MainCategories).OrderByDescending(p => p.Id).ToList());
        }

        //POST Index action method
        [HttpGet]       
        //Get Create method

        public IActionResult Create()
        {
            ViewData["mainId"] = new SelectList(_context.MainCategories.ToList(), "Id", "Name");
            ViewData["mainId_kh"] = new SelectList(_context.MainCategories.ToList(), "Id", "Name_kh");

            return View();
        }
        //Post Create method
        [HttpPost]
        public async Task<IActionResult> Create(SubCategory subcategories, IFormFile image)
        {
            
            var searchsubcategory = _context.SubCategories.FirstOrDefault(c => c.Name == subcategories.Name);
            if (searchsubcategory != null)
            {
                ViewBag.message = "This main category is already exist";
                ViewData["mainId"] = new SelectList(_context.MainCategories.ToList(), "Id", "Name");
                ViewData["mainId_kh"] = new SelectList(_context.MainCategories.ToList(), "Id", "Name_kh");

                return View(subcategories);
            }

            if (image != null)
            {
                var name = Path.GetFileNameWithoutExtension(image.FileName);
                var extension = Path.GetExtension(image.FileName);
                var fileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                var path = Path.Combine(_webHostEnvironment.WebRootPath + "/Images/SubCategory/", fileName);
                await image.CopyToAsync(new FileStream(path, FileMode.Create));
                subcategories.Image = "Images/SubCategory/" + fileName;

            }


            _context.SubCategories.Add(subcategories);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "YourCreatedSuccessfully";

            return RedirectToAction(nameof(Index));
        }
        //GET Edit Action Method

        public ActionResult Edit(int? id)
        {
            ViewData["mainId"] = new SelectList(_context.MainCategories.ToList(), "Id", "Name");
            ViewData["mainId_kh"] = new SelectList(_context.MainCategories.ToList(), "Id", "Name_kh");

            if (id == null)
            {
                return NotFound();
            }

            var subcategories = _context.SubCategories.Include(c => c.MainCategories)
                .FirstOrDefault(c => c.Id == id);
            if (subcategories == null)
            {
                return NotFound();
            }
            return View(subcategories);
        }

        //POST Edit Action Method
        [HttpPost]
        public async Task<IActionResult> Edit(string old_image, SubCategory subcategories, IFormFile image)
        {
            //if (ModelState.ContainsKey("Image") && ModelState["Image"].Errors.Count == 1 && ModelState["Image"].Errors[0].ErrorMessage == "The image field is required.")
            //{
                if (image != null)
                {
                    var name = Path.GetFileNameWithoutExtension(image.FileName);
                    var extension = Path.GetExtension(image.FileName);
                    var fileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "Images/SubCategory", fileName);
                    await image.CopyToAsync(new FileStream(path, FileMode.Create));
                    subcategories.Image = "Images/SubCategory/" + fileName;
                }
                else
                {
                    subcategories.Image = old_image;
                }

                _context.SubCategories.Update(subcategories);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "YourEditedSuccessfully";

            return RedirectToAction(nameof(Index));
            //}

        }


        //GET Details Action Method
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var subcategories = _context.SubCategories.Include(c => c.MainCategories).FirstOrDefault(c => c.Id == id);

            if (subcategories == null)
            {
                return NotFound();
            }
            return View(subcategories);
        }

        //GET Delete Action Method

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategories = _context.SubCategories.Include(c => c.MainCategories).Where(c => c.Id == id).FirstOrDefault();
            if (subcategories == null)
            {
                return NotFound();
            }
            return View(subcategories);
        }

        //POST Delete Action Method

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcategories = _context.SubCategories.FirstOrDefault(c => c.Id == id);
            if (subcategories == null)
            {
                return NotFound();
            }

            _context.SubCategories.Remove(subcategories);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "YourDeletedSuccessfully";


            return RedirectToAction(nameof(Index));
        }
    }
}

