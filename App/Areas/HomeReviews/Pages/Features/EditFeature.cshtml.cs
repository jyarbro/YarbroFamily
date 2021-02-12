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

        public IEnumerable<Data.Models.HomeReviewFeatureLevel> Levels { get; set; }

        public EditFeatureModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGet(int? id) {
            if (id is not null) {
                var record = await DataContext.HomeReviewFeatures
                    .Include(o => o.FeatureLevels)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (record is not null) {
                    Id = record.Id;
                    Title = record.Title;
                    Levels = record.FeatureLevels.OrderBy(o => o.Level);
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

        public async Task<IActionResult> OnPostReorderLevelsAsync() {
            HttpContext.Request.Form.TryGetValue($"level[]", out var value);

            var values = value.ToString().Split(",");

            for (var i = 0; i < values.Length; i++) {
                var id = Convert.ToInt32(values[i]);
                var record = DataContext.HomeReviewFeatureLevels.Find(id);

                if (record is not null) {
                    record.Level = i;
                    DataContext.Entry(record).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return new JsonResult(new { });
        }
    }
}
