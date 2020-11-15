using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages {
    public class EditHomeModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;

        [BindProperty] public InputModel Input { get; set; }
        [BindProperty] public RecordModel Record { get; set; }

        public EditHomeModel(
            DataContext dataContext,
            AppUserService appUserService
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
        }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id is not null) {
                var record = await DataContext.Homes
                    .Include(h => h.CreatedBy)
                    .Include(h => h.ModifiedBy)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (record is not null) {
                    Input = new InputModel {
                        Id = record.Id,
                        City = record.City,
                        HouseNumber = record.HouseNumber,
                        State = record.State,
                        StreetName = record.StreetName,
                        Zip = record.Zip
                    };

                    Record = new RecordModel {
                        Address = record.Address,
                        Created = record.Created,
                        CreatedBy = record.CreatedBy?.FirstName,
                        Modified = record.Modified,
                        ModifiedBy = record.ModifiedBy?.FirstName
                    };
                }
            }

            if (Input is null) {
                return NotFound();
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
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

            var record = await DataContext.Homes.FindAsync(Input.Id);

            if (record is null) {
                return NotFound();
            }

            if (record.Address != address) {
                record.Address = address;
                record.HouseNumber = Input.HouseNumber;
                record.StreetName = Input.StreetName;
                record.City = Input.City;
                record.State = Input.State;
                record.Zip = Input.Zip;
                record.ModifiedById = appUser.Id;
                record.Modified = DateTime.Now;

                DataContext.Entry(record).State = EntityState.Modified;
            }

            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Details", new { record.Id });
        }

        public class InputModel {
            [Required]
            [HiddenInput]
            public int Id { get; set; }

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

        public class RecordModel {
            public string Address { get; set; }
            public DateTime Created { get; set; }
            public string CreatedBy { get; set; }
            public DateTime Modified { get; set; }
            public string ModifiedBy { get; set; }
        }
    }
}
