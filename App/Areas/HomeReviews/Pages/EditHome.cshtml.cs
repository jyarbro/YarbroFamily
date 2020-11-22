using App.Areas.HomeReviews.Models.Input;
using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class EditHomeModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        [BindProperty] public Home Input { get; set; }
        [BindProperty] public Data.Models.HomeReviewHome Record { get; set; }

        public EditHomeModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                var record = await DataContext.HomeReviewHomes
                    .Include(h => h.CreatedBy)
                    .Include(h => h.ModifiedBy)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (record is not null) {
                    Input = new Home {
                        Id = record.Id,
                        City = record.City,
                        HouseNumber = record.HouseNumber,
                        State = record.State,
                        StreetName = record.StreetName,
                        Zip = record.Zip,
                        Cost = record.Cost,
                        Space = record.Space,
                        Bedrooms = record.Bedrooms,
                        Bathrooms = record.Bathrooms
                    };

                    Record = record;
                }
            }

            if (Input is null) {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            Input.HouseNumber = Input.HouseNumber.Trim();
            Input.StreetName = Input.StreetName.Trim();
            Input.City = Input.City.Trim();
            Input.State = Input.State.Trim();

            var address = $"{Input.HouseNumber} {Input.StreetName}, {Input.City}, {Input.State} {Input.Zip}";
            var appUser = await AppUsers.Get(User);

            var record = await DataContext.HomeReviewHomes.FindAsync(Input.Id);

            if (record is null) {
                return NotFound();
            }

            record.ModifiedById = appUser.Id;
            record.Modified = DateTime.Now;

            record.Address = address;
            record.HouseNumber = Input.HouseNumber;
            record.StreetName = Input.StreetName;
            record.City = Input.City;
            record.State = Input.State;
            record.Zip = Input.Zip;
            record.Cost = Convert.ToInt32(Input.Cost);
            record.Space = Input.Space;
            record.Bedrooms = Input.Bedrooms;
            record.Bathrooms = Input.Bathrooms;

            DataContext.Entry(record).State = EntityState.Modified;
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./HomeDetails", new { record.Id });
        }
    }
}
