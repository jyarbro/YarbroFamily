using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class DeletePreferenceModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public DeletePreferenceModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        [BindProperty] public int Id { get; set; }
        [BindProperty] public string Title { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                var record = await DataContext.HomeReviewDetails.FindAsync(id);

                if (record is null) {
                    return NotFound();
                }

                Id = record.Id;
                Title = record.Title;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            var appUser = await AppUsers.Get(User);

            if (id is not null) {
                var detail = await DataContext.HomeReviewDetails
                    .Include(o => o.Levels)
                    .Include(o => o.Weights)
                    .Include(o => o.Details)
                    .Include(o => o.HomePreferenceLevels)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (detail is null) {
                    return NotFound();
                }

                foreach (var level in detail.Levels) {
                    level.HomePreferenceLevels.Clear();
                    await DataContext.SaveChangesAsync();

                    level.Weights.Clear();
                    await DataContext.SaveChangesAsync();
                }

                detail.Levels.Clear();
                await DataContext.SaveChangesAsync();
                
                detail.Weights.Clear();
                await DataContext.SaveChangesAsync();

                detail.Details.Clear();
                await DataContext.SaveChangesAsync();

                detail.HomePreferenceLevels.Clear();
                await DataContext.SaveChangesAsync();

                DataContext.Remove(detail);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./Preferences");
        }
    }
}
