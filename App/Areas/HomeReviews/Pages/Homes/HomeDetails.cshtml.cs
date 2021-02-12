using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nrrdio.Utilities.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Homes {
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
                .Include(r => r.HomeFeatures)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Home is null) {
                return NotFound();
            }

            Categories = new List<CategoryViewModel>();

            var categories = await DataContext.HomeReviewFeatureCategories
                .Include(r => r.Features)
                    .ThenInclude(r => r.HomeFeatures)
                .ToListAsync();

            foreach (var category in categories.OrderBy(r => r.SortOrder)) {
                var categoryViewModel = new CategoryViewModel {
                    Title = category.Title,
                    Features = new List<FeatureViewModel>()
                };

                Categories.Add(categoryViewModel);

                foreach (var feature in category.Features.OrderBy(r => r.SortOrder)) {
                    var selectedLevel = await DataContext.HomeReviewHomeFeatureLevels.FirstOrDefaultAsync(o => o.HomeId == Home.Id && o.FeatureId == feature.Id);

                    var featureViewModel = new FeatureViewModel {
                        Id = feature.Id,
                        Title = feature.Title,
                        Value = Home.HomeFeatures.FirstOrDefault(r => r.FeatureId == feature.Id) is not null,
                        Levels = new List<SelectListItem>()
                    };

                    categoryViewModel.Features.Add(featureViewModel);

                    var levels = await DataContext.HomeReviewFeatureLevels.Where(o => o.FeatureId == feature.Id).OrderBy(o => o.Level).ToListAsync();

                    foreach (var level in levels) {
                        var selectListItem = new SelectListItem {
                            Value = level.Level.ToString(),
                            Text = level.Title
                        };

                        if (selectedLevel?.FeatureLevelId == level.Id) {
                            selectListItem.Selected = true;
                        }

                        featureViewModel.Levels.Add(selectListItem);
                    }
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
            var features = await DataContext.HomeReviewFeatures
                .Include(o => o.FeatureLevels)
                .ToListAsync();

            foreach (var feature in features) {
                HttpContext.Request.Form.TryGetValue($"feature{feature.Id}", out var value);

                if (feature.FeatureLevels.Any()) {
                    var featureLevelValue = Convert.ToInt32(value);

                    var featureLevel = await DataContext.HomeReviewFeatureLevels.FirstOrDefaultAsync(o => o.FeatureId == feature.Id && o.Level == featureLevelValue);

                    if (featureLevel is null) {
                        return NotFound();
                    }

                    var homeFeatureLevel = await DataContext.HomeReviewHomeFeatureLevels
                        .Include(o => o.FeatureLevel)
                        .FirstOrDefaultAsync(r => r.HomeId == home.Id && r.FeatureId == feature.Id);

                    if (homeFeatureLevel is not null && homeFeatureLevel.FeatureLevel.Level != featureLevel.Level) {
                        homeFeatureLevel.FeatureLevelId = featureLevel.Id;
                        homeFeatureLevel.ModifiedById = User.Identity.Name;
                        homeFeatureLevel.Modified = DateTime.Now;

                        DataContext.Entry(homeFeatureLevel).State = EntityState.Modified;

                        home.Modified = DateTime.Now;
                        home.ModifiedById = User.Identity.Name;
                        DataContext.Entry(home).State = EntityState.Modified;
                    }
                    else if (homeFeatureLevel is null) {
                        DataContext.Add(new Data.Models.HomeReviewHomeFeatureLevel {
                            HomeId = home.Id,
                            FeatureId = feature.Id,
                            FeatureLevelId = featureLevel.Id,
                            CreatedById = User.Identity.Name,
                            Created = DateTime.Now,
                            ModifiedById = User.Identity.Name,
                            Modified = DateTime.Now
                        });

                        home.Modified = DateTime.Now;
                        home.ModifiedById = User.Identity.Name;
                        DataContext.Entry(home).State = EntityState.Modified;
                    }
                }
                else {
                    var featureValue = Convert.ToBoolean(value);

                    var homeFeature = await DataContext.HomeReviewHomeFeatures.FirstOrDefaultAsync(r => r.HomeId == home.Id && r.FeatureId == feature.Id);

                    if (featureValue && homeFeature is null) {
                        DataContext.HomeReviewHomeFeatures.Add(new Data.Models.HomeReviewHomeFeature {
                            HomeId = home.Id,
                            FeatureId = feature.Id,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            CreatedById = User.Identity.Name,
                            ModifiedById = User.Identity.Name
                        });

                        home.Modified = DateTime.Now;
                        home.ModifiedById = User.Identity.Name;
                        DataContext.Entry(home).State = EntityState.Modified;
                    }
                    else if (!featureValue && homeFeature is not null) {
                        DataContext.Remove(homeFeature);

                        home.Modified = DateTime.Now;
                        home.ModifiedById = User.Identity.Name;
                        DataContext.Entry(home).State = EntityState.Modified;
                    }
                }
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./HomeDetails", new { home.Id });
        }

        public class CategoryViewModel {
            public string Title { get; set; }
            public IList<FeatureViewModel> Features { get; set; }
        }

        public class FeatureViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Value { get; set; }
            public IList<SelectListItem> Levels { get; set; }
        }
    }
}
