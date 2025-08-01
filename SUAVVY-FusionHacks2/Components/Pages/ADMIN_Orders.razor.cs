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
    public partial class ADMIN_Orders
    {
        [Inject]
        public AppShellContext AppShell { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        public CartViewModel Model { get; set; }

        protected override async void OnInitialized()
        {
            Model = new CartViewModel();
            Model.Orders = await GetCarts();
            Model.Status = ""; //clear status message
            Model.StatusMessage = ""; //clear status message
            await InvokeAsync(StateHasChanged);//refresh rendered page
            //return Task.Delay(0);
        }

        public async Task<List<Cart>> GetCarts()
        {
            return await DB.Carts();
        }

        public async void LoadCart(int CartID)
        {
            Nav.NavigateTo("/ADMIN_CartView?cartid=" + CartID);
        }

    }
}
