using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class DeleteModel : PageModel {
        readonly DataContext DataContext;

        public DeleteModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        [BindProperty]
        public Data.Models.Home Home { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                Home = await DataContext.Homes
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
                Home = await DataContext.Homes
                    .Include(r => r.Details)
                    .Include(r => r.Links)
                    .FirstOrDefaultAsync(r => r.Id == id);
            }

            if (Home is null) {
                return NotFound();
            }

            DataContext.RemoveRange(Home.Details);
            DataContext.RemoveRange(Home.Links);
            DataContext.Remove(Home);
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
