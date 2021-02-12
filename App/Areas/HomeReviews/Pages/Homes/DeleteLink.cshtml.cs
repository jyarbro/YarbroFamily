using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Homes {
    public class DeleteLinkModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public DeleteLinkModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        [BindProperty]
        public Data.Models.HomeReviewLink Link { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                Link = await DataContext.HomeReviewLinks
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
                Link = await DataContext.HomeReviewLinks
                    .Include(r => r.Home)
                    .FirstOrDefaultAsync(r => r.Id == id);
            }

            if (Link is null) {
                return NotFound();
            }

            Link.Home.ModifiedById = appUser.Id;
            Link.Home.Modified = DateTime.Now;

            DataContext.Entry(Link.Home).State = EntityState.Modified;
            DataContext.HomeReviewLinks.Remove(Link);

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./HomeDetails", new { Id = Link.HomeId });
        }
    }
}
