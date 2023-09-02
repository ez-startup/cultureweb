using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using CultureWeb.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Hosting;
using CultureWeb.Data;
using CultureWeb.Services;
using System.Net;

namespace CultureWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment; // Add this field
        private readonly ApplicationDbContext _db; // Add this field

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailService emailSender,
            IWebHostEnvironment hostEnvironment,
             ApplicationDbContext db) // Add IWebHostEnvironment parameter
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _hostEnvironment = hostEnvironment;
            _db = db; // Set your _db field to the injected DbContext// Initialize the hostEnvironment field
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            public string? Image { get; set; }
            [NotMapped]
            public IFormFile? ImageFile { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            //if (ModelState.IsValid)
            //{
                /*var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };*/
                var user = new ApplicationUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Image = Input.Image,
                    ImageFile = Input.ImageFile,
                    JoinDate = DateTime.Now
                };
                string uniqueFileName = UploadFile(user, _hostEnvironment);

                user.Image = uniqueFileName;
                _db.Attach(user);
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var email = Input.Email;
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                         "/Account/ConfirmEmail",
                         "Identity",
                         new { userId = user.Id, code = code }, // Include code parameter
                         Request.Scheme);

                        var message = new Message(
                            new string[] { Input.Email! },
                            "Confirm your email",
                            $"Please confirm to verify your account by <a href={callbackUrl}'>clicking here</a>.");

               

                     _emailSender.SendEmail(message);


                if (_userManager.Options.SignIn.RequireConfirmedAccount = true)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            //}

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private string UploadFile(ApplicationUser model, IWebHostEnvironment hostEnvironment)
        {
            string uniqueFileName = null;
            if (model.ImageFile != null)
            {
                string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, "Images/User/");
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
