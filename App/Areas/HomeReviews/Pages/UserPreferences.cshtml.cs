using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class UserPreferencesModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        [BindProperty] public string UserId { get; set; }

        public Data.Models.AppUser AppUser { get; set; }
        public IList<FeatureCategoryViewModel> FeatureCategories { get; set; }

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
            FeatureCategories = new List<FeatureCategoryViewModel>();

            foreach (var featureCategory in DataContext.HomeReviewFeatureCategories.Include(r => r.Details).ThenInclude(r => r.Levels).OrderBy(r => r.SortOrder)) {
                var featureCategoryViewModel = new FeatureCategoryViewModel {
                    Title = featureCategory.Title,
                    Features = new List<FeatureViewModel>()
                };

                FeatureCategories.Add(featureCategoryViewModel);

                foreach (var feature in featureCategory.Details.OrderBy(r => r.SortOrder)) {
                    var featureUserWeight = await DataContext.HomeReviewUserWeights.FirstOrDefaultAsync(r => r.UserId == UserId && r.DetailId == feature.Id);

                    var featureViewModel = new FeatureViewModel {
                        Title = feature.Title,
                        FeatureId = feature.Id,
                        Value = featureUserWeight?.Weight,
                        FeatureLevels = new List<FeatureLevelViewModel>()
                    };

                    featureCategoryViewModel.Features.Add(featureViewModel);

                    var levels = await DataContext.HomeReviewFeatureLevels.Where(r => r.PreferenceId == feature.Id).OrderBy(o => o.Level).ToListAsync();

                    foreach (var level in levels) {
                        var featureLevelUserWeight = await DataContext.HomeReviewUserWeights.FirstOrDefaultAsync(r => r.UserId == UserId && r.LevelId == level.Id);

                        var featureLevelViewModel = new FeatureLevelViewModel {
                            Title = level.Title,
                            FeatureLevelId = level.Id,
                            Value = featureLevelUserWeight?.Weight
                        };

                        featureViewModel.FeatureLevels.Add(featureLevelViewModel);
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            AppUser = DataContext.AppUsers.Find(UserId);

            if (AppUser is null) {
                return NotFound();
            }

            foreach (var feature in DataContext.HomeReviewFeatures) {
                HttpContext.Request.Form.TryGetValue($"slider_feature{feature.Id}", out var value);
                var userWeightValue = Convert.ToInt32(value);

                var record = DataContext.HomeReviewUserWeights.FirstOrDefault(r => r.UserId == AppUser.Id && r.DetailId == feature.Id);

                if (record is null) {
                    record = new Data.Models.HomeReviewUserWeight {
                        Weight = userWeightValue,
                        DetailId = feature.Id,
                        UserId = AppUser.Id,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        CreatedById = User.Identity.Name,
                        ModifiedById = User.Identity.Name
                    };

                    DataContext.HomeReviewUserWeights.Add(record);
                }
                else {
                    record.Weight = userWeightValue;
                    record.Modified = DateTime.Now;
                    record.ModifiedById = User.Identity.Name;

                    DataContext.Entry(record).State = EntityState.Modified;
                }
            }

            foreach (var featureLevel in DataContext.HomeReviewFeatureLevels) {
                HttpContext.Request.Form.TryGetValue($"slider_level{featureLevel.Id}", out var value);
                var userWeightValue = Convert.ToInt32(value);

                var record = DataContext.HomeReviewUserWeights.FirstOrDefault(r => r.UserId == AppUser.Id && r.LevelId == featureLevel.Id);

                if (record is null) {
                    record = new Data.Models.HomeReviewUserWeight {
                        Weight = userWeightValue,
                        DetailId = featureLevel.PreferenceId,
                        LevelId = featureLevel.Id,
                        UserId = AppUser.Id,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        CreatedById = User.Identity.Name,
                        ModifiedById = User.Identity.Name
                    };

                    DataContext.HomeReviewUserWeights.Add(record);
                }
                else {
                    record.UserId = User.Identity.Name;
                    record.DetailId = featureLevel.PreferenceId;
                    record.LevelId = featureLevel.Id;
                    record.Weight = userWeightValue;
                    record.Modified = DateTime.Now;
                    record.ModifiedById = User.Identity.Name;

                    DataContext.Entry(record).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./UserPreferences");
        }

        public class FeatureCategoryViewModel {
            public string Title { get; set; }
            public IList<FeatureViewModel> Features { get; set; }
        }

        public class FeatureViewModel {
            public string Title { get; set; }
            public int FeatureId { get; set; }
            public int? Value { get; set; }
            public IList<FeatureLevelViewModel> FeatureLevels { get; set; }
        }

        public class FeatureLevelViewModel {
            public string Title { get; set; }
            public int FeatureLevelId { get; set; }
            public int? Value { get; set; }
        }
    }
}
