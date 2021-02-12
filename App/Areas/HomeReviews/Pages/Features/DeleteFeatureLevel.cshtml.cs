using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class DeleteFeatureLevelModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public DeleteFeatureLevelModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        [BindProperty] public int Id { get; set; }
        [BindProperty] public string Title { get; set; }
        [BindProperty] public int FeatureId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                var record = await DataContext.HomeReviewFeatureLevels.FindAsync(id);

                if (record is null) {
                    return NotFound();
                }

                Id = record.Id;
                Title = record.Title;
                FeatureId = record.PreferenceId;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            var appUser = await AppUsers.Get(User);

            if (id is not null) {
                var record = await DataContext.HomeReviewFeatureLevels.FirstOrDefaultAsync(r => r.Id == id);

                if (record is null) {
                    return NotFound();
                }

                DataContext.Remove(record);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./EditFeature", new { Id = FeatureId });
        }
    }
}
