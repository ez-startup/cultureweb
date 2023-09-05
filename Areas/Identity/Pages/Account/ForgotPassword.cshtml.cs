using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using CultureWeb.Services;
using CultureWeb.Models;

namespace CultureWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(Input.Email);
        //        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return RedirectToPage("./ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please 
        //        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        //        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //        var callbackUrl = Url.Page(
        //            "/Account/ResetPassword",
        //            pageHandler: null,
        //            values: new { area = "Identity", code },
        //            protocol: Request.Scheme);

        //        await _emailSender.SendEmailAsync(
        //            Input.Email,
        //            "Reset Password",
        //            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //        return RedirectToPage("./ForgotPasswordConfirmation");
        //    }

        //    return Page();
        //}


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var email = Input.Email;
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var value = new { token = token, email = email };
                    var forgotPasswordLink = Url.Page("/Account/ResetPassword", "Identity" , value, Request.Scheme);

                    var message = new Message(new string[] { user.Email! }, "Forgot Password Link", $"Please reset your password by clicking <a href='{HtmlEncoder.Default.Encode(forgotPasswordLink)}'>here</a>.");            

                    _emailSender.SendEmail(message);

                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // To prevent user enumeration attacks, don't reveal if a user doesn't exist.
                return RedirectToPage("./ForgotPassword");
                //return StatusCode(StatusCodes.Status400BadRequest,
                //    new Response { StatusCode = "Error", Message=$"Could not send link to Email ,please try again!"});
            }

            return Page();
        }
    }
}
