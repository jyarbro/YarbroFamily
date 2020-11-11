using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    using DataModels = Data.Models;

    public class IndexModel : PageModel {
        private readonly Data.DataContext _context;

        public IndexModel(
            Data.DataContext context
        ) {
            _context = context;
        }

        public IList<DataModels.Home> Home { get; set; }

        public async Task OnGetAsync() {
            Home = await _context.Homes
                .Include(h => h.CreatedBy)
                .Include(h => h.ModifiedBy).ToListAsync();
        }
    }
}
