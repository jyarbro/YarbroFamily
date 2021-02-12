using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Homes {
    public class DeleteHomeModel : PageModel {
        readonly DataContext DataContext;

        public DeleteHomeModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        [BindProperty]
        public Data.Models.HomeReviewHome Home { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                Home = await DataContext.HomeReviewHomes
                    .Include(h => h.CreatedBy)
                    .Include(h => h.ModifiedBy)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }

            if (Home is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            if (id is not null) {
                Home = await DataContext.HomeReviewHomes.FirstOrDefaultAsync(r => r.Id == id);
            }

            if (Home is null) {
                return NotFound();
            }

            DataContext.Remove(Home);
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
