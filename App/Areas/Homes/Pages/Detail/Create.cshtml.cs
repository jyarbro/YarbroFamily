using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Detail {
    public class CreateModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public InputModel Input { get; set; }
        public IList<SelectListItem> Categories { get; set; }

        public CreateModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet() {
            Categories = new List<SelectListItem>();

            foreach (var category in DataContext.DetailCategories) {
                Categories.Add(new SelectListItem {
                    Text = category.Title,
                    Value = category.Id.ToString()
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = await DataContext.Details.FirstOrDefaultAsync(r => r.Title == Input.Title);
            var sortOrder = await DataContext.Details.MaxAsync(r => (int?)r.SortOrder) ?? -1;

            if (record is null) {
                record = new Data.Models.Detail {
                    Title = Input.Title,
                    CategoryId = Input.CategoryId,
                    SortOrder = sortOrder + 1,
                };

                DataContext.Details.Add(record);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./Overview");
        }

        public class InputModel {
            [Required]
            [MinLength(3)]
            [MaxLength(64)]
            [Display(Name = "Name of Detail")]
            public string Title { get; set; }

            [Display(Name = "Category")]
            public int CategoryId { get; set; }
        }
    }
}
