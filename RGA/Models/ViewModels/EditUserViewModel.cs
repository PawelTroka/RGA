using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace RGA.Models.ViewModels
{
    public class EditUserViewModel
    {
        private readonly ApplicationDbContext dbContext = ApplicationDbContext.Create();

        public EditUserViewModel()
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

        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nazwa użytkownika")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Numer telefonu")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Rola")]
        public string Role { get; set; }

        [Display(Name = "Przydzieleni kierowcy")]
        public List<string> SelectedDrivers { get; set; }


        public SelectList RolesSelectList { get; set; }
        public MultiSelectList DriversSelectList { get; set; }


        public User User
        {
            set
            {
                Username = value.UserName;
                Role = value.Role.Name;
                Email = value.Email;
                Phone = value.PhoneNumber;
                value.Drivers.ForEach(u => SelectedDrivers.Add(u.UserName));
                UserId = value.Id;
            }
        }
    }
}