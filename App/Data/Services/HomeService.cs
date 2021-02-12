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
            var homeFeatureIds = from homeFeature in DataContext.HomeReviewHomeFeatures
                                    where homeFeature.HomeId == home.Id
                                    select homeFeature.FeatureId;

            var homeFeatures = DataContext.HomeReviewFeatures.Where(o => homeFeatureIds.Contains(o.Id)).ToList();

            var score = 0;

            // First calculate the scores of the features with no choices.
            foreach (var homeFeature in homeFeatures) {
                var userWeights = from userWeight in DataContext.HomeReviewUserWeights
                                  where userWeight.FeatureId == homeFeature.Id
                                      && userWeight.UserId == user.Id
                                  select userWeight.Weight;

                foreach (var userWeight in userWeights) {
                    score += userWeight;
                }
            }

            var homeFeatureChoiceIds = from record in DataContext.HomeReviewHomeFeatureChoices
                                         where record.HomeId == home.Id
                                         select record.FeatureChoiceId;

            var homeFeatureChoices = DataContext.HomeReviewFeatureChoices.Where(o => homeFeatureChoiceIds.Contains(o.Id)).ToList();

            // Then add the scores for the features that have choices.
            foreach (var homeFeatureChoice in homeFeatureChoices) {
                var userWeights = from userWeight in DataContext.HomeReviewUserWeights
                                  where userWeight.FeatureChoiceId == homeFeatureChoice.Id
                                      && userWeight.UserId == user.Id
                                  select userWeight.Weight;

                foreach (var userWeight in userWeights) {
                    score += userWeight;

                }
            }

            return score;
        }

        public float CostScore(HomeReviewHome home) {
            if (Cost is null) {
                Cost = DataContext.HomeReviewBaseScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Cost)
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
                Space = DataContext.HomeReviewBaseScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Space)
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
                Bathrooms = DataContext.HomeReviewBaseScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Bathrooms)
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
                Bedrooms = DataContext.HomeReviewBaseScoreModifiers.FirstOrDefault(o => o.Type == HomeReviewScoreModifierType.Bedrooms)
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
