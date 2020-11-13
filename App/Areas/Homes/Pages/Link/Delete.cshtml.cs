using App.Data;
using App.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Link {
    public class DeleteModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public DeleteModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        [BindProperty]
        public Data.Models.HomeLink Link { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                Link = await DataContext.HomeLinks
                    .Include(r => r.CreatedBy)
                    .Include(r => r.ModifiedBy)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }

            if (Link is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            var appUser = await AppUsers.Get(User);
            
            if (id is not null) {
                Link = await DataContext.HomeLinks
                    .Include(r => r.Home)
                    .FirstOrDefaultAsync(r => r.Id == id);
            }

            if (Link is null) {
                return NotFound();
            }

            Link.Home.ModifiedById = appUser.Id;
            Link.Home.Modified = DateTime.Now;
            
            DataContext.Entry(Link.Home).State = EntityState.Modified;
            DataContext.HomeLinks.Remove(Link);

            await DataContext.SaveChangesAsync();

            return RedirectToPage("/Home/Details", new { Id = Link.HomeId });
        }
    }
}
