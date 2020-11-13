using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.DetailCategory {
    public class CreateModel : PageModel {
        readonly DataContext DataContext;

        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        [BindProperty] public string Title { get; set; }

        public CreateModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = await DataContext.DetailCategories.FirstOrDefaultAsync(r => r.Title == Title);
            var sortOrder = await DataContext.DetailCategories.MaxAsync(r => (int?)r.SortOrder) ?? -1;

            if (record is null) {
                record = new Data.Models.DetailCategory {
                    Title = Title,
                    SortOrder = sortOrder + 1
                };

                DataContext.DetailCategories.Add(record);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("/Detail/Index");
        }
    }
}
