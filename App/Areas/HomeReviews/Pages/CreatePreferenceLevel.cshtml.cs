using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class CreatePreferenceLevelModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public InputModel Input { get; set; }
        [BindProperty] public Data.Models.HomeReviewPreference Preference { get; set; }

        public CreatePreferenceLevelModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                Preference = DataContext.HomeReviewDetails.Find(id);
            }

            if (Preference is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var detail = await DataContext.HomeReviewPreferenceLevels.FirstOrDefaultAsync(r => r.PreferenceId == Preference.Id && r.Title == Input.Title);
            var level = await DataContext.HomeReviewPreferenceLevels.MaxAsync(r => (int?)r.Level) ?? -1;

            if (detail is null) {
                detail = new Data.Models.HomeReviewPreferenceLevel {
                    Title = Input.Title,
                    PreferenceId = Preference.Id,
                    Level = level + 1,
                };

                DataContext.HomeReviewPreferenceLevels.Add(detail);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./EditPreference", new { Preference.Id });
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
