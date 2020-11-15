using App.Data;
using App.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class EditPreferenceModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public InputModel Input { get; set; }

        public EditPreferenceModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                var detail = DataContext.Details.Find(id);

                if (detail is not null) {
                    Input = new InputModel {
                        Id = detail.Id,
                        Title = detail.Title
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

            var detail = DataContext.Details.Find(Input.Id);

            if (detail is null) {
                return NotFound();
            }

            if (detail.Title != Input.Title) {
                detail.Title = Input.Title;
                DataContext.Entry(detail).State = EntityState.Modified;
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public class InputModel {
            public int Id { get; set; }

            [Required]
            [MinLength(3)]
            [MaxLength(64)]
            [Display(Name = "Preference Name")]
            public string Title { get; set; }
        }
    }
}
