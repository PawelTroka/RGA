using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGA.Models
{
    public class ManageUsersViewModel
    {
        private ApplicationDbContext dbContext = ApplicationDbContext.Create();

        public IEnumerable<ApplicationUser> users
        {
            get { return dbContext.Users; }
        }

        public IEnumerable<IdentityRole> roles
        {
            get { return dbContext.Roles.OrderBy(x => x.Name); }
        }
 
    }
}