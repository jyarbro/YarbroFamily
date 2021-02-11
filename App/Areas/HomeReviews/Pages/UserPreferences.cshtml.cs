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

            foreach (var category in DataContext.HomeReviewDetailCategories.Include(r => r.Details).ThenInclude(r => r.Levels).OrderBy(r => r.SortOrder)) {
                var categoryViewModel = new CategoryViewModel {
                    Title = category.Title,
                    Preferences = new List<PreferenceViewModel>()
                };

                Categories.Add(categoryViewModel);

                foreach (var preference in category.Details.OrderBy(r => r.SortOrder)) {
                    var preferenceWeight = await DataContext.HomeReviewUserPreferences.FirstOrDefaultAsync(r => r.UserId == UserId && r.DetailId == preference.Id);
                    
                    var preferenceViewModel = new PreferenceViewModel {
                        Title = preference.Title,
                        PreferenceId = preference.Id,
                        Value = preferenceWeight?.Weight,
                        Levels = new List<PreferenceLevelViewModel>()
                    };

                    categoryViewModel.Preferences.Add(preferenceViewModel);

                    var levels = await DataContext.HomeReviewPreferenceLevels.Where(r => r.PreferenceId == preference.Id).OrderBy(o => o.Level).ToListAsync();

                    foreach (var level in levels) {
                        var levelWeight = await DataContext.HomeReviewUserPreferences.FirstOrDefaultAsync(r => r.UserId == UserId && r.LevelId == level.Id);
                        
                        var levelViewModel = new PreferenceLevelViewModel {
                            Title = level.Title,
                            LevelId = level.Id,
                            Value = levelWeight?.Weight
                        };

                        preferenceViewModel.Levels.Add(levelViewModel);
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

            foreach (var preference in DataContext.HomeReviewDetails) {
                HttpContext.Request.Form.TryGetValue($"slider_preference{preference.Id}", out var value);
                var userPreferenceValue = Convert.ToInt32(value);

                var record = DataContext.HomeReviewUserPreferences.FirstOrDefault(r => r.UserId == AppUser.Id && r.DetailId == preference.Id);

                if (record is null) {
                    record = new Data.Models.HomeReviewUserPreference {
                        Weight = userPreferenceValue,
                        DetailId = preference.Id,
                        UserId = AppUser.Id,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        CreatedById = User.Identity.Name,
                        ModifiedById = User.Identity.Name
                    };

                    DataContext.HomeReviewUserPreferences.Add(record);
                }
                else {
                    record.Weight = userPreferenceValue;
                    record.Modified = DateTime.Now;
                    record.ModifiedById = User.Identity.Name;

                    DataContext.Entry(record).State = EntityState.Modified;
                }
            }

            foreach (var level in DataContext.HomeReviewPreferenceLevels) {
                HttpContext.Request.Form.TryGetValue($"slider_level{level.Id}", out var value);
                var userPreferenceValue = Convert.ToInt32(value);

                var record = DataContext.HomeReviewUserPreferences.FirstOrDefault(r => r.UserId == AppUser.Id && r.LevelId == level.Id);

                if (record is null) {
                    record = new Data.Models.HomeReviewUserPreference {
                        Weight = userPreferenceValue,
                        DetailId = level.PreferenceId,
                        LevelId = level.Id,
                        UserId = AppUser.Id,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        CreatedById = User.Identity.Name,
                        ModifiedById = User.Identity.Name
                    };

                    DataContext.HomeReviewUserPreferences.Add(record);
                }
                else {
                    record.UserId = User.Identity.Name;
                    record.DetailId = level.PreferenceId;
                    record.LevelId = level.Id;
                    record.Weight = userPreferenceValue;
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
            public IList<PreferenceViewModel> Preferences { get; set; }
        }

        public class PreferenceViewModel {
            public string Title { get; set; }
            public int PreferenceId { get; set; }
            public int? Value { get; set; }
            public IList<PreferenceLevelViewModel> Levels { get; set; }
        }

        public class PreferenceLevelViewModel {
            public string Title { get; set; }
            public int LevelId { get; set; }
            public int? Value { get; set; }
        }
    }
}
