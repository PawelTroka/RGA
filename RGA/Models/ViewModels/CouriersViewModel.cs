using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGA.Models.ViewModels
{
    public class CouriersViewModel
    {
        private readonly User user;
        private ApplicationDbContext dbContext = ApplicationDbContext.Create();

        public CouriersViewModel()
        {
            var store = new UserStore<User>(new ApplicationDbContext());
            var userManager = new UserManager<User>(store);
            user = userManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;
        }

        public IEnumerable<User> drivers
        {
            get { return user.Drivers; }
        }
    }


    public class CourierCalendarViewModel
    {
        private readonly User user;
        private ApplicationDbContext dbContext = ApplicationDbContext.Create();

        public CourierCalendarViewModel()
        {
            var store = new UserStore<User>(new ApplicationDbContext());
            var userManager = new UserManager<User>(store);
            user = userManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;
        }


        public string WorkDatesForDriver { get; set; }

        public User SelectedDriver { get; set; }

        public IEnumerable<User> drivers
        {
            get { return user.Drivers; }
        }
    }
}