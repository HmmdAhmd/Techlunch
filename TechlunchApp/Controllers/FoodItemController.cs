using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{

    public class FoodItemController : Controller
    {
        private readonly string _apiUrl;
        public FoodItemController(IConfiguration configuration)
        {
            _apiUrl = configuration.GetValue<string>("apiUrl");
        }
        public async Task<IActionResult> Index()
        {
            List<FoodItemViewModel> FoodItems = new List<FoodItemViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{_apiUrl}fooditems"))
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
            FoodItemViewModel NewFoodItem = new FoodItemViewModel();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(FoodItemObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_apiUrl}fooditems", content);

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int ItemId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"{_apiUrl}fooditems/{ItemId}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("Index");
        }

    }
}
