using App.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Data.Services {
    public class HomeService {
        readonly DataContext DataContext;

        public HomeService(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<int> Score(Home home) {
            var totalUsers = await DataContext.AppUsers.CountAsync();

            var detailIds = from detail in DataContext.HomeDetails
                            where detail.HomeId == home.Id
                            select detail.DetailId;

            var details = from detail in DataContext.Details
                          where detailIds.Contains(detail.Id)
                          select detail;

            var score = 0;

            foreach (var detail in details) {
                var userValues = from preference in DataContext.UserPreferences
                                 where preference.DetailId == detail.Id
                                 select preference.Weight;

                var detailScore = 0;

                foreach (var value in userValues) {
                    detailScore += value;
                }

                score += detailScore / totalUsers;
            }

            return score;
        }
    }
}
