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
    public partial class ADMIN_RecipeEditor : ComponentBase
    {
        [Inject]
        public AppShellContext AppShell { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? recipeid { get; set; }
        public RecipesViewModel Model { get; set; }

        protected override async void OnInitialized()
        {
            Model = new RecipesViewModel();

            Model.IsNew = !recipeid.HasValue;
            Model.Ingredients = await GetIngredients();
            Model.CookingSteps = await GetSteps();

            if (Model.IsNew)
            {
                Model.SelectedRecipe = new Recipe();
                Model.Ingredient = new RecipeIngredient();
                //loader
                var allingredients = await DB.RecipeIngredients();
                var allsteps = await DB.CookingSteps();
                Model.Ingredients = (from row in allingredients where row.RecipeID == Model.SelectedRecipe.ID select row).ToList();
                Model.CookingSteps = (from row in allsteps where row.StepId == Model.SelectedRecipe.ID select row).ToList();
            }
            else
            {
                if (recipeid != null)
                {
                    await LoadRecipe(recipeid.Value);
                }
            }

            await InvokeAsync(StateHasChanged);//refresh rendered page
        }
        public async Task<List<Models.RecipeIngredient>> GetIngredients()
        {
            return await DB.RecipeIngredients();
        }
        public async Task<List<Models.CookingStep>> GetSteps()
        {
            return await DB.CookingSteps();
        }
        public void AddServings()
        {
            Model.Servings++;

        }

        public void MinusServings()
        {
            if (Model.Servings > 0)
            {
                Model.Servings--;
            }


            //removes to cart if zero quantity
            if (Model.Servings == 0)
            {
                //UI to remove to cart
            }
        }

        public async void AddRecipePhoto(int RecipeID)
        {
            string folderPath = Path.Combine(FileSystem.AppDataDirectory, "RecipePhotos");
            string retFile = await DeviceUtilities.AddPhoto(folderPath, $"temp.jpg");

            if (!string.IsNullOrWhiteSpace(retFile))
            {
                string filenameOnly = Path.GetFileName(retFile);
                Model.LoadedPhoto = $"/RecipePhotos/{filenameOnly}";
                await InvokeAsync(StateHasChanged);//refresh rendered page
            }
        }

        public async void SaveRecipe()
        {
            var allrecipes = await DB.Recipes();

            if (string.IsNullOrWhiteSpace(Model.SelectedRecipe.Name))
            {
                Model.Status = "danger";
                Model.StatusMessage = "Recipe name cannot be blank or only spaces!";
            }
            else if (
                allrecipes.Select(r => r.Name).ToList().Contains(Model.SelectedRecipe.Name)
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

                Model.SelectedRecipe.UserID = AppShell.CurrentUser.ID;
                Model.SelectedRecipe.ModifiedBy = AppShell.CurrentUser.Username;
                Model.SelectedRecipe.ModifiedDate = DateTime.Now;
                if (Model.IsNew)
                {
                    Model.SelectedRecipe.CreatedBy = AppShell.CurrentUser.Username;
                    Model.SelectedRecipe.CreatedDate = DateTime.Now;
                }

                await DB.SaveRecipe(Model.SelectedRecipe);

                //Post Changes
                //Get the stored ID after saving a new record
                allrecipes = await DB.Recipes();
                var storedRec = (from rw in allrecipes where rw.Name == Model.SelectedRecipe.Name select rw).FirstOrDefault();
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
                    Model.SelectedRecipe.Photo = Model.LoadedPhoto;
                    Model.Status = "success";
                    Model.StatusMessage = "Recipe changes has been saved successfully!";
                    await Task.Delay(1000);
                    await DB.SaveRecipe(Model.SelectedRecipe);
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
            Model.SelectedRecipe = (from row in allrecipes where row.ID == RecipeID select row).FirstOrDefault();
            Model.Ingredients = (from row in allingredients where row.RecipeID == RecipeID select row).ToList();
            Model.CookingSteps = (from row in allsteps where row.StepId == RecipeID select row).ToList();
            Model.LoadedPhoto = $"/RecipePhotos/{RecipeID}.jpg";
            if (Model.SelectedRecipe == null)
            {
                Model.SelectedRecipe = new Recipe();
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
        public async void AddIngredient()
        {
            await DB.SaveRecipe(Model.SelectedRecipe);
            Model.Ingredient.RecipeID = Model.SelectedRecipe.ID;
            await DB.SaveRecipeIngredient(Model.Ingredient);
            Model.Status = "success";
            Model.StatusMessage = "Ingredient has been added successfully!";
            await Task.Delay(1000);
            //var allrecipes = await DB.Recipes();
            var allingredients = await DB.RecipeIngredients();
            var allsteps = await DB.CookingSteps();
            //Model.SelectedRecipe = (from row in allrecipes where row.ID == Model.SelectedRecipe.ID select row).FirstOrDefault();
            Model.Ingredients = (from row in allingredients where row.RecipeID == Model.SelectedRecipe.ID select row).ToList();
            Model.CookingSteps = (from row in allsteps where row.StepId == Model.SelectedRecipe.ID select row).ToList();
            Model.Ingredient = new Models.RecipeIngredient();
            Model.IsNew = true;
            await InvokeAsync(StateHasChanged);//refresh rendered page
        }
        public async void DeleteIngredient(int id)
        {
            var selIngredient = (from row in Model.Ingredients where row.ID == id select row).FirstOrDefault();
            if (selIngredient != null)
            {
                await DB.DeleteRecipeIngredient(selIngredient);
                Model.Status = "success";
                Model.StatusMessage = "Ingredient has been deleted successfully!";
                //Model.Recipes = await GetRecipes();
                await Task.Delay(1000);
                var allingredients = await DB.RecipeIngredients();
                var allsteps = await DB.CookingSteps();
                Model.Ingredients = (from row in allingredients where row.RecipeID == Model.SelectedRecipe.ID select row).ToList();
                Model.CookingSteps = (from row in allsteps where row.StepId == Model.SelectedRecipe.ID select row).ToList();
                await InvokeAsync(StateHasChanged);

            }
        }
        public async void AddCookingStep()
        {
            await DB.SaveRecipe(Model.SelectedRecipe);
            Model.Step.StepId = Model.SelectedRecipe.ID;
            await DB.SaveCookingStep(Model.Step);
            Model.Status = "success";
            Model.StatusMessage = "Cooking step has been added successfully!";
            await Task.Delay(1000);
            //var allrecipes = await DB.Recipes();
            var allingredients = await DB.RecipeIngredients();
            var allsteps = await DB.CookingSteps();
            //Model.SelectedRecipe = (from row in allrecipes where row.ID == Model.SelectedRecipe.ID select row).FirstOrDefault();
            Model.Ingredients = (from row in allingredients where row.RecipeID == Model.SelectedRecipe.ID select row).ToList();
            Model.CookingSteps = (from row in allsteps where row.StepId == Model.SelectedRecipe.ID select row).ToList();
            Model.Step = new Models.CookingStep();
            Model.IsNew = true;
            await InvokeAsync(StateHasChanged);//refresh rendered page
        }
        public async void DeleteCookingStep(int id)
        {
            var selStep = (from row in Model.CookingSteps where row.ID == id select row).FirstOrDefault();
            if (selStep != null)
            {
                await DB.DeleteCookingStep(selStep);
                Model.Status = "success";
                Model.StatusMessage = "Cooking step has been deleted successfully!";
                //Model.Recipes = await GetRecipes();
                await Task.Delay(1000);
                var allingredients = await DB.RecipeIngredients();
                var allsteps = await DB.CookingSteps();
                Model.Ingredients = (from row in allingredients where row.RecipeID == Model.SelectedRecipe.ID select row).ToList();
                Model.CookingSteps = (from row in allsteps where row.StepId == Model.SelectedRecipe.ID select row).ToList();
                await InvokeAsync(StateHasChanged);

            }
        }
    }
}
