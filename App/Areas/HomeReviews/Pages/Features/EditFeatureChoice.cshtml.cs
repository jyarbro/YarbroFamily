﻿using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class EditFeatureChoiceModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        [Required]
        [MinLength(1)]
        [MaxLength(64)]
        [Display(Name = "Feature Choice Name")]
        public string Title { get; set; }

        [BindProperty]
        public int FeatureId { get; set; }

        public EditFeatureChoiceModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public IActionResult OnGet(int? id) {
            var record = DataContext.HomeReviewFeatureChoices.Find(id);

            if (record is not null) {
                Id = record.Id;
                Title = record.Title;
                FeatureId = record.FeatureId;
            }
            else {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = DataContext.HomeReviewFeatureChoices.Find(Id);

            if (record is null) {
                return NotFound();
            }

            if (record.Title != Title) {
                record.Title = Title;
                DataContext.Entry(record).State = EntityState.Modified;
                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./EditFeature", new { Id = FeatureId });
        }
    }
}
