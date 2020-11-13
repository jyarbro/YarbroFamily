using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class DetailsModel : PageModel {
        readonly DataContext DataContext;

        public Data.Models.Home Home { get; set; }
        
        public DetailsModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Home = await DataContext.Homes
                .Include(r => r.CreatedBy)
                .Include(r => r.ModifiedBy)
                .Include(r => r.Links)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Home is null) {
                return NotFound();
            }

            return Page();
        }
    }
}
