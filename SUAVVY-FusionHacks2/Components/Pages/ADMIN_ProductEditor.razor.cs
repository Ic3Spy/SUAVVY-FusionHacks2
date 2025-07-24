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
                    await LoadRecipe(productid.Value);
                }
            }

            await InvokeAsync(StateHasChanged);//refresh rendered page
        }

        public async Task<List<Models.Recipe>> GetRecipes()
        {
            return await DB.Recipes();
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

        public async void SaveRecipe()
        {
            var allrecipes = await DB.Recipes();

            if (string.IsNullOrWhiteSpace(Model.SelectedProduct.Recipe))
            {
                Model.Status = "danger";
                Model.StatusMessage = "Recipe name cannot be blank or only spaces!";
            }
            else if (
                allrecipes.Select(r => r.Name).ToList().Contains(Model.SelectedProduct.Recipe)
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
                await DB.SaveProduct(Model.SelectedProduct);

                //Post Changes
                //Get the stored ID after saving a new record
                allproducts = await DB.Products();
                var storedRec = (from rw in allrecipes where rw.Name == Model.SelectedProduct.Name select rw).FirstOrDefault();
                string tempImage = $"{FileSystem.AppDataDirectory}/RecipePhotos/temp.jpg";
                if (File.Exists(tempImage) && storedRec != null)
                {
                    string targetImage = $"{FileSystem.AppDataDirectory}/RecipePhotos/{storedRec.ID}.jpg";
                    File.Copy(tempImage, targetImage, overwrite: true);
                    Model.LoadedPhoto = $"/RecipePhotos/{storedRec.ID}.jpg";
                    //await InvokeAsync(StateHasChanged);
                    // Enclose with Try just incase File is not deletable at the moment
                    try { File.Delete(tempImage); } catch (Exception err) { }

                    Model.LoadedPhoto = $"/RecipePhotos/{storedRec.ID}.jpg";
                    Model.SelectedProduct.Photo = Model.LoadedPhoto;
                    Model.Status = "success";
                    Model.StatusMessage = "Recipe changes has been saved successfully!";
                    await Task.Delay(1000);
                    await DB.SaveRecipe(Model.SelectedProduct);
                    Model.Ingredients = await GetIngredients();
                    Model.CookingSteps = await GetSteps();
                }
            }
            await InvokeAsync(StateHasChanged);
        }

        public async Task LoadRecipe(int RecipeID)
        {
            var allrecipes = await DB.Recipes();
            var allingredients = await DB.RecipeIngredients();
            var allsteps = await DB.CookingSteps();
            Model.SelectedProduct = (from row in allrecipes where row.ID == RecipeID select row).FirstOrDefault();
            Model.Ingredients = (from row in allingredients where row.RecipeID == RecipeID select row).ToList();
            Model.CookingSteps = (from row in allsteps where row.StepId == RecipeID select row).ToList();
            Model.LoadedPhoto = $"/RecipePhotos/{RecipeID}.jpg";
            if (Model.SelectedProduct == null)
            {
                Model.SelectedProduct = new Recipe();
            }

            await InvokeAsync(StateHasChanged);//refresh rendered page
        }

        public async void DeleteRecipe(int Recipeid)
        {
            var selRecipe = (from row in Model.Recipes where row.ID == Recipeid select row).FirstOrDefault();
            if (selRecipe != null)
            {
                await DB.DeleteRecipe(selRecipe);
                Model.Status = "success";
                Model.StatusMessage = "Recipe has been deleted successfully!";
                //Model.Recipes = await GetRecipes();
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
