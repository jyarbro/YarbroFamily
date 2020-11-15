﻿using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Preferences {
    public class CreatePreferenceModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public InputModel Input { get; set; }
        [BindProperty] public Data.Models.DetailCategory Category { get; set; }

        public CreatePreferenceModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                Category = DataContext.DetailCategories.Find(id);
            }

            if (Category is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var detail = await DataContext.Details.FirstOrDefaultAsync(r => r.Title == Input.Title);
            var sortOrder = await DataContext.Details.MaxAsync(r => (int?)r.SortOrder) ?? -1;

            if (detail is null) {
                detail = new Data.Models.Detail {
                    Title = Input.Title,
                    CategoryId = Category.Id,
                    SortOrder = sortOrder + 1,
                };

                DataContext.Details.Add(detail);
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        public class InputModel {
            [Required]
            [MinLength(3)]
            [MaxLength(64)]
            [Display(Name = "Preference Name")]
            public string Title { get; set; }
        }
    }
}