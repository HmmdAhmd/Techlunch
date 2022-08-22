using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;
namespace TechlunchApp.Controllers
{
    public class FoodItemIngredientController : Controller
    {
        public async Task<IActionResult> Create(int id)
        {
            List<FoodItemIngredientViewModel> FoodItemIngredients = new List<FoodItemIngredientViewModel>();
            List<IngredientViewModel> Ingredients = new List<IngredientViewModel>();
            FoodItemViewModel FoodItem = new FoodItemViewModel();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    FoodItemIngredients = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
                }
                using (var IngredientsResponse = await httpClient.GetAsync($"{Constants.ApiUrl}Ingredients"))
                {
                    string IngredientApiResponse = await IngredientsResponse.Content.ReadAsStringAsync();
                    Ingredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(IngredientApiResponse);
                }
                using (var FoodItemResponse = await httpClient.GetAsync($"{Constants.ApiUrl}FoodItems/{id}"))
                {
                    string FooddItemApiResponse = await FoodItemResponse.Content.ReadAsStringAsync();
                    FoodItem = JsonConvert.DeserializeObject<FoodItemViewModel>(FooddItemApiResponse);
                }
            }
            dynamic Context = new ExpandoObject();
            Context.FoodItem = FoodItem;
            Context.Ingredients = Ingredients;
            Context.FoodItemIngredients = FoodItemIngredients;
            return View("Index", Context);
        }
        [HttpPost]
        public async Task<IActionResult> Create(FoodItemIngredientViewModel FoodItemIngredientObj)
        {
            FoodItemIngredientViewModel NewFoodItem = new FoodItemIngredientViewModel();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(FoodItemIngredientObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}FoodItemIngredients", content);
                string FooddItemApiResponse = await response.Content.ReadAsStringAsync();
            }
            return RedirectToAction("Create");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int ItemId, int FoodItemId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"{Constants.ApiUrl}FoodItemIngredients/{ItemId}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return Redirect($"/FoodItemIngredient/Create/{FoodItemId}");
        }
    }
}