using App.Data;
using App.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Detail {
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

        [BindProperty] public int Id { get; set; }
        [BindProperty] public string Title { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                var record = await DataContext.Details.FindAsync(id);

                if (record is null) {
                    return NotFound();
                }

                Id = record.Id;
                Title = record.Title;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            var appUser = await AppUsers.Get(User);
            
            if (id is not null) {
                var detail = await DataContext.Details.FindAsync(id);

                if (detail is null) {
                    return NotFound();
                }

                var weights = await DataContext.DetailWeights.Where(r => r.DetailId == detail.Id).ToListAsync();

                DataContext.RemoveRange(weights);
                DataContext.Remove(detail);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./Overview");
        }
    }
}
