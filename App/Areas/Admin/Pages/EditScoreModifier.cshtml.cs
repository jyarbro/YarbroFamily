using App.Data;
using App.Data.Models;
using App.Data.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace App.Areas.Admin.Pages {
    public class EditScoreModifierModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public ScoreModifier ScoreModifier { get; set; }

        public EditScoreModifierModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(ScoreModifierType type) {
            ScoreModifier = await DataContext.ScoreModifiers
                .Include(o => o.ModifiedBy)
                .FirstOrDefaultAsync(o => o.Type == type);

            if (ScoreModifier is null) {
                DataContext.Add(new ScoreModifier {
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
            var record = await DataContext.ScoreModifiers.FirstOrDefaultAsync(o => o.Type == ScoreModifier.Type);

            if (record is null) {
                return NotFound();
            }

            record.Baseline = ScoreModifier.Baseline;
            record.Multiple = ScoreModifier.Multiple;

            DataContext.Entry(record).State = EntityState.Modified;
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
