using App.Data;
using App.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Areas.Admin.Pages {
    public class UsersModel : PageModel {
        readonly DataContext DataContext;

        public IList<AppUser> Users { get; set; }

        public UsersModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task OnGet() {
            Users = await DataContext.AppUsers.ToListAsync();
        }
    }
}
