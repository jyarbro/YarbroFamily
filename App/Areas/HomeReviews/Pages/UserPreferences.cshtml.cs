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

        [BindProperty] public Data.Models.AppUser AppUser { get; set; }
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

            FeatureCategories = new List<FeatureCategoryViewModel>();

            foreach (var featureCategory in DataContext.HomeReviewFeatureCategories.Include(r => r.Features).ThenInclude(r => r.FeatureChoices).OrderBy(r => r.SortOrder)) {
                var featureCategoryViewModel = new FeatureCategoryViewModel {
                    Title = featureCategory.Title,
                    Features = new List<FeatureViewModel>()
                };

                FeatureCategories.Add(featureCategoryViewModel);

                foreach (var feature in featureCategory.Features.OrderBy(r => r.SortOrder)) {
                    var featureUserWeight = await DataContext.HomeReviewUserWeights.FirstOrDefaultAsync(r => r.UserId == AppUser.Id && r.FeatureId == feature.Id);

                    var featureViewModel = new FeatureViewModel {
                        Title = feature.Title,
                        FeatureId = feature.Id,
                        Value = featureUserWeight?.Weight,
                        FeatureChoices = new List<FeatureChoiceViewModel>()
                    };

                    featureCategoryViewModel.Features.Add(featureViewModel);

                    var choices = await DataContext.HomeReviewFeatureChoices.Where(r => r.FeatureId == feature.Id).OrderBy(o => o.SortOrder).ToListAsync();

                    foreach (var choice in choices) {
                        var featureChoiceUserWeight = await DataContext.HomeReviewUserWeights.FirstOrDefaultAsync(r => r.UserId == AppUser.Id && r.FeatureChoiceId == choice.Id);

                        var featureChoiceViewModel = new FeatureChoiceViewModel {
                            Title = choice.Title,
                            FeatureChoiceId = choice.Id,
                            Value = featureChoiceUserWeight?.Weight
                        };

                        featureViewModel.FeatureChoices.Add(featureChoiceViewModel);
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            AppUser = DataContext.AppUsers.Find(AppUser.Id);

            if (AppUser is null) {
                return NotFound();
            }

            foreach (var feature in DataContext.HomeReviewFeatures) {
                HttpContext.Request.Form.TryGetValue($"slider_feature{feature.Id}", out var value);
                var userWeightValue = Convert.ToInt32(value);

                var userWeight = DataContext.HomeReviewUserWeights.FirstOrDefault(r => r.UserId == AppUser.Id && r.FeatureId == feature.Id);

                if (userWeight is null) {
                    userWeight = new Data.Models.HomeReviewUserWeight {
                        Weight = userWeightValue,
                        FeatureId = feature.Id,
                        UserId = AppUser.Id,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        CreatedById = User.Identity.Name,
                        ModifiedById = User.Identity.Name
                    };

                    DataContext.HomeReviewUserWeights.Add(userWeight);
                }
                else {
                    userWeight.Weight = userWeightValue;
                    userWeight.Modified = DateTime.Now;
                    userWeight.ModifiedById = User.Identity.Name;

                    DataContext.Entry(userWeight).State = EntityState.Modified;
                }
            }

            foreach (var featureChoice in DataContext.HomeReviewFeatureChoices) {
                HttpContext.Request.Form.TryGetValue($"slider_choice{featureChoice.Id}", out var value);
                var userWeightValue = Convert.ToInt32(value);

                var userWeight = DataContext.HomeReviewUserWeights.FirstOrDefault(r => r.UserId == AppUser.Id && r.FeatureChoiceId == featureChoice.Id);

                if (userWeight is null) {
                    userWeight = new Data.Models.HomeReviewUserWeight {
                        Weight = userWeightValue,
                        FeatureId = featureChoice.FeatureId,
                        FeatureChoiceId = featureChoice.Id,
                        UserId = AppUser.Id,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        CreatedById = User.Identity.Name,
                        ModifiedById = User.Identity.Name
                    };

                    DataContext.HomeReviewUserWeights.Add(userWeight);
                }
                else {
                    userWeight.UserId = AppUser.Id;
                    userWeight.FeatureId = featureChoice.FeatureId;
                    userWeight.FeatureChoiceId = featureChoice.Id;
                    userWeight.Weight = userWeightValue;
                    userWeight.Modified = DateTime.Now;
                    userWeight.ModifiedById = User.Identity.Name;

                    DataContext.Entry(userWeight).State = EntityState.Modified;
                }
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./UserPreferences", new { AppUser.Id });
        }

        public class FeatureCategoryViewModel {
            public string Title { get; set; }
            public IList<FeatureViewModel> Features { get; set; }
        }

        public class FeatureViewModel {
            public string Title { get; set; }
            public int FeatureId { get; set; }
            public int? Value { get; set; }
            public IList<FeatureChoiceViewModel> FeatureChoices { get; set; }
        }

        public class FeatureChoiceViewModel {
            public string Title { get; set; }
            public int FeatureChoiceId { get; set; }
            public int? Value { get; set; }
        }
    }
}
