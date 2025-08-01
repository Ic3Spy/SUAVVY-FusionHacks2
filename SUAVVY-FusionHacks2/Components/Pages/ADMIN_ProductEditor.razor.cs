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
    public partial class ADMIN_ProductEditor : ComponentBase
    {
        [Inject]
        public AppShellContext AppShell { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? productid { get; set; }
        
        public RecipesViewModel RecipeModel { get; set; }
        public ProductViewModel Model { get; set; }
        protected override async void OnInitialized()
        {
            Model = new ProductViewModel();
            RecipeModel = new RecipesViewModel();
            Model.IsNew = !productid.HasValue;
            RecipeModel.Recipes = await GetRecipes();

            if (Model.IsNew)
            {
                Model.SelectedProduct = new Product();
                RecipeModel.Recipes = await GetRecipes();
                //loader
            }
            else
            {
                if (productid != null)
                {
                    await LoadProduct(productid.Value);
                }
            }

            await InvokeAsync(StateHasChanged);//refresh rendered page
        }

        public async Task<List<Models.Recipe>> GetRecipes()
        {
            return await DB.Recipes();
        }
        public async Task<List<Models.Product>> GetProducts()
        {
            return await DB.Products();
        }
        public async void AddRecipePhoto(int RecipeID)
        {
            string folderPath = Path.Combine(FileSystem.AppDataDirectory, "ProductPhotos");
            string retFile = await DeviceUtilities.AddPhoto(folderPath, $"temp.jpg");

            if (!string.IsNullOrWhiteSpace(retFile))
            {
                string filenameOnly = Path.GetFileName(retFile);
                Model.LoadedPhoto = $"/ProductPhotos/{filenameOnly}";
                await InvokeAsync(StateHasChanged);//refresh rendered page
            }
        }

        public async void SaveProduct()
        {
            var allproducts = await DB.Products();

            if (string.IsNullOrWhiteSpace(Model.SelectedProduct.Recipe))
            {
                Model.Status = "danger";
                Model.StatusMessage = "Recipe name cannot be blank or only spaces!";
            }
            else if (
                allproducts.Select(r => r.Recipe).ToList().Contains(Model.SelectedProduct.Recipe)
                &&
                Model.IsNew)
            {
                Model.Status = "danger";
                Model.StatusMessage = "Recipe already exists!";
            }
            else
            {
                //Set loading gif to not locked image
                Model.LoadedPhoto = $"/imgs/loading.gif";
                //var tes1 = Model.SelectedProduct.ID;
                await DB.SaveProduct(Model.SelectedProduct);

                //Post Changes
                //Get the stored ID after saving a new record
                allproducts = await DB.Products();
                var storedRec = (from rw in allproducts where rw.ID == Model.SelectedProduct.ID select rw).FirstOrDefault();
                string tempImage = $"{FileSystem.AppDataDirectory}/ProductPhotos/temp.jpg";
                if (File.Exists(tempImage) && storedRec != null)
                {
                    string targetImage = $"{FileSystem.AppDataDirectory}/ProductPhotos/{storedRec.ID}.jpg";
                    File.Copy(tempImage, targetImage, overwrite: true);
                    Model.LoadedPhoto = $"/ProductPhotos/{storedRec.ID}.jpg";
                    //await InvokeAsync(StateHasChanged);
                    // Enclose with Try just incase File is not deletable at the moment
                    try { File.Delete(tempImage); } catch (Exception err) { }
                    //var test = Model.SelectedProduct.ID;
                    Model.LoadedPhoto = $"/ProductPhotos/{storedRec.ID}.jpg";
                    
                    Model.SelectedProduct.ImageUrl = Model.LoadedPhoto;
                    Model.Status = "success";
                    Model.StatusMessage = "Recipe changes has been saved successfully!";
                    await Task.Delay(1000);
                    await DB.SaveProduct(Model.SelectedProduct);
                    RecipeModel.Recipes = await DB.Recipes();
                }
            }
            await InvokeAsync(StateHasChanged);
        }

        public async Task LoadProduct(int ProductID)
        {
            var allproducts = await DB.Products();
            var allrecipes = await GetRecipes();
            Model.SelectedProduct = (from row in allproducts where row.ID == ProductID select row).FirstOrDefault();
            RecipeModel.Recipes = (from row in allrecipes select row).ToList();
            
            Model.LoadedPhoto = $"/ProductPhotos/{ProductID}.jpg";
            if (Model.SelectedProduct == null)
            {
                Model.SelectedProduct = new Product();
            }

            await InvokeAsync(StateHasChanged);//refresh rendered page
        }

        public async void DeleteRecipe(int ProductID)
        {
            var selProduct= (from row in Model.Products where row.ID == ProductID select row).FirstOrDefault();
            if (selProduct != null)
            {
                await DB.DeleteProduct(selProduct);
                Model.Status = "success";
                Model.StatusMessage = "Product has been deleted successfully!";
                //Model.Recipes = await GetRecipes();
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
