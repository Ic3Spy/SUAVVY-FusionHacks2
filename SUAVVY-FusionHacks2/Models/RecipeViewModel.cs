using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUAVVY_FusionHacks2.Models
{
    public class RecipesViewModel : BaseViewModel
    {
        /// <summary>
        /// To Identify if the form is new or an update
        /// </summary>
        /// 
        public bool SelectMode { get; set; } = false;
        public bool IsGrid { get; set; } = false;
        public bool IsUSA { get; set; } = false;
        public int Servings { get; set; }
        public string LoadedPhoto { get; set; }
        public Recipe SelectedRecipe { get; set; }
        public RecipeIngredient Ingredient { get; set; }
        public CookingStep Step { get; set; }
        public List<RecipeIngredient> Ingredients { get; set; }
        public List<CookingStep> CookingSteps { get; set; }
        public List<Recipe> Recipes { get; set; }


        public RecipesViewModel()
        {
            Ingredients = new List<RecipeIngredient>();
            CookingSteps = new List<CookingStep>();
            Recipes = new List<Recipe>();
            SelectedRecipe = new Recipe();
            Ingredient = new RecipeIngredient();
            Step = new CookingStep();
            SelectMode = false;
        }
    }
}
