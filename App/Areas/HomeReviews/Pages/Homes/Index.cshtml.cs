﻿using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Homes {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUserService;
        readonly HomeService HomeService;

        [BindProperty] public string Sort { get; set; }

        public IList<SelectListItem> SortOptions => new List<SelectListItem> {
            new SelectListItem { Text = "Score", Value = "score" },
            new SelectListItem { Text = "Address", Value = "address" },
            new SelectListItem { Text = "Last Updated", Value = "updated" },
            new SelectListItem { Text = "Created Date", Value = "created" },
        };

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
            Sort = sort;

            await AppUserService.Get(User);

            Homes = new List<HomeModel>();

            var users = await DataContext.AppUsers.ToListAsync();

            var homeRecords = await DataContext.HomeReviewHomes
                .Include(h => h.CreatedBy)
                .Include(h => h.ModifiedBy).ToListAsync();

            foreach (var home in homeRecords) {
                var homeModel = new HomeModel {
                    Id = home.Id,
                    Available = home.Available,
                    Finished = home.Finished,
                    Address = home.Address,
                    HouseNumber = home.HouseNumber,
                    StreetName = home.StreetName,
                    City = home.City,
                    State = home.State,
                    Zip = $"{home.Zip:00000}",
                    Score = HomeService.HomeScore(home),
                    CostScore = HomeService.CostScore(home),
                    SpaceScore = HomeService.SpaceScore(home),
                    BathroomsScore = HomeService.BathroomsScore(home),
                    BedroomsScore = HomeService.BedroomsScore(home),
                    Updated = home.Modified,
                    Created = home.Created,
                    UpdatedBy = home.ModifiedBy.FirstName,
                    CreatedBy = home.CreatedBy.FirstName,
                    UserScores = new List<UserScoreModel>()
                };

                Homes.Add(homeModel);

                foreach (var user in users) {
                    homeModel.UserScores.Add(new UserScoreModel {
                        Name = user.FirstName,
                        Score = HomeService.UserScore(home, user)
                    });
                }

                homeModel.BaseScore = HomeService.BaseScore(home);
            }

            switch (Sort) {
                default:
                case "score":
                    Homes = Homes.OrderByDescending(o => o.Score).ToList();
                    break;

                case "updated":
                    Homes = Homes.OrderByDescending(o => o.Updated).ToList();
                    break;

                case "created":
                    Homes = Homes.OrderByDescending(o => o.Created).ToList();
                    break;

                case "address":
                    Homes = Homes
                        .OrderBy(o => o.City)
                        .ThenBy(o => o.StreetName)
                        .ThenBy(o => o.HouseNumber)
                        .ToList();
                    break;
            }
        }

        public class HomeModel {
            public int Id { get; set; }
            public bool Available { get; set; }
            public bool Finished { get; set; }
            public string Address { get; set; }
            public string HouseNumber { get; set; }
            public string StreetName { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public float Score { get; set; }
            public float BaseScore { get; set; }
            public float CostScore { get; set; }
            public float SpaceScore { get; set; }
            public float BedroomsScore { get; set; }
            public float BathroomsScore { get; set; }
            public IList<UserScoreModel> UserScores { get; set; }
            public DateTime Updated { get; set; }
            public DateTime Created { get; set; }
            public string UpdatedBy { get; set; }
            public string CreatedBy { get; set; }
        }

        public class UserScoreModel {
            public string Name { get; set; }
            public int Score { get; set; }
        }
    }
}
