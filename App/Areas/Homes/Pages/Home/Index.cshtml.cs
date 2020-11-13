using App.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;

        public IndexModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IList<Data.Models.Home> Homes { get; set; }

        public async Task OnGetAsync() {
            Homes = await DataContext.Homes
                .Include(h => h.CreatedBy)
                .Include(h => h.ModifiedBy).ToListAsync();
        }
    }
}
