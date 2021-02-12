using App.Data;
using App.Data.Models;
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
    using InputModels = HomeReviews.Homes.InputModels;

    public class HomeDetailsModel : PageModel {
        DataContext DataContext { get; init; }
        IAuthorizationService Auth { get; init; }

        [BindProperty] public InputModels.Home Input { get; set; }
        
        public HomeReviewHome Record { get; set; }
        public List<HomeReviewLink> Links { get; set; }
        public IList<FeatureCategoryViewModel> FeatureCategories { get; set; }

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

            Record = await DataContext.HomeReviewHomes
                .Include(r => r.CreatedBy)
                .Include(r => r.ModifiedBy)
                .Include(r => r.Links)
                .Include(r => r.HomeFeatures)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Record is null) {
                return NotFound();
            }

            Input = new InputModels.Home {
                Id = Record.Id,
                Address = Record.Address,
                HouseNumber = Record.HouseNumber,
                StreetName = Record.StreetName,
                City = Record.City,
                State = Record.State,
                Zip = Record.Zip,
                Bathrooms = Record.Bathrooms,
                Bedrooms = Record.Bedrooms,
                Cost = Record.Cost,
                Space = Record.Space,
                Available = Record.Available,
                Finished = Record.Finished
            };

            Links = Record.Links;

            FeatureCategories = new List<FeatureCategoryViewModel>();

            var featureCategoryRecords = await DataContext.HomeReviewFeatureCategories
                .Include(r => r.Features)
                    .ThenInclude(r => r.HomeFeatures)
                .ToListAsync();

            foreach (var featureCategory in featureCategoryRecords.OrderBy(r => r.SortOrder)) {
                var featureCategoryViewModel = new FeatureCategoryViewModel {
                    Title = featureCategory.Title,
                    Features = new List<FeatureViewModel>()
                };

                FeatureCategories.Add(featureCategoryViewModel);

                foreach (var feature in featureCategory.Features.OrderBy(r => r.SortOrder)) {
                    var selectedLevel = await DataContext.HomeReviewHomeFeatureLevels.FirstOrDefaultAsync(o => o.HomeId == Input.Id && o.FeatureId == feature.Id);

                    var featureViewModel = new FeatureViewModel {
                        Id = feature.Id,
                        Title = feature.Title,
                        Value = Record.HomeFeatures.FirstOrDefault(r => r.FeatureId == feature.Id) is not null,
                        Levels = new List<SelectListItem>()
                    };

                    featureCategoryViewModel.Features.Add(featureViewModel);

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

            Record = DataContext.HomeReviewHomes.Find(Input.Id);

            await updateRecordDetails();

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
                        .FirstOrDefaultAsync(r => r.HomeId == Record.Id && r.FeatureId == feature.Id);

                    if (homeFeatureLevel is not null && homeFeatureLevel.FeatureLevel.Level != featureLevel.Level) {
                        homeFeatureLevel.FeatureLevelId = featureLevel.Id;
                        homeFeatureLevel.ModifiedById = User.Identity.Name;
                        homeFeatureLevel.Modified = DateTime.Now;

                        DataContext.Entry(homeFeatureLevel).State = EntityState.Modified;

                        Record.Modified = DateTime.Now;
                        Record.ModifiedById = User.Identity.Name;
                        DataContext.Entry(Record).State = EntityState.Modified;
                    }
                    else if (homeFeatureLevel is null) {
                        DataContext.Add(new HomeReviewHomeFeatureLevel {
                            HomeId = Record.Id,
                            FeatureId = feature.Id,
                            FeatureLevelId = featureLevel.Id,
                            CreatedById = User.Identity.Name,
                            Created = DateTime.Now,
                            ModifiedById = User.Identity.Name,
                            Modified = DateTime.Now
                        });

                        Record.Modified = DateTime.Now;
                        Record.ModifiedById = User.Identity.Name;
                        DataContext.Entry(Record).State = EntityState.Modified;
                    }
                }
                else {
                    var featureValue = Convert.ToBoolean(value);

                    var homeFeature = await DataContext.HomeReviewHomeFeatures.FirstOrDefaultAsync(r => r.HomeId == Record.Id && r.FeatureId == feature.Id);

                    if (featureValue && homeFeature is null) {
                        DataContext.HomeReviewHomeFeatures.Add(new HomeReviewHomeFeature {
                            HomeId = Record.Id,
                            FeatureId = feature.Id,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            CreatedById = User.Identity.Name,
                            ModifiedById = User.Identity.Name
                        });

                        Record.Modified = DateTime.Now;
                        Record.ModifiedById = User.Identity.Name;
                        DataContext.Entry(Record).State = EntityState.Modified;
                    }
                    else if (!featureValue && homeFeature is not null) {
                        DataContext.Remove(homeFeature);

                        Record.Modified = DateTime.Now;
                        Record.ModifiedById = User.Identity.Name;
                        DataContext.Entry(Record).State = EntityState.Modified;
                    }
                }
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./HomeDetails", new { Record.Id });

            async Task updateRecordDetails() {
                var update = Record.Available != Input.Available
                          || Record.Bathrooms != Input.Bathrooms
                          || Record.Bedrooms != Input.Bedrooms
                          || Record.City != Input.City
                          || Record.Cost != Input.Cost
                          || Record.Finished != Input.Finished
                          || Record.HouseNumber != Input.HouseNumber
                          || Record.Space != Input.Space
                          || Record.State != Input.State
                          || Record.StreetName != Input.StreetName
                          || Record.Zip != Input.Zip;

                if (update) {
                    Record.Available = Input.Available;
                    Record.Bathrooms = Input.Bathrooms;
                    Record.Bedrooms = Input.Bedrooms;
                    Record.City = Input.City;
                    Record.Cost = Input.Cost;
                    Record.Finished = Input.Finished;
                    Record.HouseNumber = Input.HouseNumber;
                    Record.Space = Input.Space;
                    Record.State = Input.State;
                    Record.StreetName = Input.StreetName;
                    Record.Zip = Input.Zip;

                    Record.Address = $"{Input.HouseNumber} {Input.StreetName}, {Input.City}, {Input.State} {Input.Zip}";

                    Record.Modified = DateTime.Now;
                    Record.ModifiedById = User.Identity.Name;

                    DataContext.Entry(Record).State = EntityState.Modified;
                    await DataContext.SaveChangesAsync();
                }
            }
        }

        public class FeatureCategoryViewModel {
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
