using App.Data.Models;
using App.Utilities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace App.Data.Services {
    public class HomeService {
        readonly DataContext DataContext;
        readonly ScoreModifiers ScoreModifiers;

        public HomeService(
            DataContext dataContext,
            IOptions<ScoreModifiers> options
        ) {
            DataContext = dataContext;
            ScoreModifiers = options.Value;
        }

        public float HomeScore(HomeReviewHome home) {
            var homeScore = BaseScore(home) + UserScores(home);
            homeScore /= 2;

            return homeScore;
        }

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
            var costScore = 0f;
            var spaceScore = 0f;
            var bedroomsScore = 0f;
            var bathroomsScore = 0f;
            var elements = 0;

            if (home.Cost > 0) {
                costScore = (ScoreModifiers.CostBase - home.Cost) / ScoreModifiers.CostMultiple;
                elements++;
            }

            if (home.Space > 0) {
                spaceScore = (home.Space - ScoreModifiers.SpaceBase) / ScoreModifiers.SpaceMultiple;
                elements++;
            }

            if (home.Bedrooms > 0) {
                bedroomsScore = (home.Bedrooms - ScoreModifiers.BedroomsBase) / ScoreModifiers.BedroomsMultiple;
                elements++;
            }

            if (home.Bathrooms > 0) {
                bathroomsScore = (home.Bathrooms - ScoreModifiers.BathroomsBase) / ScoreModifiers.BathroomsMultiple;
                elements++;
            }

            var baseScore = costScore + spaceScore + bedroomsScore + bathroomsScore;

            if (elements > 0) { 
                baseScore /= elements;
            }

            return baseScore;
        }
    }
}
