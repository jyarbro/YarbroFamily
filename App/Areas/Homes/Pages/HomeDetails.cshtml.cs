using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class HomeDetailsModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public Data.Models.Home Home { get; set; }
        public IList<CategoryViewModel> Categories { get; set; }

        public HomeDetailsModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Home = await DataContext.Homes
                .Include(r => r.CreatedBy)
                .Include(r => r.ModifiedBy)
                .Include(r => r.Links)
                .Include(r => r.Details)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Home is null) {
                return NotFound();
            }

            Categories = new List<CategoryViewModel>();

            var categories = await DataContext.DetailCategories
                .Include(r => r.Details)
                    .ThenInclude(r => r.HomeDetails)
                .ToListAsync();

            foreach (var category in categories.OrderBy(r => r.SortOrder)) {
                var categoryViewModel = new CategoryViewModel {
                    Title = category.Title,
                    Details = new List<DetailViewModel>()
                };

                Categories.Add(categoryViewModel);

                foreach (var detail in category.Details.OrderBy(r => r.SortOrder)) {
                    categoryViewModel.Details.Add(new DetailViewModel {
                        Id = detail.Id,
                        Title = detail.Title,
                        Value = Home.Details.FirstOrDefault(r => r.DetailId == detail.Id) is not null
                    });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var home = DataContext.Homes.Find(Home.Id);
            var details = await DataContext.Details.ToListAsync();

            foreach (var detail in details) {
                HttpContext.Request.Form.TryGetValue($"detail{detail.Id}", out var value);
                var homeDetailValue = Convert.ToBoolean(value);

                var homeDetail = await DataContext.HomeDetails.FirstOrDefaultAsync(r => r.HomeId == home.Id && r.DetailId == detail.Id);

                if (homeDetailValue && homeDetail is null) {
                    DataContext.HomeDetails.Add(new Data.Models.HomeDetail {
                        HomeId = home.Id,
                        DetailId = detail.Id,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        CreatedById = User.Identity.Name,
                        ModifiedById = User.Identity.Name
                    });

                    home.Modified = DateTime.Now;
                    home.ModifiedById = User.Identity.Name;
                    DataContext.Entry(home).State = EntityState.Modified;
                }
                else if (!homeDetailValue && homeDetail is not null) {
                    DataContext.Remove(homeDetail);

                    home.Modified = DateTime.Now;
                    home.ModifiedById = User.Identity.Name;
                    DataContext.Entry(home).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Details", new { home.Id });
        }

        public class CategoryViewModel {
            public string Title { get; set; }
            public IList<DetailViewModel> Details { get; set; }
        }

        public class DetailViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Value { get; set; }
        }
    }
}
