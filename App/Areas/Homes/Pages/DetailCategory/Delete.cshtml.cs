using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.DetailCategory {
    public class DeleteModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public Data.Models.DetailCategory Category { get; set; }
        
        public DeleteModel(
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
            Category = await DataContext.DetailCategories.FindAsync(Category.Id);

            if (Category is null) {
                return NotFound();
            }

            DataContext.DetailCategories.Remove(Category);

            await DataContext.SaveChangesAsync();

            return RedirectToPage("/Detail/Index");
        }
    }
}
