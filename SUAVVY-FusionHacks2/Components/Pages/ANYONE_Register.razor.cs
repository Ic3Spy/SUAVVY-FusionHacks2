using Microsoft.AspNetCore.Components;
using SUAVVY_FusionHacks2.Data;
using SUAVVY_FusionHacks2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Components.Pages
{
    public partial class ANYONE_Register
    {
        public string Status = "";
        public string StatusMessage = "";


        /// <summary>
        /// This injects a preloaded copy of NavigationManager from MauiProgram.cs
        /// </summary>
        [Inject]
        public NavigationManager Nav { set; get; }

        /// <summary>
        /// This injects a preloaded copy of DatabaseContext from MauiProgram.cs
        /// </summary>
        [Inject]
        public DatabaseContext DB { set; get; }

        /// <summary>
        /// User model bound to the page
        /// </summary>
        public Models.User Model = new Models.User();
        public UsersViewModel UserModel = new UsersViewModel();
        public async void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(Model.Username) || string.IsNullOrWhiteSpace(Model.Password))
            {
                Status = "danger";
                StatusMessage = "Username/Password cannot be blank or only spaces!";
            }
            else if (Model.Password.Length < 6)
            {
                Status = "danger";
                StatusMessage = "Password must be at least 6 characters long!";
            }
            else if (Model.Username.Length < 3)
            {
                Status = "danger";
                StatusMessage = "Username must be at least 3 characters long!";
            }
            else if (
                UserModel.Users.Select(r => r.Username).ToList().Contains(Model.Username))
            {
                UserModel.Status = "danger";
                UserModel.StatusMessage = "User already exists!";
            }
            else if (Model.Username == "admin")
            {
                Model.IsDeleted = false;
                Model.CreatedBy = "SYSTEM";
                Model.ModifiedBy = "SYSTEM";
                Model.CreatedDate = DateTime.Now;
                Model.ModifiedDate = DateTime.Now;

                await DB.SaveUser(Model);
                Status = "success";
                StatusMessage = "User changes has been saved successfully!";
            }
            else
            {
                Model.IsDeleted = false;
                Model.CreatedBy = "SYSTEM";
                Model.ModifiedBy = "SYSTEM";
                Model.CreatedDate = DateTime.Now;
                Model.ModifiedDate = DateTime.Now;

                await DB.SaveUser(Model);
                Status = "success";
                StatusMessage = "User changes has been saved successfully!";
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}
