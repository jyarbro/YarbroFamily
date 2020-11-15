using App.Data;
using App.Data.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        public IndexModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        public IList<HomeModel> Homes { get; set; }

        public async Task OnGetAsync(string sort) {
            await AppUsers.Get(User);

            Homes = new List<HomeModel>();

            ViewData["sort"] = sort ?? "score";

            var homeRecords = await DataContext.Homes
                .Include(h => h.CreatedBy)
                .Include(h => h.ModifiedBy).ToListAsync();

            if (sort is "address") {
                homeRecords = homeRecords
                    .OrderBy(o => o.City)
                    .ThenBy(o => o.StreetName)
                    .ThenBy(o => o.HouseNumber)
                    .ToList();
            }

            foreach (var home in homeRecords) {
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

                Homes.Add(new HomeModel {
                    Id = home.Id,
                    Address = home.Address,
                    Score = score
                });
            }

            if (sort is null or "score") {
                Homes = Homes.OrderByDescending(o => o.Score).ToList();
            }
        }

        public class HomeModel {
            public int Id { get; set; }
            public string Address { get; set; }
            public int Score { get; set; }
        }
    }
}
