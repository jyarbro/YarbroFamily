using App.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.PreferenceCategories {
    public class IndexModel : PageModel {
        readonly DataContext DataContext;

        public IList<DetailCategoryViewModel> Categories { get; set; }
        public IList<Data.Models.AppUser> Users { get; set; }

        public IndexModel(
            DataContext dataContext
        ) {
            DataContext = dataContext;
        }

        public async Task OnGetAsync() {
            Categories = new List<DetailCategoryViewModel>();

            Users = await DataContext.AppUsers.ToListAsync();

            foreach (var category in DataContext.DetailCategories.Include(r => r.Details).OrderBy(r => r.SortOrder)) {
                var categoryViewModel = new DetailCategoryViewModel {
                    Id = category.Id,
                    Title = category.Title,
                    Details = new List<DetailViewModel>()
                };

                Categories.Add(categoryViewModel);

                foreach (var detail in category.Details.OrderBy(r => r.SortOrder)) {
                    var detailViewModel = new DetailViewModel {
                        Id = detail.Id,
                        Title = detail.Title,
                        Weights = new List<DetailWeightViewModel>()
                    };

                    categoryViewModel.Details.Add(detailViewModel);
                }
            }
        }

        public class DetailCategoryViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public IList<DetailViewModel> Details { get; set; }
        }

        public class DetailViewModel {
            public int Id { get; set; }
            public string Title { get; set; }
            public IList<DetailWeightViewModel> Weights { get; set; }
        }

        public class DetailWeightViewModel {
            public int DetailId { get; set; }
            public string UserId { get; set; }
            public int? Value { get; set; }
        }
    }
}
