using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class DeleteFeatureModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public DeleteFeatureModel(
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
                var record = await DataContext.HomeReviewFeatures.FindAsync(id);

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
                var record = await DataContext.HomeReviewFeatures
                    .Include(o => o.FeatureChoices)
                    .Include(o => o.UserWeights)
                    .Include(o => o.HomeFeatures)
                    .Include(o => o.HomeFeatureChoices)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (record is null) {
                    return NotFound();
                }

                foreach (var choice in record.FeatureChoices) {
                    choice.HomeFeatureChoices?.Clear();
                    await DataContext.SaveChangesAsync();

                    choice.UserWeights?.Clear();
                    await DataContext.SaveChangesAsync();
                }

                record.FeatureChoices?.Clear();
                await DataContext.SaveChangesAsync();

                record.UserWeights?.Clear();
                await DataContext.SaveChangesAsync();

                record.HomeFeatures?.Clear();
                await DataContext.SaveChangesAsync();

                record.HomeFeatureChoices?.Clear();
                await DataContext.SaveChangesAsync();

                DataContext.Remove(record);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
