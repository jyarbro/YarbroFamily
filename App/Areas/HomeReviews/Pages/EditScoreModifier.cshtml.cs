using App.Data;
using App.Data.Models;
using App.Data.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class EditScoreModifierModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public HomeReviewBaseScoreModifier ScoreModifier { get; set; }

        public EditScoreModifierModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(HomeReviewScoreModifierType type) {
            ScoreModifier = await DataContext.HomeReviewScoreModifiers
                .Include(o => o.ModifiedBy)
                .FirstOrDefaultAsync(o => o.Type == type);

            if (ScoreModifier is null) {
                DataContext.Add(new HomeReviewBaseScoreModifier {
                    Type = type,
                    Baseline = 0,
                    Multiple = 0,
                    Modified = DateTime.Now,
                    ModifiedById = User.Identity.Name
                });

                await DataContext.SaveChangesAsync();

                return RedirectToPage("./EditScoreModifier", new { type });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = await DataContext.HomeReviewScoreModifiers.FirstOrDefaultAsync(o => o.Type == ScoreModifier.Type);

            if (record is null) {
                return NotFound();
            }

            record.Baseline = ScoreModifier.Baseline;
            record.Multiple = ScoreModifier.Multiple;

            DataContext.Entry(record).State = EntityState.Modified;
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Preferences");
        }
    }
}
