using App.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nrrdio.Utilities.Loggers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Areas.Ynab.Pages {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;

        public IList<LogEntry> LogEntries { get; set; }

        public IndexModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task OnGet() {
            LogEntries = await DataContext.YnabFeederLog.ToListAsync();
        }
    }
}
