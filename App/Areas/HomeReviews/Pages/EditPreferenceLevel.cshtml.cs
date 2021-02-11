using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class EditPreferenceLevelModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public InputModel Input { get; set; }

        public EditPreferenceLevelModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                var record = DataContext.HomeReviewPreferenceLevels.Find(id);

                if (record is not null) {
                    Input = new InputModel {
                        Id = record.Id,
                        Title = record.Title,
                        PreferenceId = record.PreferenceId
                    };
                }
            }

            if (Input is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = DataContext.HomeReviewPreferenceLevels.Find(Input.Id);

            if (record is null) {
                return NotFound();
            }

            if (record.Title != Input.Title) {
                record.Title = Input.Title;
                DataContext.Entry(record).State = EntityState.Modified;
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./EditPreference", new { Id = Input.PreferenceId });
        }

        public class InputModel {
            public int Id { get; set; }

            [Required]
            [MinLength(1)]
            [MaxLength(64)]
            [Display(Name = "Preference Level Name")]
            public string Title { get; set; }

            public int PreferenceId { get; set; }
        }
    }
}
