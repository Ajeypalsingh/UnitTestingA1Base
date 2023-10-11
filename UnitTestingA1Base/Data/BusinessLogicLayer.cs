using SendGrid.Helpers.Errors.Model;
using UnitTestingA1Base.Models;

namespace UnitTestingA1Base.Data
{
    public class BusinessLogicLayer
    {
        private AppStorage _appStorage;

        public BusinessLogicLayer(AppStorage appStorage) {
            _appStorage = appStorage;
        }
        public HashSet<Recipe> GetRecipesByIngredient(int? id, string? name)
        {
            Ingredient ingredient;
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            if (id != null)
            {
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id);
                if (ingredient == null) 
                {
                    return recipes;
                }
            } else if (name != null)
            {
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Name.Contains(name));
                if (ingredient == null)
                {
                    return recipes;
                }
            } else
            {
                throw new ArgumentNullException("Either ID or Name must be provided.");
            }

            HashSet<RecipeIngredient> recipeIngredients = _appStorage.RecipeIngredients.Where(rI => rI.IngredientId == ingredient.Id).ToHashSet();

            recipes = _appStorage.Recipes.Where(r => recipeIngredients.Any(ri => ri.RecipeId == r.Id)).ToHashSet();
            
            return recipes;
        }

        public HashSet<Recipe> GetRecipesByDiet(int? id, string? name)
        {
            DietaryRestriction dietaryRestriction;
            if (id != null)
            {
                dietaryRestriction = _appStorage.DietaryRestrictions.First(d => d.Id == id);
            }
            else if (name != null)
            {
                dietaryRestriction = _appStorage.DietaryRestrictions.First(d => d.Name.Contains(name));
            }
            else
            {
                throw new ArgumentNullException("Either ID or Name must be provided.");
            }

            // Get all ingredients Ids associatted with dietery restriction
            List<int> ingredientIDs = _appStorage.IngredientRestrictions
                                                .Where(ir => ir.DietaryRestrictionId == dietaryRestriction.Id)
                                                .Select(ir => ir.IngredientId)
                                                .ToList();

            // Get all recipie Ids associatted with above ingredients
            List<int> recieIds = _appStorage.RecipeIngredients
                                            .Where(ri => ingredientIDs.Contains(ri.IngredientId))
                                            .Select(ri => ri.RecipeId)
                                            .ToList();

            // Retrieve all recipies using above recipie ids
            HashSet<Recipe> recipies = _appStorage.Recipes
                                         .Where(r => recieIds.Contains(r.Id))
                                         .ToHashSet();

            return recipies;
        }

        public HashSet<Recipe> GetAllRecipies(int? id, string? name)
        {
            HashSet<Recipe> recipies;
            if (id != null)
            {
                recipies = _appStorage.Recipes.Where(r => r.Id == id).ToHashSet();
            }
            else if (name != null)
            {
                recipies = _appStorage.Recipes.Where(r => r.Name.Contains(name)).ToHashSet();
            }
            else
            {
                throw new ArgumentException("Either ID or Name must be provided.");
            }

            return recipies;
        }

        public void AddRecipeWithIngridients(Recipe recipie, List<Ingredient> ingredients)
        {
            Recipe existingRecipe = _appStorage.Recipes.FirstOrDefault(r => r.Name == recipie.Name);

            if (existingRecipe != null)
            {
                throw new InvalidOperationException("Recipie with same name already exists");
            }

            recipie.Id = _appStorage.GeneratePrimaryKey();
            _appStorage.Recipes.Add(recipie);

            foreach (Ingredient ingriedient in ingredients)
            {
                Ingredient existingIngredients = _appStorage.Ingredients.FirstOrDefault(i => i.Id == ingriedient.Id);
                if (existingIngredients == null)
                {
                    ingriedient.Id = _appStorage.GeneratePrimaryKey();
                    _appStorage.Ingredients.Add(ingriedient);

                    _appStorage.RecipeIngredients.Add(new RecipeIngredient { RecipeId = recipie.Id, IngredientId = ingriedient.Id });
                } else
                {
                    _appStorage.RecipeIngredients.Add(new RecipeIngredient { RecipeId = recipie.Id, IngredientId = existingIngredients.Id });
                }

            }
        }

        public void DeleteIngredient(int? id, string? name)
        {
            Ingredient ingredientToDelete;
            if (id != null)
            {
                ingredientToDelete = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id);
            }
            else if (name != null)
            {
                ingredientToDelete = _appStorage.Ingredients.FirstOrDefault(i => i.Name.Contains(name));
            }
            else
            {
                throw new ArgumentException("Either ID or Name must be provided.");
            }

            List<RecipeIngredient> associatedRecipes = _appStorage.RecipeIngredients.Where(ri => ri.IngredientId == ingredientToDelete.Id).ToList();
            if (associatedRecipes.Count > 1)
            {
                throw new ForbiddenException("Multiple recipes use this ingredient. Cannot delete.");
            }
            else if (associatedRecipes.Count == 1)
            {
                _appStorage.Recipes.Remove(_appStorage.Recipes.FirstOrDefault(r => r.Id == associatedRecipes[0].RecipeId));
                _appStorage.RecipeIngredients.Remove(_appStorage.RecipeIngredients.FirstOrDefault(ri => ri.RecipeId == associatedRecipes[0].RecipeId));
            }

            _appStorage.Ingredients.Remove(ingredientToDelete);
        }

        public void DeleteRecipe(int? id, string name)
        {
            Recipe recipeToDelete = null;

            if (id != null)
            {
                recipeToDelete = _appStorage.Recipes.FirstOrDefault(r => r.Id == id);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                recipeToDelete = _appStorage.Recipes.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                throw new ArgumentException("Either ID or Name must be provided.");
            }

            // Remove the RecipeIngredient associations first
            List<RecipeIngredient> recipeIngredientsToDelete = _appStorage.RecipeIngredients.Where(ri => ri.RecipeId == recipeToDelete.Id).ToList();
            foreach (RecipeIngredient ri in recipeIngredientsToDelete)
            {
                _appStorage.RecipeIngredients.Remove(ri);
            }

            // Now, remove the recipe itself
            _appStorage.Recipes.Remove(recipeToDelete);
        }
    }
}
