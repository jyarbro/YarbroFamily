using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Preferences {
    public class DeleteCategoryModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public Data.Models.DetailCategory Category { get; set; }
        
        public DeleteCategoryModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                Category = await DataContext.DetailCategories.FindAsync(id);
            }

            if (Category is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            Category = await DataContext.DetailCategories
                .Include(r => r.Details).ThenInclude(r => r.Weights)
                .Include(r => r.Details).ThenInclude(r => r.HomeDetails)
                .FirstOrDefaultAsync(r => r.Id == Category.Id);

            if (Category is null) {
                return NotFound();
            }

            foreach (var detail in Category.Details) {
                DataContext.RemoveRange(detail.HomeDetails);
                DataContext.RemoveRange(detail.Weights);
                DataContext.Remove(detail);
            }

            DataContext.Remove(Category);
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
