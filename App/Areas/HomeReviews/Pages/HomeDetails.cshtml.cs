using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nrrdio.Utilities.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class HomeDetailsModel : PageModel {
        readonly DataContext DataContext;
        readonly IAuthorizationService Auth;

        [BindProperty] public Data.Models.HomeReviewHome Home { get; set; }
        public IList<CategoryViewModel> Categories { get; set; }

        public HomeDetailsModel(
            DataContext dataContext,
            IAuthorizationService auth
        ) {
            DataContext = dataContext;
            Auth = auth;
        }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Home = await DataContext.HomeReviewHomes
                .Include(r => r.CreatedBy)
                .Include(r => r.ModifiedBy)
                .Include(r => r.Links)
                .Include(r => r.Details)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Home is null) {
                return NotFound();
            }

            Categories = new List<CategoryViewModel>();

            var categories = await DataContext.HomeReviewDetailCategories
                .Include(r => r.Details)
                    .ThenInclude(r => r.Details)
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
            if (!await Auth.IsParent(User)) {
                return Forbid();
            }

            if (!ModelState.IsValid) {
                return Page();
            }

            var home = DataContext.HomeReviewHomes.Find(Home.Id);
            var details = await DataContext.HomeReviewDetails.ToListAsync();

            foreach (var detail in details) {
                HttpContext.Request.Form.TryGetValue($"detail{detail.Id}", out var value);
                var homeDetailValue = Convert.ToBoolean(value);

                var homeDetail = await DataContext.HomeReviewHomeDetails.FirstOrDefaultAsync(r => r.HomeId == home.Id && r.DetailId == detail.Id);

                if (homeDetailValue && homeDetail is null) {
                    DataContext.HomeReviewHomeDetails.Add(new Data.Models.HomeReviewHomeDetail {
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

            return RedirectToPage("./HomeDetails", new { home.Id });
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
