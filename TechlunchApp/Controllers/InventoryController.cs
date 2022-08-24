using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class InventoryController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<GeneralInventoryViewModel> inventoryList = new List<GeneralInventoryViewModel>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    inventoryList = JsonConvert.DeserializeObject<List<GeneralInventoryViewModel>>(apiResponse);
                }
            }
            return View(inventoryList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<IngredientViewModel> Ingredients = new List<IngredientViewModel>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var IngredientsResponse = await httpClient.GetAsync($"{Constants.ApiUrl}Ingredients"))
                {
                    string IngredientApiResponse = await IngredientsResponse.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(IngredientsResponse))
                    {
                        return Redirect("/logout");
                    }
                    Ingredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(IngredientApiResponse);

                }
            }

            return View(Ingredients);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InventoryViewModel inventoryObj)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(inventoryObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}inventories", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> IngredientHistory(int id)
        {
            List<IngredientHistoryViewModel> IngredientHistories = new List<IngredientHistoryViewModel>();
            IngredientViewModel ingredientObj;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}Inventories/Ingredient/{id}"))
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    IngredientHistories = JsonConvert.DeserializeObject<List<IngredientHistoryViewModel>>(content);
                }

                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}Ingredients/{id}"))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                    string content = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    ingredientObj = JsonConvert.DeserializeObject<IngredientViewModel>(content);
                }
            }

            ViewData["name"] = ingredientObj.Name;
            return View(IngredientHistories);
        }

    }
}
