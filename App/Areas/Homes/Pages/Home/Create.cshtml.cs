using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Home {
    public class CreateModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;
                
        [BindProperty] public InputModel Input { get; set; }
        public bool Confirmed { get; set; }

        public CreateModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        public IActionResult OnGet() => Page();

        public IActionResult OnPost() {
            Confirmed = true;

            var parts = Input.Address.Split(",");

            if (parts.Length > 0) {
                var subParts = parts[0].Trim().Split(" ").ToList();

                if (subParts.Count > 1) {
                    Input.HouseNumber = subParts[0].Trim();
                    subParts.RemoveAt(0);

                    Input.StreetName = string.Join(" ", subParts).Trim();
                }
            }

            if (parts.Length > 1) {
                Input.City = parts[1];
            }

            if (parts.Length > 2) {
                var subParts = parts[2].Trim().Split(" ");

                if (subParts.Length > 1) {
                    Input.State = subParts[0].Trim();
                    Input.Zip = Convert.ToInt32(subParts[1].Trim());
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmedAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var record = await DataContext.Homes.FirstOrDefaultAsync(r => r.Address == Input.Address);

            if (record is not null) {
                return RedirectToPage("./Details", new { record.Id });
            }

            var appUser = await AppUsers.Get(User);

            record = new Data.Models.Home {
                Address = Input.Address,
                HouseNumber = Input.HouseNumber,
                StreetName = Input.StreetName,
                City = Input.City,
                State = Input.State,
                Zip = Input.Zip,
                CreatedById = appUser.Id,
                Created = DateTime.Now,
                ModifiedById = appUser.Id,
                Modified = DateTime.Now
            };

            DataContext.Homes.Add(record);
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Details", new { record.Id });
        }

        public class InputModel {
            [Required]
            [Display(Name = "Street Address")]
            [MaxLength(256)]
            public string Address { get; set; }

            [Required]
            [Display(Name = "House Number")]
            [MaxLength(32)]
            public string HouseNumber { get; set; }

            [Required]
            [Display(Name = "Street")]
            [MaxLength(128)]
            public string StreetName { get; set; }
            
            [Required]
            [MaxLength(64)]
            public string City { get; set; }
            
            [Required]
            [MaxLength(2)]
            public string State { get; set; }

            [Required]
            [Range(10000,99999)]
            [Display(Name = "Zip Code")]
            public int Zip { get; set; }
        }
    }
}
