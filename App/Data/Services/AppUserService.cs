using App.Data.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Data.Services {
    public class AppUserService {
        readonly DataContext DataContext;

        public AppUserService(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<AppUser> Get(ClaimsPrincipal claimsPrincipal) {
            var user = DataContext.AppUsers.Find(claimsPrincipal.Identity.Name);

            if (user is null) {
                user = new AppUser {
                    Id = claimsPrincipal.Identity.Name,
                    Name = claimsPrincipal.FindFirstValue("name"),
                    FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName),
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                DataContext.AppUsers.Add(user);
                await DataContext.SaveChangesAsync();
            }

            return user;
        }
    }
}
