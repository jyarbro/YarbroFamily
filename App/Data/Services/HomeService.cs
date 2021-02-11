using App.Data.Models;
using App.Data.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace App.Data.Services {
    public class HomeService {
        DataContext DataContext { get; init; }

        ScoreModifier Cost { get; set; }
        ScoreModifier Space { get; set; }
        ScoreModifier Bathrooms { get; set; }
        ScoreModifier Bedrooms { get; set; }

        public HomeService(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public float HomeScore(HomeReviewHome home) => BaseScore(home) + UserScores(home);

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

        public float BaseScore(HomeReviewHome home) {
            var baseScore = CostScore(home) + SpaceScore(home) + BedroomsScore(home) + BathroomsScore(home);
            baseScore /= 4;

            return baseScore;
        }

        public float CostScore(HomeReviewHome home) {
            if (Cost is null) {
                Cost = DataContext.ScoreModifiers.FirstOrDefault(o => o.Type == ScoreModifierType.Cost)
                    ?? new ScoreModifier {
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
                Space = DataContext.ScoreModifiers.FirstOrDefault(o => o.Type == ScoreModifierType.Space)
                    ?? new ScoreModifier {
                        Baseline = 2000,
                        Multiple = 200
                    };
            }

            var score = 0f;

            if (home.Space > 0) {
                score = (Space.Baseline - home.Space) / Space.Multiple;
            }

            return score;
        }

        public float BathroomsScore(HomeReviewHome home) {
            if (Bathrooms is null) {
                Bathrooms = DataContext.ScoreModifiers.FirstOrDefault(o => o.Type == ScoreModifierType.Bathrooms)
                    ?? new ScoreModifier {
                        Baseline = 2,
                        Multiple = 0.5f
                    };
            }

            var score = 0f;

            if (home.Bathrooms > 0) {
                score = (Bathrooms.Baseline - home.Bathrooms) / Bathrooms.Multiple;
            }

            return score;
        }

        public float BedroomsScore(HomeReviewHome home) {
            if (Bedrooms is null) {
                Bedrooms = DataContext.ScoreModifiers.FirstOrDefault(o => o.Type == ScoreModifierType.Bedrooms)
                    ?? new ScoreModifier {
                        Baseline = 3,
                        Multiple = 1
                    };
            }

            var score = 0f;

            if (home.Bedrooms > 0) {
                score = (Bedrooms.Baseline - home.Bedrooms) / Bedrooms.Multiple;
            }

            return score;
        }
    }
}
