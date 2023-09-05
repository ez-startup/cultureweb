using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CultureWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager, ILogger<ConfirmEmailModel> logger)
        {
            _userManager = userManager;
            _logger = logger; // Inject the logger
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            _logger.LogInformation($"Received userId: {userId}, code: {code}");

            var user = await _userManager.FindByIdAsync(userId); // Find user by userId
            if (user == null)
            {
                _logger.LogError($"User with ID '{userId}' not found in the database.");
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var email = await _userManager.GetEmailAsync(user); // Get user's email

            if (userId != null || code != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, code);
                StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
                return Page();
            }
            return Page();
        }
    }
}
