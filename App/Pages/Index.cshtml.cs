using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace App.Pages {
    public class IndexModel : PageModel {
        readonly ILogger<IndexModel> Logger;

        public IndexModel(
            ILogger<IndexModel> logger
        ) {
            Logger = logger;
        }

        public void OnGet() {
        }
    }
}
