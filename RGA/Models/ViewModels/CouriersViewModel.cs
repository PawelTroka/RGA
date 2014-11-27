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
        private ApplicationUser user;

        public ApplicationUser SelectedDriver { get; set; }

        public CouriersViewModel()
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            user = userManager.FindByNameAsync(HttpContext.Current.User.Identity.Name).Result;
        }

        public IEnumerable<ApplicationUser> drivers
        {
            get { return user.Divers; }
        }
    }
}