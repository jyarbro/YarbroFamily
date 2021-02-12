using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class CreateFeatureChoiceModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public InputModel Input { get; set; }
        [BindProperty] public Data.Models.HomeReviewFeature Feature { get; set; }

        public CreateFeatureChoiceModel(
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

            var record = await DataContext.HomeReviewFeatureChoices.FirstOrDefaultAsync(r => r.FeatureId == Feature.Id && r.Title == Input.Title);
            var max = await DataContext.HomeReviewFeatureChoices.MaxAsync(r => (int?)r.SortOrder) ?? -1;

            if (record is null) {
                record = new Data.Models.HomeReviewFeatureChoice {
                    Title = Input.Title,
                    FeatureId = Feature.Id,
                    SortOrder = max + 1,
                };

                DataContext.HomeReviewFeatureChoices.Add(record);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./EditFeature", new { Feature.Id });
        }

        public class InputModel {
            [Required]
            [MinLength(1)]
            [MaxLength(64)]
            [Display(Name = "Choice Name")]
            public string Title { get; set; }
        }
    }
}
