using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGA.Models.ViewModels
{
    public class CouriersViewModel
    {
        private ApplicationDbContext dbContext = ApplicationDbContext.Create();
        private User user;

        public User SelectedDriver { get; set; }

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
}