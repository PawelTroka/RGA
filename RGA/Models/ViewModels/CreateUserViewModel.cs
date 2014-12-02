using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGA.Models
{
    public class CreateUserViewModel : RegisterViewModel
    {
        private readonly ApplicationDbContext dbContext = ApplicationDbContext.Create();


        public CreateUserViewModel()
        {
            SelectedDrivers = new List<string>();
            RolesSelectList =
                new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem {Value = "Kierowca", Text = "Kierowca"},
                        new SelectListItem {Value = "Pracownik", Text = "Pracownik"},
                        new SelectListItem {Value = "Admin", Text = "Admin"}
                    }, "Value", "Text");

            IDbSet<User> users = dbContext.Users;


            var selectListItem = new List<User>();

// ReSharper disable once LoopCanBeConvertedToQuery
            foreach (User user in users)
            {
                if (user.Role.Name == "Kierowca")
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


        [Required]
        [Display(Name = "Rola")]
        public string Role { get; set; }

        [Display(Name = "Przydzieleni kierowcy")]
        public List<string> SelectedDrivers { get; set; }

        public SelectList RolesSelectList { get; set; }
        public MultiSelectList DriversSelectList { get; set; }
    }
}