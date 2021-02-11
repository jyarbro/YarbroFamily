using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class DeletePreferenceLevelModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public DeletePreferenceLevelModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        [BindProperty] public int Id { get; set; }
        [BindProperty] public string Title { get; set; }
        [BindProperty] public int PreferenceId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                var record = await DataContext.HomeReviewPreferenceLevels.FindAsync(id);

                if (record is null) {
                    return NotFound();
                }

                Id = record.Id;
                Title = record.Title;
                PreferenceId = record.PreferenceId;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            var appUser = await AppUsers.Get(User);

            if (id is not null) {
                var record = await DataContext.HomeReviewPreferenceLevels.FirstOrDefaultAsync(r => r.Id == id);

                if (record is null) {
                    return NotFound();
                }

                DataContext.Remove(record);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./EditPreference", new { Id = PreferenceId });
        }
    }
}
