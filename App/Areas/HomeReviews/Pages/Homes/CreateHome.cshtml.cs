using App.Data;
using App.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nrrdio.Utilities.Web;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Homes {
    using InputModels = HomeReviews.Homes.InputModels;

    public class CreateHomeModel : PageModel {
        readonly DataContext DataContext;
        readonly AppUserService AppUsers;
        readonly GzipWebClient WebClient;

        [BindProperty(SupportsGet = true)] public InputModels.Home Input { get; set; }

        public bool Confirmed { get; set; }

        public CreateHomeModel(
            DataContext dataContext,
            AppUserService appUserService,
            GzipWebClient webClient
        ) {
            DataContext = dataContext;
            AppUsers = appUserService;
            WebClient = webClient;
        }

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnGetConfirm() {
            Confirmed = true;

            var isUri = Uri.TryCreate(Input.Address, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttps);

            if (isUri) {
                if (uri is { Host: "www.zillow.com" } or { Host: "zillow.com" }) {
                    await UpdateFromZillow(uri.AbsoluteUri);
                }
            }

            if (Input.Address?.Length > 0) {
                var parts = Input.Address.Split(",");

                if (parts.Length > 0) {
                    var subParts = parts[0].Trim().Split(" ").ToList();

                    if (subParts.Count > 1) {
                        Input.HouseNumber = subParts[0].Trim();
                        subParts.RemoveAt(0);

                        Input.StreetName = string.Join(" ", subParts).Trim();
                    }
                }
                else {
                    ModelState.AddModelError(nameof(Input.Address), "This doesn't appear to be a valid address or URL.");
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
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmedAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            Input.Address = $"{Input.HouseNumber} {Input.StreetName}, {Input.City}, {Input.State} {Input.Zip}";

            var record = await DataContext.HomeReviewHomes.FirstOrDefaultAsync(r => r.Address == Input.Address);

            if (record is not null) {
                return RedirectToPage("./HomeDetails", new { record.Id });
            }

            var appUser = await AppUsers.Get(User);

            record = new Data.Models.HomeReviewHome {
                CreatedById = appUser.Id,
                Created = DateTime.Now,
                ModifiedById = appUser.Id,
                Modified = DateTime.Now,
                Address = Input.Address,
                HouseNumber = Input.HouseNumber,
                StreetName = Input.StreetName,
                City = Input.City,
                State = Input.State,
                Zip = Input.Zip,
                Cost = Convert.ToInt32(Input.Cost),
                Space = Input.Space,
                Bedrooms = Input.Bedrooms,
                Bathrooms = Input.Bathrooms,
                Available = true
            };

            DataContext.HomeReviewHomes.Add(record);
            await DataContext.SaveChangesAsync();

            if (Input.Url?.Length > 0) {
                DataContext.Add(new Data.Models.HomeReviewLink {
                    CreatedById = appUser.Id,
                    Created = DateTime.Now,
                    ModifiedById = appUser.Id,
                    Modified = DateTime.Now,
                    HomeId = record.Id,
                    Link = Input.Url
                });

                await DataContext.SaveChangesAsync();
            }

            return RedirectToPage("./HomeDetails", new { record.Id });
        }

        async Task UpdateFromZillow(string url) {
            var document = await WebClient.DownloadDocument(url);

            Input.Url = url;

            Input.Address = document.DocumentNode.SelectSingleNode(@"//meta[@property='og:zillow_fb:address']")?.Attributes["content"]?.Value.Trim();

            if (Input.Address is not { Length: > 0 }) {
                ModelState.AddModelError("", "This doesn't look like a Zillow house. Maybe it's apartments? Please manually enter the details.");
                return;
            }

            var bathrooms = document.DocumentNode.SelectSingleNode(@"//meta[@property='zillow_fb:baths']")?.Attributes["content"]?.Value.Trim();

            if (bathrooms?.Length > 0) {
                Input.Bathrooms = Convert.ToSingle(bathrooms);
            }

            var bedrooms = document.DocumentNode.SelectSingleNode(@"//meta[@property='zillow_fb:beds']")?.Attributes["content"]?.Value.Trim();

            if (bedrooms?.Length > 0) {
                Input.Bedrooms = Convert.ToSingle(bedrooms);
            }

            var cost = document.DocumentNode.SelectSingleNode(@"//meta[@property='product:price:amount']")?.Attributes["content"]?.Value.Trim();

            if (cost?.Length > 0) {
                Input.Cost = Convert.ToSingle(cost);
            }

            var description = document.DocumentNode.SelectSingleNode(@"//meta[@name='description']")?.Attributes["content"]?.Value.Trim();

            if (description?.Length > 0) {
                var spaceMatch = Regex.Match(description, @"\s([\d\,\.]+)\ssq\.\sft\.");

                if (spaceMatch.Success) {
                    var convertedNumber = float.Parse(spaceMatch.Groups[1].Value, CultureInfo.InvariantCulture.NumberFormat);
                    Input.Space = Convert.ToInt32(convertedNumber);
                }
            }
        }
    }
}
