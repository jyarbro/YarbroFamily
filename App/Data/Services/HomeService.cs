using App.Data.Models;
using App.Data.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace App.Data.Services {
    public class HomeService {
        DataContext DataContext { get; init; }

        HomeReviewScoreModifier Cost { get; set; }
        HomeReviewScoreModifier Space { get; set; }
        HomeReviewScoreModifier Bathrooms { get; set; }
        HomeReviewScoreModifier Bedrooms { get; set; }

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
            var detailIds = from detail in DataContext.HomeReviewHomeDetails
                            where detail.HomeId == home.Id
                            select detail.DetailId;

            var details = from detail in DataContext.HomeReviewDetails
                          where detailIds.Contains(detail.Id)
                          select detail;

            var score = 0;

            foreach (var detail in details) {
                var userValues = from preference in DataContext.HomeReviewUserPreferences
                                 where preference.DetailId == detail.Id
                                    && preference.UserId == user.Id
                                 select preference.Weight;

                foreach (var value in userValues) {
                    score += value;
                }
            }

            return score;
        }

        public float CostScore(HomeReviewHome home) {
            if (Cost is null) {
                Cost = DataContext.HomeReviewScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Cost)
                    ?? new HomeReviewScoreModifier {
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
                    ?? new HomeReviewScoreModifier {
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
                    ?? new HomeReviewScoreModifier {
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
                    ?? new HomeReviewScoreModifier {
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
