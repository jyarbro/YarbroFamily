using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class DetailsModel : PageModel {
        private readonly DataContext _context;

        public DetailsModel(DataContext context) {
            _context = context;
        }

        public Data.Models.Home Home { get; set; }

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
    }
}
