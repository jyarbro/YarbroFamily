using App.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Homes.Pages.Detail {
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
            
            // TODO possible optimization
            //var categories = await DataContext.DetailCategories.Include(r => r.Details).ThenInclude(r => r.Weights).ToListAsync();
            
            foreach (var category in DataContext.DetailCategories.Include(r => r.Details).OrderBy(r => r.SortOrder)) {
                var categoryViewModel = new DetailCategoryViewModel {
                    Title = category.Title,
                    Details = new List<DetailViewModel>()
                };

                Categories.Add(categoryViewModel);

                foreach (var detail in category.Details.OrderBy(r => r.SortOrder)) {
                    var detailViewModel = new DetailViewModel {
                        Title = detail.Title,
                        Weights = new List<DetailWeightViewModel>()
                    };

                    categoryViewModel.Details.Add(detailViewModel);

                    foreach (var user in Users) {
                        var detailWeight = await DataContext.DetailWeights.FirstOrDefaultAsync(r => r.UserId == user.Id && r.DetailId == detail.Id);

                        detailViewModel.Weights.Add(new DetailWeightViewModel {
                            DetailId = detail.Id,
                            UserId = user.Id,
                            Value = detailWeight?.Value
                        });
                    }
                }
            }
        }

        public class DetailCategoryViewModel {
            public string Title { get; set; }
            public IList<DetailViewModel> Details { get; set; }
        }

        public class DetailViewModel {
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
