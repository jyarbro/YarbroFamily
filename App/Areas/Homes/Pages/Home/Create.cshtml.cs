using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class CreateModel : PageModel {
        private readonly Data.DataContext _context;

        [BindProperty]
        public Input Input { get; set; }

        public CreateModel(
            Data.DataContext context
        ) {
            _context = context;
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync() {
            PreventRapidPosts();

            if (!ModelState.IsValid) {
                return Page();
            }

            var record = new Data.Models.Home {
                Title = Input.Title,
                Description = Input.Description,
            };

            _context.Homes.Add(record);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        void PreventRapidPosts() {
            var lastPostString = HttpContext.Session.GetString("LastPost");
            var lastPost = lastPostString?.Length > 0 ? Convert.ToDateTime(lastPostString) : default;

            if (DateTime.Now < lastPost.AddSeconds(5)) {
                ModelState.AddModelError(string.Empty, "You're posting too fast.");
            }
            else {
                HttpContext.Session.SetString("LastPost", DateTime.Now.ToString());
            }
        }
    }

    public class Input {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
