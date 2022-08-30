using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;
namespace TechlunchApp.Controllers
{
    public class FoodItemIngredientController : Controller
    {

        private static List<FoodItemIngredientViewModel> FoodItemIngredients = new List<FoodItemIngredientViewModel>();
        private static List<IngredientViewModel> Ingredients = new List<IngredientViewModel>();
        private static FoodItemViewModel FoodItem = new FoodItemViewModel();

        private readonly IApiHelper _apiHelper;

        public FoodItemIngredientController(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<IActionResult> Create(int id)
        {
            FoodItemIngredients = await _apiHelper.Get<List<FoodItemIngredientViewModel>>($"fooditemingredients/{id}");
            Ingredients = await _apiHelper.Get<List<IngredientViewModel>>($"Ingredients");
            FoodItem = await _apiHelper.Get<FoodItemViewModel>($"FoodItems/{id}");

            ViewBag.FoodItem = FoodItem;
            ViewBag.Ingredients = Ingredients;
            ViewBag.FoodItemIngredients = FoodItemIngredients;
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Create(FoodItemIngredientViewModel FoodItemIngredientObj)
        {
            if (ModelState.IsValid)
            {
                await _apiHelper.Post<FoodItemIngredientViewModel>(FoodItemIngredientObj, "FoodItemIngredients");
                return RedirectToAction("Create");
            }

            ViewBag.FoodItem = FoodItem;
            ViewBag.Ingredients = Ingredients;
            ViewBag.FoodItemIngredients = FoodItemIngredients;
            return View("Index", FoodItemIngredientObj);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int ItemId, int FoodItemId)
        {
            await _apiHelper.Delete($"FoodItemIngredients/{ItemId}");
            return Redirect($"/FoodItemIngredient/Create/{FoodItemId}");
        }
    }
}