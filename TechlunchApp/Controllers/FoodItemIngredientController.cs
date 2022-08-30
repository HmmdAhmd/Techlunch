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

        private readonly IConfiguration _configuration;

        public FoodItemIngredientController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Create(int id)
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}fooditemingredients/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    FoodItemIngredients = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
                }
                using (var IngredientsResponse = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}Ingredients"))
                {
                    string IngredientApiResponse = await IngredientsResponse.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(IngredientsResponse))
                    {
                        return Redirect("/logout");
                    }
                    Ingredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(IngredientApiResponse);
                }
                using (var FoodItemResponse = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}FoodItems/{id}"))
                {
                    string FooddItemApiResponse = await FoodItemResponse.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(FoodItemResponse))
                    {
                        return Redirect("/logout");
                    }
                    FoodItem = JsonConvert.DeserializeObject<FoodItemViewModel>(FooddItemApiResponse);
                }
            }
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
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(FoodItemIngredientObj), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{_configuration.GetValue<string>("ApiUrl")}FoodItemIngredients", content);
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    string FooddItemApiResponse = await response.Content.ReadAsStringAsync();
                }
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
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.DeleteAsync($"{_configuration.GetValue<string>("ApiUrl")}FoodItemIngredients/{ItemId}"))
                {
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return Redirect($"/FoodItemIngredient/Create/{FoodItemId}");
        }
    }
}