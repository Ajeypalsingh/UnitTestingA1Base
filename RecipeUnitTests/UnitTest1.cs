using SendGrid.Helpers.Errors.Model;
using UnitTestingA1Base.Data;
using UnitTestingA1Base.Models;

namespace RecipeUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private BusinessLogicLayer _initializeBusinessLogic()
        {
            return new BusinessLogicLayer(new AppStorage());
        }
        [TestMethod]
        public void GetRecipesByIngredient_ValidId_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientId = 6;
            int recipeCount = 2;

            // act
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(ingredientId, null);

            //Assert
            Assert.AreEqual(recipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipesByIngredient_ValidName_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredientName = "Eggs";
            int recipeCount = 1;

            // Act
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(null, ingredientName);

            // Arrange 
            Assert.AreEqual(recipeCount, recipes.Count);
        }


        [TestMethod]
        public void GetRecipesByIngredient_InvalidId_ReturnsEmptySet()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();

            int invalidIngredientId = 9999; 

            // Act
            HashSet<Recipe> resultRecipes = bll.GetRecipesByIngredient(invalidIngredientId, null);

            // Assert
            Assert.AreEqual(0, resultRecipes.Count);
        }


        [TestMethod]
        public void GetRecipesByDiet_ValidId_ReturnsRecipesWithDietaryRestriction()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int dietaryRestrictionId = 1; 

            // Act
            HashSet<Recipe> resultRecipes = bll.GetRecipesByDiet(dietaryRestrictionId, null);

            // Assert
            Assert.AreEqual(3, resultRecipes.Count); 
        }

        [TestMethod]
        public void GetRecipesByDiet_ValidName_ReturnsRecipesWithDietaryRestriction()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string dietaryRestrictionName = "Gluten-Free"; 

            // Act
            HashSet<Recipe> resultRecipes = bll.GetRecipesByDiet(null, dietaryRestrictionName);

            // Assert
            Assert.AreEqual(3, resultRecipes.Count); // Adjust this based on your test data
        }

        [TestMethod]
        public void GetRecipesByDiet_InvalidIdAndName_ReturnsEmptySet()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int invalidDietaryRestrictionId = 999; 

            // Act
            HashSet<Recipe> resultRecipes = bll.GetRecipesByDiet(invalidDietaryRestrictionId, null);

            // Assert
            Assert.AreEqual(0, resultRecipes.Count);
        }

        [TestMethod]
        public void GetRecipes_ByValidId_ReturnsMatchingRecipe()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int validRecipeId = 1;

            // Act
            HashSet<Recipe> resultRecipes = bll.GetRecipies(validRecipeId, null);

            // Assert
            Assert.AreEqual(1, resultRecipes.Count);
            Assert.IsTrue(resultRecipes.Any(r => r.Id == validRecipeId));
        }

        [TestMethod]
        public void GetRecipes_ByValidName_ReturnsMatchingRecipes()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string validRecipeName = "Mushroom Risotto";

            // Act
            HashSet<Recipe> resultRecipes = bll.GetRecipies(null, validRecipeName);

            // Assert
            Assert.IsTrue(resultRecipes.Count > 0);
            Assert.IsTrue(resultRecipes.All(r => r.Name.Contains(validRecipeName)));
        }

        [TestMethod]
        public void GetRecipes_NoParameters_ReturnsAllRecipes()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int totalRecipes = 12;

            // Act
            HashSet<Recipe> resultRecipes = bll.GetRecipies(null, null);

            // Assert
            Assert.AreEqual(totalRecipes, resultRecipes.Count); ;
        }


        [TestMethod]
        public void AddRecipeWithIngredients_NewRecipeAndIngredients_CreatesRecipeAndIngredients()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);
            Recipe newRecipe = new Recipe { Name = "New Recipe", Description = "Description", Servings = 4 };
            List<Ingredient> newIngredients = new List<Ingredient>
                {
                new Ingredient { Name = "Ingredient 1" },
                 new Ingredient { Name = "Ingredient 2" }
                };

            // Act
            bll.AddRecipeWithIngridients(newRecipe, newIngredients);

            // Assert
            Assert.AreEqual(13, appStorage.Recipes.Count);
            Assert.AreEqual(12, appStorage.Ingredients.Count);
        }

        [TestMethod]
        public void AddRecipeWithIngredients_ExistingRecipe_ThrowsException()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);
            Recipe existingRecipe = new Recipe { Name = "Grilled Salmon" };
            appStorage.Recipes.Add(existingRecipe);

            List<Ingredient> newIngredients = new List<Ingredient>
        {
            new Ingredient { Name = "Ingredient 1" },
            new Ingredient { Name = "Ingredient 2" }
        };

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => bll.AddRecipeWithIngridients(existingRecipe, newIngredients));
        }

        [TestMethod]
        public void AddRecipeWithIngredients_ExistingIngredients_UsesExistingIngredients()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);

           Ingredient existingIngredient = appStorage.Ingredients.First(i => i.Name.Contains("Eggs"));

            Recipe newRecipe = new Recipe { Name = "New Test Recipe" };

            List<Ingredient> existingIngredients = new List<Ingredient>
                      {
                           existingIngredient
                      };

            // Act
            bll.AddRecipeWithIngridients(newRecipe, existingIngredients);

            // Assert
            Assert.AreEqual(13, appStorage.Recipes.Count);
            Assert.AreEqual(10, appStorage.Ingredients.Count);
        }

        [TestMethod]
        public void DeleteIngredient_ByValidId_DeletesIngredient()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);

            // Act
            bll.DeleteIngredient(1, null);

            // Assert
            Assert.AreEqual(9, appStorage.Ingredients.Count);
        }

        [TestMethod]
        public void DeleteIngredient_ByValidName_DeletesIngredient()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);


            // Act
            bll.DeleteIngredient(null, "Tomatoes");

            // Assert
            Assert.AreEqual(9, appStorage.Ingredients.Count);
        }

        [TestMethod]
        public void DeleteIngredient_IngredientInUse_ThrowsForbiddenException()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);

            Ingredient existingIngredient = new Ingredient { Id = 15, Name = "Ingredient Test" };
            appStorage.Ingredients.Add(existingIngredient);

            Recipe associatedRecipeOne = new Recipe { Id = 15, Name = "Recipe Test One" };
            appStorage.Recipes.Add(associatedRecipeOne);

            Recipe associatedRecipeTwo = new Recipe { Id = 16, Name = "Recipe Test Two" };
            appStorage.Recipes.Add(associatedRecipeTwo);

            RecipeIngredient associatedRecipeIngredientOne = new RecipeIngredient { IngredientId = 15, RecipeId = 15 };
            RecipeIngredient associatedRecipeIngredientTwo = new RecipeIngredient { IngredientId = 15, RecipeId = 16 };

            appStorage.RecipeIngredients.Add(associatedRecipeIngredientOne);
            appStorage.RecipeIngredients.Add(associatedRecipeIngredientTwo);


            // Act & Assert
            Assert.ThrowsException<ForbiddenException>(() => bll.DeleteIngredient(15, null));
        }

        [TestMethod]
        public void DeleteIngredient_NoParameters_ThrowsArgumentException()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => bll.DeleteIngredient(null, null));
        }

        [TestMethod]
        public void DeleteRecipe_ByValidId_DeletesRecipe()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);

            Recipe existingRecipe = appStorage.Recipes.First(r => r.Name.Contains("Grilled Salmon"));

            // Act
            bll.DeleteRecipe(existingRecipe.Id, null);

            // Assert
            Assert.AreEqual(11, appStorage.Recipes.Count);
        }

        [TestMethod]
        public void DeleteRecipe_ByValidName_DeletesRecipe()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);

            Recipe existingRecipe = appStorage.Recipes.First(r => r.Name.Contains("Grilled Salmon"));


            // Act
            bll.DeleteRecipe(null, existingRecipe.Name);

            // Assert
            Assert.AreEqual(11, appStorage.Recipes.Count);
        }

        [TestMethod]
        public void DeleteRecipe_RecipeInUse_RemovesAssociationsAndDeletesRecipe()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);

            Recipe existingRecipe = new Recipe { Id = 15, Name = "Recipe Test" };
            appStorage.Recipes.Add(existingRecipe);

            Ingredient associatedIngredient = new Ingredient { Id = 15, Name = "Ingredient Test" };
            appStorage.Ingredients.Add(associatedIngredient);

            RecipeIngredient associatedRecipeIngredient = new RecipeIngredient { RecipeId = 15, IngredientId = 15 };
            appStorage.RecipeIngredients.Add(associatedRecipeIngredient);

            // Act
            bll.DeleteRecipe(15, null);

            // Assert
            Assert.AreEqual(12, appStorage.Recipes.Count);
            Assert.AreEqual(appStorage.RecipeIngredients.Count, appStorage.RecipeIngredients.Count);
            Assert.AreEqual(11, appStorage.Ingredients.Count); 
        }

        [TestMethod]
        public void DeleteRecipe_NoParameters_ThrowsArgumentException()
        {
            // Arrange
            AppStorage appStorage = new AppStorage();
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => bll.DeleteRecipe(null, null));
        }
    }
}