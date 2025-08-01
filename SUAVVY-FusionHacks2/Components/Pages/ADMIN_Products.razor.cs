using Microsoft.AspNetCore.Components;
using SUAVVY_FusionHacks2.Data;
using SUAVVY_FusionHacks2.Models;
using SUAVVY_FusionHacks2.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Components.Pages
{
    public partial class ADMIN_Products : ComponentBase
    {
        [Inject]
        public AppShellContext AppShell { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        public ProductViewModel Model { get; set; }
        public RecipesViewModel RecipesModel { get; set; }

        protected override async void OnInitialized()
        {
            Model = new ProductViewModel();
            Model.Products = await GetProducts();
            Model.IsGrid = true; //default to grid view
            Model.ShowForm = false; //hide form by default
            Model.Status = ""; //clear status message
            Model.StatusMessage = ""; //clear status message
            RecipesModel = new RecipesViewModel();
            RecipesModel.Recipes = await DB.Recipes();

            await InvokeAsync(StateHasChanged);//refresh rendered page
            //return Task.Delay(0);
        }

        public async Task<List<Product>> GetProducts()
        {
            return await DB.Products();
        }

        public async void SaveRecipe()
        {
            if (string.IsNullOrWhiteSpace(Model.SelectedProduct.Recipe))
            {
                Model.Status = "danger";
                Model.StatusMessage = "Recipe name cannot be blank or only spaces!";
            }
            else if (
                Model.Products.Select(r => r.Recipe).ToList().Contains(Model.SelectedProduct.Recipe)
                &&
                Model.IsNew)
            {
                Model.Status = "danger";
                Model.StatusMessage = "Product already exists!";
            }
            else
            {
                //Model.SelectedRecipe.SKU = string.IsNullOrWhiteSpace(Model.SelectedRecipe.SKU) ? DateTime.Now.Ticks.ToString() : Model.SelectedRecipe.SKU;
                await DB.SaveProduct(Model.SelectedProduct);
                Model.ShowForm = false;
                Model.Status = "success";
                Model.StatusMessage = "Recipe changes has been saved successfully!";
                Model.Products = await GetProducts();
            }
            await InvokeAsync(StateHasChanged);
        }

        public async void LoadProduct(int ProductID)
        {
            Nav.NavigateTo("/ADMIN_ProductEditor?productid=" + ProductID);
        }

        public async void DeleteProduct(int Productid)
        {
            var selProduct = (from row in Model.Products where row.ID == Productid select row).FirstOrDefault();
            if (selProduct != null)
            {
                await DB.DeleteProduct(selProduct);
                Model.Status = "success";
                Model.StatusMessage = "Product has been deleted successfully!";
                Model.Products = await GetProducts();
                await InvokeAsync(StateHasChanged);
            }
        }

        public void AddProduct()
        {
            Model.StatusMessage = ""; //clear alert
            Model.SelectedProduct = new Models.Product();
            Model.IsNew = true;
            //ShowRecipeForm();
            Nav.NavigateTo("/ADMIN_ProductEditor?IsNew=true");
        }

        public void ShowRecipeForm()
        {
            //Model.ShowForm = true;
            Nav.NavigateTo("/ADMIN_ProductEditor");
        }

        public string GetIconFromCategory(string cat)
        {
            string resp = "";
            switch (cat)
            {
                case "Hamburgers":
                    resp = "fa-hamburger";
                    break;
                case "Pizza":
                    resp = "fa-pizza-slice";
                    break;
                case "Hotdogs":
                    resp = "fa-hotdog";
                    break;
                case "Cookies":
                    resp = "fa-cookie-bite";
                    break;
                case "IceCream":
                    resp = "fa-ice-cream";
                    break;
            }
            return resp;
        }

        public async void SearchTerm(ChangeEventArgs e)
        {
            var items = await GetProducts();
            string searchTerm = e.Value.ToString().ToLower();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {

                var searchResults = (from row in items
                                     where row.Recipe.ToLower().Contains(searchTerm)
                                     select row).ToList();
                Model.Products = searchResults;
            }
            else
            {
                Model.Products = items;
            }

            await InvokeAsync(StateHasChanged);//refresh rendered page
        }

        public async void AddProductPhoto(int ProductID)
        {
            string folderPath = Path.Combine(FileSystem.AppDataDirectory, "ProductPhotos");
            string retFile = await DeviceUtilities.AddPhoto(folderPath, $"{ProductID}.jpg");

            if (!string.IsNullOrWhiteSpace(retFile))
            {
                string filenameOnly = Path.GetFileName(retFile);
                //Model.LoadedPhotoPath = $"/RecipePhotos/{filenameOnly}";
                await InvokeAsync(StateHasChanged);//refresh rendered page
            }
        }
        public async void ShowList()
        {
            Model.IsGrid = false;
            await InvokeAsync(StateHasChanged);//refresh rendered page
        }
        public async void ShowGrid()
        {
            Model.IsGrid = true;
            await InvokeAsync(StateHasChanged);//refresh rendered page
        }
    }
}
