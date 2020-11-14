using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Preferences {
    public class EditCategoryModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public InputModel Input { get; set; }

        public EditCategoryModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                var category = DataContext.DetailCategories.Find(id);

                if (category is not null) {
                    Input = new InputModel {
                        Id = category.Id,
                        Title = category.Title
                    };
                }
            }

            if (Input is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var category = DataContext.DetailCategories.Find(Input.Id);

            if (category is null) {
                return NotFound();
            }

            if (category.Title != Input.Title) {
                category.Title = Input.Title;
                DataContext.Entry(category).State = EntityState.Modified;
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class InputModel {
            public int Id { get; set; }

            [Required]
            [MinLength(3)]
            [MaxLength(64)]
            [Display(Name = "Category Name")]
            public string Title { get; set; }
        }
    }
}
