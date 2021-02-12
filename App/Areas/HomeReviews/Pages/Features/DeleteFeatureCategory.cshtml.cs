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
                .Include(o => o.Features)
                    .ThenInclude(o => o.HomeFeatures)
                .Include(o => o.Features)
                    .ThenInclude(o => o.HomeFeatureLevels)
                .Include(o => o.Features)
                    .ThenInclude(o => o.UserWeights)
                .FirstOrDefaultAsync(o => o.Id == Category.Id);

            if (Category is null) {
                return NotFound();
            }

            foreach (var feature in Category.Features) {
                feature.HomeFeatures.Clear();
                feature.HomeFeatureLevels.Clear();
                feature.UserWeights.Clear();
                await DataContext.SaveChangesAsync();
            }

            Category.Features.Clear();
            await DataContext.SaveChangesAsync();

            DataContext.Remove(Category);
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
