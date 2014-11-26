using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGA.Models
{
    public class ManageUsersViewModel : RegisterViewModel
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


        // [Required]
        //[DataType(DataType.)]
        [Display(Name = "Rola")]
        public string Role { get; set; }


    }
}