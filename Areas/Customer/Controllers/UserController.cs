using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CultureWeb.Data;
using CultureWeb.Models;
using System.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CultureWeb.Services;

namespace CultureWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Admin , SuperUser")]
    public class UserController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailService _emailSender;
        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext db, IWebHostEnvironment hostEnvironment ,  IEmailService emailSender)
        {
            _userManager = userManager;
            _db = db;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
        }

        [HttpGet]
        public ViewResult List(string search)
        {
            var model = from m in _db.ApplicationUsers select m;

            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.FirstName!.Contains(search) || s.LastName!.Contains(search) || s.UserName!.Contains(search));

            }
            return View("Index", model);
        }

        public IActionResult Index()
        {
            var dd = _userManager.GetUserId(HttpContext.User);
            return View(_db.ApplicationUsers.ToList());
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            
                string uniqueFileName = UploadFile(user);
                user.Image = uniqueFileName;
                user.Email = user.UserName;
                _db.Attach(user);

                user.JoinDate = DateTime.Now;
                var result = await _userManager.CreateAsync(user, user.PasswordHash);
                if (result.Succeeded)
                {
                    var isSaveRole = await _userManager.AddToRoleAsync(user, "User");
                    
                    var email = user.Email;
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        null,
                        new { area = "Identity", userId = user.Id, code = code },
                        Request.Scheme,
                        Request.Host.Value);



                var message = new Message(
                        new string[] { user.Email! },
                        "Confirm your email",
                        $"Please confirm to verify your account by <a href=\"{callbackUrl}\"'>clicking here</a>.");

                    _emailSender.SendEmail(message);

                    TempData["save"] = "User has been created successfully";
                    return RedirectToRoute(new { controller = "User", action = "Index" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
          
            return RedirectToRoute(new { controller = "User", action = "Index" });
        }
        public async Task<IActionResult> Edit(string id)
        {
           
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            // Check if a new image has been uploaded
            if (user.ImageFile != null)
            {
                // Upload the new image and set the model's Image property to the new file name
                string uniqueFileName = UploadFile(user);
                user.Image = uniqueFileName;
            }
            else
            {
                // Keep the existing image file name in the model's Image property
                var existingUser = _db.ApplicationUsers.AsNoTracking().FirstOrDefault(m => m.Id == user.Id);
                if (existingUser != null)
                {
                    user.Image = existingUser.Image;
                }
            }
            var entry = _db.ChangeTracker.Entries<ApplicationUser>().FirstOrDefault(e => e.Entity.Id == user.Id);
            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();
            }
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            userInfo.Image = user.Image;
            userInfo.Gender = user.Gender;
            userInfo.Phone = user.Phone;
            userInfo.Address = user.Address;
            userInfo.BirthDate = user.BirthDate;

            var result = await _userManager.UpdateAsync(userInfo);
            if (result.Succeeded)
            {
                TempData["save"] = "User has been updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(userInfo);
        }
        public async Task<IActionResult> Details(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> Locout(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Locout(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();

            }
            userInfo.LockoutEnd = DateTime.Now.AddYears(100);
            int rowAffected = _db.SaveChanges();
            if (rowAffected > 0)
            {
                TempData["save"] = "User has been lockout successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(userInfo);
        }
        public async Task<IActionResult> Active(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Active(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();

            }
            userInfo.LockoutEnd = DateTime.Now.AddDays(-1);
            int rowAffected = _db.SaveChanges();
            if (rowAffected > 0)
            {
                TempData["save"] = "User has been active successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(userInfo);
        }
        public async Task<IActionResult> Delete(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(c => c.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();

            }
            _db.ApplicationUsers.Remove(userInfo);
            int rowAffected = _db.SaveChanges();
            if (rowAffected > 0)
            {
                TempData["save"] = "User has been delete successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(userInfo);
        }


        private string UploadFile(ApplicationUser model)
        {
            string uniqueFileName = null;
            if (model.ImageFile != null)
            {
                string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/User/");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
