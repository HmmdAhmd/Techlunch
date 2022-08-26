using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class InventoryController : Controller
    {
        private static List<IngredientViewModel> Ingredients = new List<IngredientViewModel>();

        private readonly IConfiguration _configuration;

        public InventoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            List<GeneralInventoryViewModel> inventoryList = new List<GeneralInventoryViewModel>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}generalinventories"))
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
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var IngredientsResponse = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}Ingredients"))
                {
                    string IngredientApiResponse = await IngredientsResponse.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(IngredientsResponse))
                    {
                        return Redirect("/logout");
                    }
                    Ingredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(IngredientApiResponse);

                }
            }

            ViewBag.Ingredients = Ingredients;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(InventoryViewModel inventoryObj)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(inventoryObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_configuration.GetValue<string>("ApiUrl")}inventories", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }

                }
                return RedirectToAction("Index");
            }

            ViewBag.Ingredients = Ingredients;
            return View(inventoryObj);
        }

        [HttpGet]
        public async Task<ActionResult> IngredientHistory(int id)
        {
            List<IngredientHistoryViewModel> IngredientHistories = new List<IngredientHistoryViewModel>();
            IngredientViewModel ingredientObj;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}Inventories/Ingredient/{id}"))
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    IngredientHistories = JsonConvert.DeserializeObject<List<IngredientHistoryViewModel>>(content);
                }

                using (var response = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}Ingredients/{id}"))
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
