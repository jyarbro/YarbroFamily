using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class DeleteFeatureCategoryModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public Data.Models.HomeReviewFeatureCategory Category { get; set; }

        public DeleteFeatureCategoryModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                Category = await DataContext.HomeReviewFeatureCategories.FindAsync(id);
            }

            if (Category is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            Category = await DataContext.HomeReviewFeatureCategories
                .Include(o => o.Details)
                    .ThenInclude(o => o.Details)
                .Include(o => o.Details)
                    .ThenInclude(o => o.HomePreferenceLevels)
                .Include(o => o.Details)
                    .ThenInclude(o => o.Weights)
                .FirstOrDefaultAsync(o => o.Id == Category.Id);

            if (Category is null) {
                return NotFound();
            }

            foreach (var feature in Category.Details) {
                feature.Details.Clear();
                feature.HomePreferenceLevels.Clear();
                feature.Weights.Clear();
                await DataContext.SaveChangesAsync();
            }

            Category.Details.Clear();
            await DataContext.SaveChangesAsync();

            DataContext.Remove(Category);
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
