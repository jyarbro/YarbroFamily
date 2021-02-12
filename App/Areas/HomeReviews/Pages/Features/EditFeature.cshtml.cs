using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class EditFeatureModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        [Required]
        [MinLength(3)]
        [MaxLength(64)]
        [Display(Name = "Feature Name")]
        public string Title { get; set; }

        public IEnumerable<Data.Models.HomeReviewFeatureChoice> Choices { get; set; }

        public EditFeatureModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGet(int? id) {
            if (id is not null) {
                var record = await DataContext.HomeReviewFeatures
                    .Include(o => o.FeatureChoices)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (record is not null) {
                    Id = record.Id;
                    Title = record.Title;
                    Choices = record.FeatureChoices.OrderBy(o => o.SortOrder);
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

            var detail = DataContext.HomeReviewFeatures.Find(Id);

            if (detail is null) {
                return NotFound();
            }

            if (detail.Title != Title) {
                detail.Title = Title;
                DataContext.Entry(detail).State = EntityState.Modified;
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostReorderChoicesAsync(int[] choices) {
            for (var i = 0; i < choices.Length; i++) {
                var id = Convert.ToInt32(choices[i]);
                var record = DataContext.HomeReviewFeatureChoices.Find(id);

                if (record is not null) {
                    record.SortOrder = i;
                    DataContext.Entry(record).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return new JsonResult(new { });
        }
    }
}
