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
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    FoodItemIngredients = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
                }
                using (var IngredientsResponse = await httpClient.GetAsync($"{Constants.ApiUrl}Ingredients"))
                {
                    string IngredientApiResponse = await IngredientsResponse.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(IngredientsResponse))
                    {
                        return Redirect("/logout");
                    }
                    Ingredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(IngredientApiResponse);
                }
                using (var FoodItemResponse = await httpClient.GetAsync($"{Constants.ApiUrl}FoodItems/{id}"))
                {
                    string FooddItemApiResponse = await FoodItemResponse.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(FoodItemResponse))
                    {
                        return Redirect("/logout");
                    }
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
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(FoodItemIngredientObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}FoodItemIngredients", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }
                string FooddItemApiResponse = await response.Content.ReadAsStringAsync();
            }
            return RedirectToAction("Create");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int ItemId, int FoodItemId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.DeleteAsync($"{Constants.ApiUrl}FoodItemIngredients/{ItemId}"))
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