using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class CreateHomeModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        [BindProperty] public InputModel Input { get; set; }

        public bool Confirmed { get; set; }

        public CreateHomeModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        public IActionResult OnGet() => Page();

        public IActionResult OnGetConfirm() {
            Confirmed = true;

            Input = new InputModel {
                Address = HttpContext.Request.Query["Input.Address"]
            };

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

            Input.Address = $"{Input.HouseNumber} {Input.StreetName}, {Input.City}, {Input.State} {Input.Zip}";

            var record = await DataContext.Homes.FirstOrDefaultAsync(r => r.Address == Input.Address);

            if (record is not null) {
                return RedirectToPage("./HomeDetails", new { record.Id });
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

            return RedirectToPage("./HomeDetails", new { record.Id });
        }

        public class InputModel {
            [Required]
            [Display(Name = "Street Address")]
            [MaxLength(256)]
            public string Address { get; set; }

            [Required]
            [Display(Name = "House Number")]
            [MinLength(1)]
            [MaxLength(32)]
            [RegularExpression(@"^([a-zA-Z0-9\.-]+)$", ErrorMessage = "This must be letters, numbers, periods, and dashes.")]
            public string HouseNumber { get; set; }

            [Required]
            [Display(Name = "Street")]
            [MinLength(1)]
            [MaxLength(128)]
            [RegularExpression(@"^([a-zA-Z0-9 \.&'-]+)$", ErrorMessage = "This must be letters, numbers, and certain special characters.")]
            public string StreetName { get; set; }

            [Required]
            [MinLength(1)]
            [MaxLength(64)]
            [RegularExpression(@"^([a-zA-Z0-9 \.&'-]+)$", ErrorMessage = "This must be letters, numbers, and certain special characters.")]
            public string City { get; set; }

            public string State { get; set; }

            [Required]
            [Range(10000, 99999)]
            [Display(Name = "Zip Code")]
            public int Zip { get; set; }
        }
    }
}
