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
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
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
                using (var IngredientsResponse = await httpClient.GetAsync($"{Constants.ApiUrl}Ingredients"))
                {
                    string IngredientApiResponse = await IngredientsResponse.Content.ReadAsStringAsync();
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
                StringContent content = new StringContent(JsonConvert.SerializeObject(inventoryObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}inventories", content);

            }
            return RedirectToAction("Index");
        }

    }
}
