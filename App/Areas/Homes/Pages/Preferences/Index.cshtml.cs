using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.PreferenceCategories {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;

        public IList<DetailCategoryViewModel> Categories { get; set; }
        public IList<Data.Models.AppUser> Users { get; set; }

        public IndexModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task OnGetAsync() {
            Categories = new List<DetailCategoryViewModel>();

            Users = await DataContext.AppUsers.ToListAsync();

            foreach (var category in DataContext.DetailCategories.Include(r => r.Details).OrderBy(r => r.SortOrder)) {
                var categoryViewModel = new DetailCategoryViewModel {
                    Id = category.Id,
                    Title = category.Title,
                    Details = new List<DetailViewModel>()
                };

                Categories.Add(categoryViewModel);

                foreach (var detail in category.Details.OrderBy(r => r.SortOrder)) {
                    var detailViewModel = new DetailViewModel {
                        Id = detail.Id,
                        Title = detail.Title,
                        Weights = new List<DetailWeightViewModel>()
                    };

                    categoryViewModel.Details.Add(detailViewModel);
                }
            }
        }

        public async Task<IActionResult> OnPostReorderCategoriesAsync() {
            HttpContext.Request.Form.TryGetValue($"category[]", out var value);

            var values = value.ToString().Split(",");

            for (var i = 0; i < values.Length; i++) {
                var categoryId = Convert.ToInt32(values[i]);
                var category = DataContext.DetailCategories.Find(categoryId);

                if (category is not null) {
                    category.SortOrder = i;
                    DataContext.Entry(category).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return new JsonResult(new { });
        }

        public async Task<IActionResult> OnPostReorderDetailsAsync() {
            HttpContext.Request.Form.TryGetValue($"detail[]", out var value);

            var values = value.ToString().Split(",");

            for (var i = 0; i < values.Length; i++) {
                var detailId = Convert.ToInt32(values[i]);
                var detail = DataContext.Details.Find(detailId);

                if (detail is not null) {
                    detail.SortOrder = i;
                    DataContext.Entry(detail).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return new JsonResult(new { });
        }

        public class DetailCategoryViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public IList<DetailViewModel> Details { get; set; }
        }

        public class DetailViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public IList<DetailWeightViewModel> Weights { get; set; }
        }

        public class DetailWeightViewModel {
            public int DetailId { get; set; }
            public string UserId { get; set; }
            public int? Value { get; set; }
        }
    }
}
