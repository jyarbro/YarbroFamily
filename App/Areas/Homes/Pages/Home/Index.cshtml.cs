using App.Data;
using App.Data.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public IndexModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        public IList<Data.Models.Home> Homes { get; set; }

        public async Task OnGetAsync() {
            // Ensures the user is created.
            await AppUsers.Get(User);

            Homes = await DataContext.Homes
                .Include(h => h.CreatedBy)
                .Include(h => h.ModifiedBy).ToListAsync();
        }
    }
}
