using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    using DataModels = Data.Models;

    public class DeleteModel : PageModel {
        private readonly DataContext _context;

        public DeleteModel(DataContext context) {
            _context = context;
        }

        [BindProperty]
        public DataModels.Home Home { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Home = await _context.Homes
                .Include(h => h.CreatedBy)
                .Include(h => h.ModifiedBy).FirstOrDefaultAsync(m => m.Id == id);

            if (Home == null) {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Home = await _context.Homes.FindAsync(id);

            if (Home != null) {
                _context.Homes.Remove(Home);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
