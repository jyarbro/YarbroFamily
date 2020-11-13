﻿using App.Data;
using App.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Link {
    public class CreateModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        [BindProperty] public Data.Models.Home Home { get; set; }

        [Required]
        [Url]
        [BindProperty] public string Address { get; set; }

        public CreateModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        public IActionResult OnGet(int? id) {
            if (id is not null) {
                Home = DataContext.Homes.Find(id);
            }

            if (Home is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }
            
            var appUser = await AppUsers.Get(User);
            var homeRecord = await DataContext.Homes.FindAsync(Home.Id);

            if (homeRecord is null) {
                return NotFound();
            }

            var linkRecord = await DataContext.HomeLinks.FirstOrDefaultAsync(r => r.Link == Address);

            if (linkRecord is null) {
                linkRecord = new Data.Models.HomeLink {
                    Link = Address,
                    HomeId = homeRecord.Id,
                    CreatedById = appUser.Id,
                    Created = DateTime.Now,
                    ModifiedById = appUser.Id,
                    Modified = DateTime.Now
                };

                DataContext.HomeLinks.Add(linkRecord);

                homeRecord.ModifiedById = appUser.Id;
                homeRecord.Modified = DateTime.Now;

                DataContext.Entry(homeRecord).State = EntityState.Modified;

                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("/Home/Details", new { Home.Id });
        }
    }
}
