using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using COMP4911Timesheets.Data;
using COMP4911Timesheets.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using COMP4911Timesheets.Controllers;

namespace COMP4911Timesheets.Areas.Identity.Pages.Account.Manage
{
    public class SignatureModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public SignatureModel(
            ApplicationDbContext context,
            UserManager<Employee> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Current pass phrase")]
            public string OldPassPhrase { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "New pass phrase")]
            public string NewPassPhrase { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Confirm new pass phrase")]
            [Compare("NewPassPhrase", ErrorMessage = "The new pass phrase and confirmation pass phrase do not match.")]
            public string ConfirmPassPhrase { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var signature = _context.Signatures.Where(s => s.EmployeeId == user.Id).Where(s => s.Status == Signature.VALID).FirstOrDefault();
            ViewData["signature"] = signature;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var datetime = DateTime.Now;
            var signature = new Signature
            {
                HashedSignature = Utility.HashEncrypt(Input.NewPassPhrase + datetime),
                CreatedTime = datetime,
                Status = Signature.VALID,
                EmployeeId = user.Id
            };
            if (Input.OldPassPhrase != null)
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                var oldSignature = _context.Signatures.Where(s => s.EmployeeId == user.Id).Where(s => s.Status == Signature.VALID).FirstOrDefault();
                var decryptedOldSignature = Utility.HashDecrypt(oldSignature.HashedSignature);
                if (Input.OldPassPhrase + oldSignature.CreatedTime != decryptedOldSignature)
                {
                    StatusMessage = "Please enter the correct old pass phrase";
                    return RedirectToPage();
                }
                else
                {
                    oldSignature.Status = Signature.INVALID;
                    _context.Update(oldSignature);
                    await _context.SaveChangesAsync();
                }
            }
            _context.Signatures.Add(signature);
            await _context.SaveChangesAsync();
            StatusMessage = "Your pass phrase has been changed.";

            return RedirectToPage();
        }
    }
}
