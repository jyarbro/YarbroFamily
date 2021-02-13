using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class DeleteFeatureChoiceModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public DeleteFeatureChoiceModel(
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
                var record = await DataContext.HomeReviewFeatureChoices.FindAsync(id);

                if (record is null) {
                    return NotFound();
                }

                Id = record.Id;
                Title = record.Title;
                FeatureId = record.FeatureId;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            var appUser = await AppUsers.Get(User);

            if (id is not null) {
                var record = await DataContext.HomeReviewFeatureChoices
                    .Include(o => o.HomeFeatureChoices)
                    .Include(o => o.UserWeights)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (record is null) {
                    return NotFound();
                }

                record.UserWeights.Clear();
                await DataContext.SaveChangesAsync();

                record.HomeFeatureChoices.Clear();
                await DataContext.SaveChangesAsync();

                DataContext.Remove(record);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./EditFeature", new { Id = FeatureId });
        }
    }
}
