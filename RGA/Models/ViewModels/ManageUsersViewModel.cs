using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGA.Models
{
    public class ManageUsersViewModel : RegisterViewModel
    {
        private ApplicationDbContext dbContext = ApplicationDbContext.Create();


        public ManageUsersViewModel()
        {
            var users = dbContext.Users;

            
            var selectListItem = new List<User>();

// ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var user in users)
            {
                if(user.Role.Name=="Kierowca")
                    selectListItem.Add(user);
            }

            DriversSelectList = new MultiSelectList(selectListItem);
        }

        public IEnumerable<User> users
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

        [Display(Name = "Przydzieleni kierowcy")]
        public ICollection<User> Drivers { get; set; }


        public SelectList RolesSelectList =
            new SelectList(
                new List<SelectListItem>()
                {
                    new SelectListItem() {Value = "Kierowca", Text = "Kierowca"},
                    new SelectListItem() {Value = "Pracownik", Text = "Pracownik"},
                    new SelectListItem() {Value = "Admin", Text = "Admin"}
                }, "Value", "Text");


        public MultiSelectList DriversSelectList { get; set; }
    }
}