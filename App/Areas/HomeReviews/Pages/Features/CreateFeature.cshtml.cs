using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class CreateFeatureModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty]
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        [Display(Name = "Feature Name")]
        public string Title { get; set; }

        [BindProperty] public Data.Models.HomeReviewFeatureCategory FeatureCategory { get; set; }

        public CreateFeatureModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                FeatureCategory = DataContext.HomeReviewFeatureCategories.Find(id);
            }

            if (FeatureCategory is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var existingRecord = await DataContext.HomeReviewFeatures.FirstOrDefaultAsync(r => r.Title == Title);
            var sortOrder = await DataContext.HomeReviewFeatures.MaxAsync(r => (int?)r.SortOrder) ?? -1;

            if (existingRecord is null) {
                existingRecord = new Data.Models.HomeReviewFeature {
                    Title = Title,
                    CategoryId = FeatureCategory.Id,
                    SortOrder = sortOrder + 1,
                };

                DataContext.Add(existingRecord);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
