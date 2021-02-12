using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class EditFeatureCategoryModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public int Id { get; set; }

        [BindProperty]
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        [Display(Name = "Feature Category Name")]
        public string Title { get; set; }

        public EditFeatureCategoryModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                var category = DataContext.HomeReviewFeatureCategories.Find(id);

                if (category is not null) {
                    Id = category.Id;
                    Title = category.Title;
                }
                else {
                    return NotFound();
                }
            }
            else {
                return BadRequest();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var category = DataContext.HomeReviewFeatureCategories.Find(Id);

            if (category is null) {
                return NotFound();
            }

            if (category.Title != Title) {
                category.Title = Title;
                DataContext.Entry(category).State = EntityState.Modified;
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
