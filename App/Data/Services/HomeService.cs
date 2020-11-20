using App.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace App.Data.Services {
    public class HomeService {
        readonly DataContext DataContext;

        public HomeService(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public float HomeScore(HomeReviewHome home) {
            var totalUsers = 0;
            var score = 0f;

            foreach (var user in DataContext.AppUsers) {
                totalUsers++;
                score += UserScore(home, user);
            }

            return score / totalUsers;
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
    }
}
