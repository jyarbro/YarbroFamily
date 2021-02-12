using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class EditFeatureLevelModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        [Required]
        [MinLength(1)]
        [MaxLength(64)]
        [Display(Name = "Feature Level Name")]
        public string Title { get; set; }

        [BindProperty]
        public int FeatureId { get; set; }

        public EditFeatureLevelModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet() {
            var record = DataContext.HomeReviewFeatureLevels.Find(Id);

            if (record is not null) {
                Id = record.Id;
                Title = record.Title;
                FeatureId = record.PreferenceId;
            }
            else {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = DataContext.HomeReviewFeatureLevels.Find(Id);

            if (record is null) {
                return NotFound();
            }

            if (record.Title != Title) {
                record.Title = Title;
                DataContext.Entry(record).State = EntityState.Modified;
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./EditFeature", new { Id = FeatureId });
        }
    }
}
