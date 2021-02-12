using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class CreateFeatureCategoryModel : PageModel {
        readonly DataContext DataContext;

        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        [BindProperty] public string Title { get; set; }

        public CreateFeatureCategoryModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var existingRecord = await DataContext.HomeReviewFeatureCategories.FirstOrDefaultAsync(r => r.Title == Title);
            var sortOrder = await DataContext.HomeReviewFeatureCategories.MaxAsync(r => (int?)r.SortOrder) ?? -1;

            if (existingRecord is null) {
                existingRecord = new Data.Models.HomeReviewFeatureCategory {
                    Title = Title,
                    SortOrder = sortOrder + 1
                };

                DataContext.HomeReviewFeatureCategories.Add(existingRecord);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
