using App.Data;
using App.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace App.Areas.Admin.Pages {
    public class DeleteUserModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public AppUser AppUser { get; set; }

        public DeleteUserModel(
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

            DataContext.HomeReviewUserPreferences.RemoveRange(record.Preferences);
            DataContext.Remove(record);

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
