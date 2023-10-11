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

    }
}