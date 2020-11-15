﻿using App.Data;
using App.Data.Services;
using App.Utilities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUserService;
        readonly HomeService HomeService;

        public IndexModel(
            DataContext dataContext,
            AppUserService appUserService,
            HomeService homeService
        ) {
            DataContext = dataContext;
            AppUserService = appUserService;
            HomeService = homeService;
        }

        public IList<HomeModel> Homes { get; set; }

        public async Task OnGetAsync(string sort) {
            await AppUserService.Get(User);

            Homes = new List<HomeModel>();

            ViewData["sort"] = sort ?? "score";

            var users = await DataContext.AppUsers.ToListAsync();

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
                var homeModel = new HomeModel {
                    Id = home.Id,
                    Address = home.Address,
                    Score = HomeService.HomeScore(home),
                    Updated = home.Modified,
                    Created = home.Created,
                    UserScores = new List<UserScoreModel>()
                };

                Homes.Add(homeModel);

                foreach (var user in users) {
                    homeModel.UserScores.Add(new UserScoreModel {
                        Name = user.FirstName,
                        Score = HomeService.UserScore(home, user)
                    });
                }
            }

            if (sort is null or "score") {
                Homes = Homes.OrderByDescending(o => o.Score).ToList();
            }

            if (sort is "updated") {
                Homes = Homes.OrderByDescending(o => o.Updated).ToList();
            }

            if (sort is "created") {
                Homes = Homes.OrderByDescending(o => o.Created).ToList();
            }
        }

        public class HomeModel {
            public int Id { get; set; }
            public string Address { get; set; }
            public int Score { get; set; }
            public IList<UserScoreModel> UserScores { get; set; }
            public DateTime Updated { get; set; }
            public DateTime Created { get; set; }
        }

        public class UserScoreModel {
            public string Name { get; set; }
            public int Score { get; set; }
        }
    }
}
