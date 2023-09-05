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
    public class BlogsController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BlogsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public ViewResult List(string search)
        {
            var model = from m in _context.Blogs.Include(c => c.SubCategories) select m;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.Title!.Contains(search) || s.Title_kh!.Contains(search) || s.SubCategories.Name!.Contains(search));

            }
            return View("Index", model);
        }
        public IActionResult Index()
        {
            return View(_context.Blogs.Include(c => c.SubCategories).OrderByDescending(p => p.Id).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            // Find the MainCategory with the name "Product"
            var productMainCategory = _context.MainCategories.FirstOrDefault(mc => mc.Name == "Blog");

            if (productMainCategory != null)
            {
                // Get the associated SubCategories for the "Product" MainCategory
                var subCategoriesForProduct = _context.SubCategories
                    .Where(sc => sc.MainCategoryId == productMainCategory.Id)
                    .ToList();

                // Populate the SelectList for English names
                ViewData["subCategoryId"] = new SelectList(subCategoriesForProduct, "Id", "Name");

                // Populate the SelectList for Khmer names
                ViewData["subCategoryId_kh"] = new SelectList(subCategoriesForProduct, "Id", "Name_kh");
            }
            return View();
        }

        //Post Create method
        [HttpPost]
        public async Task<IActionResult> Create(Blog blogs, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return View(blogs);
            }
            var searchBlog = _context.Blogs.FirstOrDefault(c => c.Title == blogs.Title);
            if (searchBlog != null)
            {
                ViewBag.message = "This product is already exist";
                // Find the MainCategory with the name "Product"
                var productMainCategory = _context.MainCategories.FirstOrDefault(mc => mc.Name == "Blog");

                if (productMainCategory != null)
                {
                    // Get the associated SubCategories for the "Product" MainCategory
                    var subCategoriesForProduct = _context.SubCategories
                        .Where(sc => sc.MainCategoryId == productMainCategory.Id)
                        .ToList();

                    // Populate the SelectList for English names
                    ViewData["subCategoryId"] = new SelectList(subCategoriesForProduct, "Id", "Name");

                    // Populate the SelectList for Khmer names
                    ViewData["subCategoryId_kh"] = new SelectList(subCategoriesForProduct, "Id", "Name_kh");
                }

                return View(blogs);
            }

            if (image != null)
            {
                var name = Path.GetFileNameWithoutExtension(image.FileName);
                var extension = Path.GetExtension(image.FileName);
                var fileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                var path = Path.Combine(_webHostEnvironment.WebRootPath + "/Images/Blog/", fileName);
                await image.CopyToAsync(new FileStream(path, FileMode.Create));
                blogs.Image = "Images/Blog/" + fileName;

            }


            _context.Blogs.Add(blogs);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "CreatedSuccessfully";
            return RedirectToAction(nameof(Index));
        }

        //GET Edit Action Method
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            ViewData["subCategoryId"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name");
            ViewData["subCategoryId_kh"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name_kh");

            if (id == null)
            {
                return NotFound();
            }

            var blogs = _context.Blogs.Include(c => c.SubCategories)
                .FirstOrDefault(c => c.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }
            return View(blogs);
        }

        //POST Edit Action Method
        [HttpPost]
        public async Task<IActionResult> Edit(string old_image, Blog blogs, IFormFile image)
        {
            if (ModelState.IsValid || (ModelState.ContainsKey("Image") && ModelState["image"].Errors.Count == 1 && ModelState["Image"].Errors[0].ErrorMessage == "The image field is required."))
            {
                if (image != null)
                {                   

                    var name = Path.GetFileNameWithoutExtension(image.FileName);
                    var extension = Path.GetExtension(image.FileName);
                    var fileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                    var path = Path.Combine(_webHostEnvironment.WebRootPath + "/Images/Blog/", fileName);
                    await image.CopyToAsync(new FileStream(path, FileMode.Create));
                    blogs.Image = "Images/Blog/" + fileName;

                }
                else
                {
                    blogs.Image = old_image;
                }

                _context.Blogs.Update(blogs);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "EditedSuccessfully";
                return RedirectToAction(nameof(Index));
            }

            return View(blogs);
        }


        //GET Details Action Method
        [HttpGet]
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var blogs = _context.Blogs.Include(c => c.SubCategories).FirstOrDefault(c => c.Id == id);

            if (blogs == null)
            {
                return NotFound();
            }
            return View(blogs);
        }

        //GET Delete Action Method
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = _context.Blogs.Include(c => c.SubCategories).Where(c => c.Id == id).FirstOrDefault();
            if (blogs == null)
            {
                return NotFound();
            }
            return View(blogs);
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

            var blogs = _context.Blogs.FirstOrDefault(c => c.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blogs);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "DeletedSuccessfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

