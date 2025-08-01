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
    public partial class ADMIN_CartView : ComponentBase
    {
        [Inject]
        public AppShellContext AppShell { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? cartid { get; set; }
        public CartViewModel Model { get; set; }

        public const double DeliveryFee = 10.00;
        /// <summary>
        /// This will be called on load or start of a page
        /// </summary>
        /// 
        public async Task<List<CartItem>> GetCartItems()
        {
            return await DB.CartItems();
        }
        protected override async void OnInitialized()
        {
            Model = new CartViewModel();
            Model.Items4Checkout = new List<CartItemViewModel>();
            Model.IsNew = !cartid.HasValue;

            if (cartid != null)
            {
                await LoadCartItems(cartid.Value);
            }

            await InvokeAsync(StateHasChanged);//refresh rendered page
        }
        public async Task LoadCartItems(int cartid)
        {
            
            if (cartid != null)
            {
                
                List<Cart> orders = await DB.Carts();
                List<CartItem> orderedItems = await DB.CartItems();
                List<Product> allproducts = await DB.Products();
                Cart? order = null;
                if (orders != null)
                {
                    order = (from r in orders
                             where r.ID == cartid
                             && !r.IsPaid
                             && !r.IsCompleted
                             select r
                         ).FirstOrDefault();
                    if (order != null)
                    {
                        Model.Order = order;
                        var ordereditems = (from r in orderedItems
                                            where r.CartID == cartid
                                            select r).ToList();
                        foreach (var row in ordereditems)
                        {
                            var refProd = (from r in allproducts
                                           where r.ID == row.ProductID
                                           select r).FirstOrDefault();
                            CartItemViewModel transferObject = new CartItemViewModel
                            {
                                CartID = row.CartID,
                                ID = row.ProductID,
                                Quantity = row.Quantity,
                                IsDeleted = row.IsDeleted,
                                Recipe = refProd.Recipe,
                                Price = refProd.Price,
                                Category = refProd.Category,
                                Description = refProd.Description,
                            };

                            Model.FullPrice += refProd.Price;
                            Model.Items4Checkout.Add(transferObject);
                        }
                        Model.Order.Total = Model.FullPrice + DeliveryFee;


                    }
                }
                await InvokeAsync(StateHasChanged);
            }
            else // go to login if no user available
            {
                Nav.NavigateTo($"/login?returnto=home");
            }
        }
    }
}
