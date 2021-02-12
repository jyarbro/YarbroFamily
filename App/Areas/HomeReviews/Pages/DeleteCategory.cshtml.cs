using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class DeleteCategoryModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public Data.Models.HomeReviewFeatureCategory Category { get; set; }
        
        public DeleteCategoryModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                Category = await DataContext.HomeReviewDetailCategories.FindAsync(id);
            }

            if (Category is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            Category = await DataContext.HomeReviewDetailCategories.FirstOrDefaultAsync(r => r.Id == Category.Id);

            if (Category is null) {
                return NotFound();
            }

            DataContext.Remove(Category);
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Preferences");
        }
    }
}
