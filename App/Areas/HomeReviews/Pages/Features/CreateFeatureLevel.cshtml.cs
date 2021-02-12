using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class CreateFeatureLevelModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public InputModel Input { get; set; }
        [BindProperty] public Data.Models.HomeReviewFeature Feature { get; set; }

        public CreateFeatureLevelModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                Feature = DataContext.HomeReviewFeatures.Find(id);
            }

            if (Feature is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = await DataContext.HomeReviewFeatureLevels.FirstOrDefaultAsync(r => r.PreferenceId == Feature.Id && r.Title == Input.Title);
            var maxLevel = await DataContext.HomeReviewFeatureLevels.MaxAsync(r => (int?)r.Level) ?? -1;

            if (record is null) {
                record = new Data.Models.HomeReviewFeatureLevel {
                    Title = Input.Title,
                    PreferenceId = Feature.Id,
                    Level = maxLevel + 1,
                };

                DataContext.HomeReviewFeatureLevels.Add(record);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./EditFeature", new { Feature.Id });
        }

        public class InputModel {
            [Required]
            [MinLength(1)]
            [MaxLength(64)]
            [Display(Name = "Level Name")]
            public string Title { get; set; }
        }
    }
}
