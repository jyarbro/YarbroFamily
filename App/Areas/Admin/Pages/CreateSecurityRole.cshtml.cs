using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Admin.Pages {
    public class CreateSecurityRoleModel : PageModel {
        readonly DataContext DataContext;

        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        [BindProperty] public string Title { get; set; }

        public CreateSecurityRoleModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = await DataContext.SecurityRoles.FirstOrDefaultAsync(r => r.Title == Title);

            if (record is null) {
                DataContext.Add(new Data.Models.SecurityRole {
                    Title = Title,
                    ModifiedById = User.Identity.Name,
                    Modified = DateTime.Now
                });

                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
