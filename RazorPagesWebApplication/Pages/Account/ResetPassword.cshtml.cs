﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesWebApplication.Models;

namespace RazorPagesWebApplication.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Required]
        [EmailAddress]
        [ModelBinder]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [ModelBinder]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [ModelBinder]
        public string ConfirmPassword { get; set; }

        [ModelBinder]
        public string Code { get; set; }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return Redirect("~/Error");
            }
            else
            {
                Code = code;
                return View();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Redirect("~/Account/ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Code, Password);
            if (result.Succeeded)
            {
                return Redirect("~/Account/ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
    }
}