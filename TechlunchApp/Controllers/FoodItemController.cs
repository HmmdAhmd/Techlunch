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

    public class FoodItemController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<FoodItemViewModel> FoodItems = new List<FoodItemViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditems"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    FoodItems = JsonConvert.DeserializeObject<List<FoodItemViewModel>>(apiResponse);
                }
            }
            return View(FoodItems);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(FoodItemViewModel FoodItemObj)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(FoodItemObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}fooditems", content);

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int ItemId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"{Constants.ApiUrl}fooditems/{ItemId}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("Index");
        }

    }
}
