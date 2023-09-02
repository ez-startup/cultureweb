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
    public class ProductController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public ViewResult List(string search)
        {
            var model = from m in _context.Products.Include(c => c.SubCategories) select m;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.Name!.Contains(search) || s.Name_kh!.Contains(search) || s.SubCategories.Name!.Contains(search));

            }
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Products.Include(c=>c.SubCategories).OrderByDescending(p => p.Id).ToList());
        }
        //POST Index action method
        [HttpPost]
        public IActionResult Index(decimal? lowAmount, decimal? largeAmount)
        {
            var products = _context.Products.Include(c => c.SubCategories)
                .Where(c => c.Price >= lowAmount && c.Price <= largeAmount).ToList();
            if (lowAmount == null || largeAmount == null)
            {
                products = _context.Products.Include(c => c.SubCategories).ToList();
            }
            return View(products);
        }
        //Get Create method

        public IActionResult Create()
        {
           
            // Find the MainCategory with the name "Product"
            var productMainCategory = _context.MainCategories.FirstOrDefault(mc => mc.Name == "Product");

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
         

            ViewBag.Attributes = _context.Attributes.ToList();
            return View();
        }

        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(Products product, IFormFile image, List<int> selectedAttributes)
        {
            var searchProduct = _context.Products.FirstOrDefault(c => c.Name == product.Name);
            if (searchProduct != null)
            {
                ViewBag.message = "This product already exists";             
                // Find the MainCategory with the name "Product"
                var productMainCategory = _context.MainCategories.FirstOrDefault(mc => mc.Name == "Product");

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
              

                ViewBag.Attributes = _context.Attributes.ToList();
                return View(product);
            }

            if (image != null)
            {
                var name = Path.GetFileNameWithoutExtension(image.FileName);
                var extension = Path.GetExtension(image.FileName);
                var fileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                var path = Path.Combine(_webHostEnvironment.WebRootPath + "/Images/Product/", fileName);
                await image.CopyToAsync(new FileStream(path, FileMode.Create));
                product.Image = "Images/Product/" + fileName;
            }

            if (selectedAttributes != null)
            {
                foreach (var attributeId in selectedAttributes)
                {
                    var attribute = await _context.Attributes.FindAsync(attributeId);
                    if (attribute != null)
                    {
                        if (product.ProductAttributes == null)
                            product.ProductAttributes = new List<ProductAttribute>();

                        product.ProductAttributes.Add(new ProductAttribute { 
                            AttributeId = attributeId 
                        });
                    }
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "CreatedSuccessfully";
            return RedirectToAction(nameof(Index));
        }

        //GET Edit Action Method

        public ActionResult Edit(int? id)
        {
            ViewData["subCategoryId"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name");
            ViewData["subCategoryId_kh"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name_kh");

            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(c => c.SubCategories)
                .Include(c => c.ProductAttributes) // Include product attributes
                .FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Attributes = _context.Attributes.ToList();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string old_image, Products product, IFormFile image, List<int> selectedAttributes)
        {
          
                var existingProduct = await _context.Products
                    .Include(p => p.ProductAttributes)
                    .FirstOrDefaultAsync(p => p.Id == product.Id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (image != null)
                {
                    var name = Path.GetFileNameWithoutExtension(image.FileName);
                    var extension = Path.GetExtension(image.FileName);
                    var fileName = name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "Images/Product", fileName);
                    await image.CopyToAsync(new FileStream(path, FileMode.Create));
                    existingProduct.Image = "Images/Product/" + fileName;
                    
                }
                else
                {
                    existingProduct.Image = old_image;
                }

                existingProduct.Name = product.Name;              
                existingProduct.Name_kh = product.Name_kh;              
                existingProduct.Price = product.Price;
                existingProduct.Qty = product.Qty;
                existingProduct.SubCategoryId = product.SubCategoryId;
                existingProduct.ProductColor = product.ProductColor;
                existingProduct.ProductColor_kh = product.ProductColor_kh;
                existingProduct.IsAvailable = product.IsAvailable;
                existingProduct.Information = product.Information;
                existingProduct.Description = product.Description;
                existingProduct.Description_kh = product.Description_kh;

                // Update product attributes
                if (selectedAttributes != null)
                {
                    var existingAttributeIds = existingProduct.ProductAttributes
                        .Select(pa => pa.AttributeId)
                        .ToList();

                    var attributesToRemove = existingAttributeIds.Except(selectedAttributes).ToList();
                    foreach (var attributeId in attributesToRemove)
                    {
                        var productAttribute = existingProduct.ProductAttributes
                            .FirstOrDefault(pa => pa.AttributeId == attributeId);
                        if (productAttribute != null)
                        {
                            existingProduct.ProductAttributes.Remove(productAttribute);
                        }
                    }

                    var attributesToAdd = selectedAttributes.Except(existingAttributeIds).ToList();
                    foreach (var attributeId in attributesToAdd)
                    {
                        var attribute = await _context.Attributes.FindAsync(attributeId);
                        if (attribute != null)
                        {
                            existingProduct.ProductAttributes.Add(new ProductAttribute { AttributeId = attributeId });
                        }
                    }
                }
                else
                {
                    // No selected attributes, remove all product attributes
                    existingProduct.ProductAttributes?.Clear();
                }

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                ViewData["subCategoryId"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name");
                ViewData["subCategoryId_kh"] = new SelectList(_context.SubCategories.ToList(), "Id", "Name_kh");
                ViewBag.Attributes = _context.Attributes.ToList();
                TempData["StatusMessage"] = "EditedSuccessfully";
                return RedirectToAction(nameof(Index));           
            //return View(product);
        }



        //GET Details Action Method
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products
                .Include(m => m.SubCategories)
                .Include(m => m.PurchaseDetails)
                .Include(m => m.ProductAttributes)
                .ThenInclude(ma => ma.Attribute)
                .FirstOrDefault(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            var assignedAttributeIds = product.ProductAttributes.Select(ma => ma.AttributeId).ToList();
            var availableAttributes = _context.Attributes
               .Where(a => !assignedAttributeIds.Contains(a.Id))
               .ToList();

            ViewBag.AvailableAttributes = availableAttributes;

            var assignAttributeView = new AssignAttributeView
            {
                ProductId = product.Id,
                AvailableAttributes = _context.Attributes.ToList() // Retrieve the list of available attributes from your data source
            };
            ViewBag.Product = product;
            ViewBag.AssignAttributeView = assignAttributeView;

            return View(product);
        }


        //GET Delete Action Method

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products.Include(c => c.SubCategories).Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
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

            var product = _context.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "DeletedSuccessfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AssignAttribute(int id)
        {
            var product = _context.Products.FirstOrDefault(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var assignedAttributeIds = product.ProductAttributes.Select(ma => ma.AttributeId).ToList();

            var availableAttributes = _context.Attributes
                .Where(a => !assignedAttributeIds.Contains(a.Id))
                .ToList();

            ViewBag.AvailableAttributes = availableAttributes;

            return View("Details", product);
        }


        [HttpPost]
        public async Task<IActionResult> AssignAttribute(AssignAttributeView viewModel)
        {

            var product = _context.Products
                .Include(m => m.ProductAttributes)
                .FirstOrDefault(m => m.Id == viewModel.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            var attribute = _context.Attributes.FirstOrDefault(a => a.Id == viewModel.SelectedAttributeId);

            if (attribute == null)
            {
                return NotFound();
            }

            product.ProductAttributes.Add(new ProductAttribute
            {
                ProductId = product.Id,
                AttributeId = attribute.Id
            });


            // If the model state is not valid, retrieve the available actors and return the view
            var availableAttribute = _context.Attributes.ToList();
            viewModel.AvailableAttributes = availableAttribute;

            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "assignedSuccessfully";
            return RedirectToAction("Details", new { id = viewModel.ProductId });            
        }
    }
}
