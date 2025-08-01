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
    public partial class USERS_Products : ComponentBase
    {
        [Inject]
        public AppShellContext AppShell { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        public SearchViewModel Model { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string lookingfor { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string category { get; set; }
        /// <summary>
        /// This will be called on load or start of a page
        /// </summary>
        protected override async void OnInitialized()
        {
            Model = new SearchViewModel();
            Model.Search = lookingfor ?? "";
            Model.Category = category;
            await LoadProducts();

        }

        public async Task LoadProducts()
        {
            var allproducts = await DB.Products();

            if (string.IsNullOrWhiteSpace(Model.Search) && Model.Category == "all")
            {
                Model.Result = allproducts;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Model.Category))
                {
                    Model.Result = allproducts.Where(r =>
                    r.Recipe.ToLower().Contains(Model.Search.ToLower())
                    &&
                    r.Description.ToLower().Contains(Model.Search.ToLower())
                    ).ToList();
                }
                else
                {
                    Model.Result = allproducts
                        .Where(r =>
                        r.Category == Model.Category
                        )
                        .Where(r =>
                    r.Recipe.ToLower().Contains(Model.Search.ToLower())
                    &&
                    r.Description.ToLower().Contains(Model.Search.ToLower())
                    ).ToList();
                }
            }
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
