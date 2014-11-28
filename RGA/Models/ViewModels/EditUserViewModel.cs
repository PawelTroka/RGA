using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RGA.Models.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            ManageUsersViewModel = new ManageUsersViewModel();
            AllDriversItems = new List<string>();
            SelectedDriversItems = new List<string>();
            var dbContext = ApplicationDbContext.Create();
            var users = dbContext.Users;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var user in users)
            {
                if (user.Role.Name == "Kierowca")
                    AllDriversItems.Add(user.UserName);
            }
        }

        public User UserModel
        {
            get
            {
                return m_UserModel;
            }
            set
            {
                m_UserModel = value;

                ManageUsersViewModel.Role = m_UserModel.Role.Name;

                if (m_UserModel.Drivers != null && m_UserModel.Drivers.Count > 0)
                {
                    ManageUsersViewModel.Drivers = m_UserModel.Drivers;
                    foreach (var driver in m_UserModel.Drivers)
                    {
                        SelectedDriversItems.Add(driver.UserName);
                    }
                }

                ManageUsersViewModel.Username = m_UserModel.UserName;
                ManageUsersViewModel.Phone = m_UserModel.PhoneNumber;
                ManageUsersViewModel.Email = m_UserModel.Email;
                ManageUsersViewModel.Password = ManageUsersViewModel.ConfirmPassword = "!Abc123!Abc123";
            }
        }


        private User m_UserModel;

        public ManageUsersViewModel ManageUsersViewModel { get; set; }
        public List<string>  SelectedDriversItems { get; set; }
        public List<string> AllDriversItems { get; set; }
    }
}