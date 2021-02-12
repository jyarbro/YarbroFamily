using App.Data.Models;
using App.Data.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace App.Data.Services {
    public class HomeService {
        DataContext DataContext { get; init; }

        HomeReviewBaseScoreModifier Cost { get; set; }
        HomeReviewBaseScoreModifier Space { get; set; }
        HomeReviewBaseScoreModifier Bathrooms { get; set; }
        HomeReviewBaseScoreModifier Bedrooms { get; set; }

        public HomeService(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public float HomeScore(HomeReviewHome home) => BaseScore(home) + UserScores(home);
        public float BaseScore(HomeReviewHome home) => CostScore(home) + SpaceScore(home) + BedroomsScore(home) + BathroomsScore(home);

        public float UserScores(HomeReviewHome home) {
            var totalUsers = 0;
            var userScore = 0f;

            foreach (var user in DataContext.AppUsers) {
                totalUsers++;
                userScore += UserScore(home, user);
            }

            userScore /= totalUsers;

            return userScore;
        }

        public int UserScore(HomeReviewHome home, AppUser user) {
            var homePreferenceIds = from homePreference in DataContext.HomeReviewHomeDetails
                                    where homePreference.HomeId == home.Id
                                    select homePreference.DetailId;

            var homePreferences = DataContext.HomeReviewDetails.Where(o => homePreferenceIds.Contains(o.Id)).ToList();

            var score = 0;

            // First calculate the scores of the preferences with no levels.
            foreach (var homePreference in homePreferences) {
                var userPreferences = from preference in DataContext.HomeReviewUserPreferences
                                      where preference.DetailId == homePreference.Id
                                          && preference.UserId == user.Id
                                      select preference.Weight;

                foreach (var value in userPreferences) {
                    score += value;
                }
            }

            var homePreferenceLevelIds = from record in DataContext.HomeReviewHomePreferenceLevels
                                         where record.HomeId == home.Id
                                         select record.PreferenceLevelId;

            var homePreferenceLevels = DataContext.HomeReviewPreferenceLevels.Where(o => homePreferenceLevelIds.Contains(o.Id)).ToList();

            // Then add the scores for the preferences that have levels.
            foreach (var homePreferenceLevel in homePreferenceLevels) {
                var userPreferences = from preference in DataContext.HomeReviewUserPreferences
                                      where preference.LevelId == homePreferenceLevel.Id
                                          && preference.UserId == user.Id
                                      select preference.Weight;

                foreach (var value in userPreferences) {
                    score += value;
                }
            }

            return score;
        }

        public float CostScore(HomeReviewHome home) {
            if (Cost is null) {
                Cost = DataContext.HomeReviewScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Cost)
                    ?? new HomeReviewBaseScoreModifier {
                        Baseline = 2000,
                        Multiple = 150
                    };
            }

            var score = 0f;

            if (home.Cost > 0) {
                score = (Cost.Baseline - home.Cost) / Cost.Multiple;
            }

            return score;
        }

        public float SpaceScore(HomeReviewHome home) {
            if (Space is null) {
                Space = DataContext.HomeReviewScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Space)
                    ?? new HomeReviewBaseScoreModifier {
                        Baseline = 2000,
                        Multiple = 200
                    };
            }

            var score = 0f;

            if (home.Space > 0) {
                score = (home.Space - Space.Baseline) / Space.Multiple;
            }

            return score;
        }

        public float BathroomsScore(HomeReviewHome home) {
            if (Bathrooms is null) {
                Bathrooms = DataContext.HomeReviewScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Bathrooms)
                    ?? new HomeReviewBaseScoreModifier {
                        Baseline = 2,
                        Multiple = 0.5f
                    };
            }

            var score = 0f;

            if (home.Bathrooms > 0) {
                score = (home.Bathrooms - Bathrooms.Baseline) / Bathrooms.Multiple;
            }

            return score;
        }

        public float BedroomsScore(HomeReviewHome home) {
            if (Bedrooms is null) {
                Bedrooms = DataContext.HomeReviewScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Bedrooms)
                    ?? new HomeReviewBaseScoreModifier {
                        Baseline = 3,
                        Multiple = 1
                    };
            }

            var score = 0f;

            if (home.Bedrooms > 0) {
                score = (home.Bedrooms - Bedrooms.Baseline) / Bedrooms.Multiple;
            }

            return score;
        }
    }
}
