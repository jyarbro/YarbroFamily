using App.Data;
using App.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Admin.Pages {
    public class EditSecurityRoleModel : PageModel {
        readonly DataContext DataContext;

        [BindProperty] public SecurityRole SecurityRole { get; set; }

        public IList<SelectListItem> Users { get; set; }

        public EditSecurityRoleModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task<IActionResult> OnGetAsync(int id) {
            SecurityRole = await DataContext.SecurityRoles.FindAsync(id);

            if (SecurityRole is null) {
                return NotFound();
            }

            var userIdsInRoleQuery = from o in DataContext.UserSecurityRoles
                                     where o.SecurityRoleId == SecurityRole.Id
                                     select o.UserId;

            var userIdsInRole = await userIdsInRoleQuery.ToListAsync();

            Users = new List<SelectListItem>();

            foreach (var user in DataContext.AppUsers) {
                Users.Add(new SelectListItem { Text = user.FirstName, Value = user.Id, Selected = userIdsInRole.Contains(user.Id) });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var securityRole = await DataContext.SecurityRoles.FindAsync(SecurityRole.Id);

            if (securityRole is null) {
                return NotFound();
            }

            securityRole.Title = SecurityRole.Title;
            securityRole.ModifiedById = User.Identity.Name;
            securityRole.Modified = DateTime.Now;

            HttpContext.Request.Form.TryGetValue($"selectedUsers[]", out var value);
            var values = value.ToString().Split(",");

            foreach (var user in DataContext.AppUsers) {
                var userSecurityRole = await DataContext.UserSecurityRoles.FirstOrDefaultAsync(o => o.SecurityRoleId == securityRole.Id && o.UserId == user.Id);

                if (values.Contains(user.Id) && userSecurityRole is null) {
                    DataContext.Add(new UserSecurityRole {
                        SecurityRoleId = securityRole.Id,
                        UserId = user.Id
                    });
                }
                else if (!values.Contains(user.Id) && userSecurityRole is not null) {
                    DataContext.Remove(userSecurityRole);
                }
            }

            DataContext.Entry(securityRole).State = EntityState.Modified;
            await DataContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
