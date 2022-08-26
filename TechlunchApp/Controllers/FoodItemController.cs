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

    public class FoodItemController : Controller
    {
        private readonly IConfiguration _configuration;

        public FoodItemController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            List<FoodItemViewModel> FoodItems = new List<FoodItemViewModel>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}fooditems"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
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
            if (ModelState.IsValid)
            {

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(FoodItemObj), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{_configuration.GetValue<string>("ApiUrl")}fooditems", content);
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }

                }
                return RedirectToAction("Index");
            }

            return View(FoodItemObj);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int ItemId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.DeleteAsync($"{_configuration.GetValue<string>("ApiUrl")}fooditems/{ItemId}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                }
            }
            return RedirectToAction("Index");
        }

    }
}
