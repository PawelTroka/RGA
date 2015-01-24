using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGA.Models.ViewModels
{
    public class CalendarViewModel
    {
        private ApplicationDbContext dbContext = ApplicationDbContext.Create();

        public CalendarViewModel()
        {
            var store = new UserStore<User>(new ApplicationDbContext());
            var userManager = new UserManager<User>(store);
            User = userManager.FindByName(HttpContext.Current.User.Identity.Name);
        }


        public string WorkDatesForUser { get; set; }

        public User User { get; set; }
    }
}