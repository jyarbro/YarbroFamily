using App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Features {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;

        public IList<FeatureCategoryViewModel> FeatureCategories { get; set; }
        public IList<Data.Models.AppUser> Users { get; set; }
        public IList<Data.Models.HomeReviewBaseScoreModifier> BaseScoreModifiers { get; set; }

        public IndexModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task OnGetAsync() {
            FeatureCategories = new List<FeatureCategoryViewModel>();

            Users = await DataContext.AppUsers.ToListAsync();
            BaseScoreModifiers = await DataContext.HomeReviewBaseScoreModifiers.ToListAsync();

            foreach (var category in DataContext.HomeReviewFeatureCategories.Include(r => r.Features).OrderBy(r => r.SortOrder)) {
                var categoryViewModel = new FeatureCategoryViewModel {
                    Id = category.Id,
                    Title = category.Title,
                    Features = new List<FeatureViewModel>()
                };

                FeatureCategories.Add(categoryViewModel);

                foreach (var detail in category.Features.OrderBy(r => r.SortOrder)) {
                    var detailViewModel = new FeatureViewModel {
                        Id = detail.Id,
                        Title = detail.Title,
                        Weights = new List<FeatureWeightViewModel>()
                    };

                    categoryViewModel.Features.Add(detailViewModel);
                }
            }
        }

        public async Task<IActionResult> OnPostReorderFeatureCategoriesAsync() {
            HttpContext.Request.Form.TryGetValue($"FeatureCategory[]", out var value);

            var values = value.ToString().Split(",");

            for (var i = 0; i < values.Length; i++) {
                var categoryId = Convert.ToInt32(values[i]);
                var category = DataContext.HomeReviewFeatureCategories.Find(categoryId);

                if (category is not null) {
                    category.SortOrder = i;
                    DataContext.Entry(category).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return new JsonResult(new { });
        }

        public async Task<IActionResult> OnPostReorderFeaturesAsync() {
            HttpContext.Request.Form.TryGetValue($"feature[]", out var value);

            var values = value.ToString().Split(",");

            for (var i = 0; i < values.Length; i++) {
                var featureId = Convert.ToInt32(values[i]);
                var feature = DataContext.HomeReviewFeatures.Find(featureId);

                if (feature is not null) {
                    feature.SortOrder = i;
                    DataContext.Entry(feature).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return new JsonResult(new { });
        }

        public class FeatureCategoryViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public IList<FeatureViewModel> Features { get; set; }
        }

        public class FeatureViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public IList<FeatureWeightViewModel> Weights { get; set; }
        }

        public class FeatureWeightViewModel {
            public int FeatureId { get; set; }
            public string UserId { get; set; }
            public int? Value { get; set; }
        }
    }
}
