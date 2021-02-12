using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public async Task<IActionResult> OnGet(int? id) {
            if (id is not null) {
                var record = await DataContext.HomeReviewDetails
                    .Include(o => o.Levels)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (record is not null) {
                    Input = new InputModel {
                        Id = record.Id,
                        Title = record.Title,
                        Levels = record.Levels.OrderBy(o => o.Level)
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

            var detail = DataContext.HomeReviewDetails.Find(Input.Id);

            if (detail is null) {
                return NotFound();
            }

            if (detail.Title != Input.Title) {
                detail.Title = Input.Title;
                DataContext.Entry(detail).State = EntityState.Modified;
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Preferences");
        }

        public async Task<IActionResult> OnPostReorderLevelsAsync() {
            HttpContext.Request.Form.TryGetValue($"level[]", out var value);

            var values = value.ToString().Split(",");

            for (var i = 0; i < values.Length; i++) {
                var id = Convert.ToInt32(values[i]);
                var record = DataContext.HomeReviewPreferenceLevels.Find(id);

                if (record is not null) {
                    record.Level = i;
                    DataContext.Entry(record).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return new JsonResult(new { });
        }

        public class InputModel {
            public int Id { get; set; }

            [Required]
            [MinLength(3)]
            [MaxLength(64)]
            [Display(Name = "Preference Name")]
            public string Title { get; set; }

            public IEnumerable<Data.Models.HomeReviewFeatureLevel> Levels { get; set; }
        }
    }
}
