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
                    Preferences = new List<PreferenceViewModel>()
                };

                Categories.Add(categoryViewModel);

                foreach (var preference in category.Details.OrderBy(r => r.SortOrder)) {
                    var selectedLevel = await DataContext.HomeReviewHomePreferenceLevels.FirstOrDefaultAsync(o => o.HomeId == Home.Id && o.PreferenceId == preference.Id);

                    var preferenceViewModel = new PreferenceViewModel {
                        Id = preference.Id,
                        Title = preference.Title,
                        Value = Home.Details.FirstOrDefault(r => r.DetailId == preference.Id) is not null,
                        Levels = new List<SelectListItem>()
                    };

                    categoryViewModel.Preferences.Add(preferenceViewModel);

                    var levels = await DataContext.HomeReviewPreferenceLevels.Where(o => o.PreferenceId == preference.Id).OrderBy(o => o.Level).ToListAsync();

                    foreach (var level in levels) {
                        var selectListItem = new SelectListItem {
                            Value = level.Level.ToString(),
                            Text = level.Title
                        };

                        if (selectedLevel?.PreferenceLevelId == level.Id) {
                            selectListItem.Selected = true;
                        }

                        preferenceViewModel.Levels.Add(selectListItem);
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
            var preferences = await DataContext.HomeReviewDetails
                .Include(o => o.Levels)
                .ToListAsync();

            foreach (var preference in preferences) {
                HttpContext.Request.Form.TryGetValue($"preference{preference.Id}", out var value);

                if (preference.Levels.Any()) {
                    var preferenceLevelValue = Convert.ToInt32(value);

                    var preferenceLevel = await DataContext.HomeReviewPreferenceLevels.FirstOrDefaultAsync(o => o.PreferenceId == preference.Id && o.Level == preferenceLevelValue);

                    if (preferenceLevel is null) {
                        return NotFound();
                    }

                    var homePreferenceLevel = await DataContext.HomeReviewHomePreferenceLevels
                        .Include(o => o.PreferenceLevel)
                        .FirstOrDefaultAsync(r => r.HomeId == home.Id && r.PreferenceId == preference.Id);

                    if (homePreferenceLevel is not null && homePreferenceLevel.PreferenceLevel.Level != preferenceLevel.Level) {
                        homePreferenceLevel.PreferenceLevelId = preferenceLevel.Id;
                        homePreferenceLevel.ModifiedById = User.Identity.Name;
                        homePreferenceLevel.Modified = DateTime.Now;

                        DataContext.Entry(homePreferenceLevel).State = EntityState.Modified;

                        home.Modified = DateTime.Now;
                        home.ModifiedById = User.Identity.Name;
                        DataContext.Entry(home).State = EntityState.Modified;
                    }
                    else if (homePreferenceLevel is null) {
                        DataContext.Add(new Data.Models.HomeReviewHomePreferenceLevel {
                            HomeId = home.Id,
                            PreferenceId = preference.Id,
                            PreferenceLevelId = preferenceLevel.Id,
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
                    var preferenceValue = Convert.ToBoolean(value);

                    var homePreference = await DataContext.HomeReviewHomeDetails.FirstOrDefaultAsync(r => r.HomeId == home.Id && r.DetailId == preference.Id);

                    if (preferenceValue && homePreference is null) {
                        DataContext.HomeReviewHomeDetails.Add(new Data.Models.HomeReviewHomePreference {
                            HomeId = home.Id,
                            DetailId = preference.Id,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            CreatedById = User.Identity.Name,
                            ModifiedById = User.Identity.Name
                        });

                        home.Modified = DateTime.Now;
                        home.ModifiedById = User.Identity.Name;
                        DataContext.Entry(home).State = EntityState.Modified;
                    }
                    else if (!preferenceValue && homePreference is not null) {
                        DataContext.Remove(homePreference);

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
            public IList<PreferenceViewModel> Preferences { get; set; }
        }

        public class PreferenceViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Value { get; set; }
            public IList<SelectListItem> Levels { get; set; }
        }
    }
}
