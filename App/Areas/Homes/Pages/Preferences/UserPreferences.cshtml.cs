using App.Data;
using App.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Preferences {
    public class UserPreferencesModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        [BindProperty] public string UserId { get; set; }

        public Data.Models.AppUser AppUser { get; set; }
        public IList<CategoryViewModel> Categories { get; set; }

        public UserPreferencesModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        public async Task<IActionResult> OnGetAsync(string id) {
            if (id?.Length > 0) {
                AppUser = await DataContext.AppUsers.FindAsync(id);

                if (AppUser is null) {
                    return NotFound();
                }
            }
            else {
                AppUser = await AppUsers.Get(User);
            }

            UserId = AppUser.Id;
            Categories = new List<CategoryViewModel>();

            foreach (var category in DataContext.DetailCategories.Include(r => r.Details).OrderBy(r => r.SortOrder)) {
                var categoryViewModel = new CategoryViewModel {
                    Title = category.Title,
                    Details = new List<DetailViewModel>()
                };

                Categories.Add(categoryViewModel);

                foreach (var detail in category.Details.OrderBy(r => r.SortOrder)) {
                    var detailViewModel = new DetailViewModel {
                        Title = detail.Title,
                        Weights = new List<DetailWeightViewModel>()
                    };

                    categoryViewModel.Details.Add(detailViewModel);

                    var detailWeight = await DataContext.DetailWeights.FirstOrDefaultAsync(r => r.UserId == UserId && r.DetailId == detail.Id);

                    detailViewModel.Weights.Add(new DetailWeightViewModel {
                        DetailId = detail.Id,
                        Value = detailWeight?.Value
                    });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            AppUser = DataContext.AppUsers.Find(UserId);

            if (AppUser is null) {
                return NotFound();
            }

            foreach (var detail in DataContext.Details) {
                HttpContext.Request.Form.TryGetValue($"detail{detail.Id}", out var value);
                var weight = Convert.ToInt32(value);

                var record = DataContext.DetailWeights.FirstOrDefault(r => r.UserId == AppUser.Id && r.DetailId == detail.Id);

                if (record is null) {
                    record = new Data.Models.DetailWeight {
                        Value = weight,
                        DetailId = detail.Id,
                        UserId = AppUser.Id,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        CreatedById = User.Identity.Name,
                        ModifiedById = User.Identity.Name
                    };

                    DataContext.DetailWeights.Add(record);
                }
                else {
                    record.Value = weight;
                    record.Modified = DateTime.Now;
                    record.ModifiedById = User.Identity.Name;

                    DataContext.Entry(record).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./UserPreferences");
        }

        public class CategoryViewModel {
            public string Title { get; set; }
            public IList<DetailViewModel> Details { get; set; }
        }

        public class DetailViewModel {
            public string Title { get; set; }
            public IList<DetailWeightViewModel> Weights { get; set; }
        }

        public class DetailWeightViewModel {
            public int DetailId { get; set; }
            public int? Value { get; set; }
        }
    }
}
