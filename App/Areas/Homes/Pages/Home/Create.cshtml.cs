using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    using DataModels = Data.Models;

    public class CreateModel : PageModel {
        private readonly Data.DataContext _context;

        public CreateModel(Data.DataContext context) {
            _context = context;
        }

        public IActionResult OnGet() {
            ViewData["CreatedById"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["ModifiedById"] = new SelectList(_context.AppUsers, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public DataModels.Home Home { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            _context.Homes.Add(Home);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
