﻿using Microsoft.AspNetCore.Components;
using SUAVVY_FusionHacks2.Data;
using SUAVVY_FusionHacks2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Components.Pages
{
    public partial class USERS_Dashboard : ComponentBase
    {
        [Inject]
        public AppShellContext AppShell { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        public HomeViewModel Model { get; set; }

        /// <summary>
        /// This will be called on load or start of a page
        /// </summary>
        protected override async void OnInitialized()
        {
            Model = new HomeViewModel();

            //check logged-in user
            var loggedUser = AppShell.GetSessionUser();
            if (loggedUser != null)
            {
                AppShell.CurrentUser = loggedUser;
                AppShell.IsUserLoggedIn = true;
            }

            //temporary load up since we have to get a random list of items
            var allproducts = await DB.Products();

            allproducts.Shuffle<Product>();//Shuffle Order
            Model.Popular = (from row in allproducts where row.IsActive select row).Take(5).ToList();

            allproducts.Shuffle<Product>();//Shuffle Order
            Model.TopWeek = (from row in allproducts where row.IsActive orderby row.ID ascending select row).Take(5).ToList();

            await InvokeAsync(StateHasChanged);
        }


        public async void SearchTerm(ChangeEventArgs e)
        {

            string searchTerm = e.Value.ToString().ToLower();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Nav.NavigateTo("/USERS_Products?lookingfor=" + searchTerm);
            }
            await InvokeAsync(StateHasChanged);//refresh rendered page
        }
    }
}
