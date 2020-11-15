using App.Data;
using App.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace App.Areas.Admin.Pages {
    public class EditUserModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public AppUser AppUser { get; set; }

        public EditUserModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(string id) {
            AppUser = await DataContext.AppUsers.FindAsync(id);

            if (AppUser is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var record = DataContext.AppUsers.Find(AppUser.Id);

            if (record is null) {
                return NotFound();
            }

            record.Name = AppUser.Name;
            record.FirstName = AppUser.FirstName;
            record.ModifiedById = User.Identity.Name;
            record.Modified = DateTime.Now;

            DataContext.Entry(record).State = EntityState.Modified;
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./EditUser", new { AppUser.Id });
        }
    }
}
