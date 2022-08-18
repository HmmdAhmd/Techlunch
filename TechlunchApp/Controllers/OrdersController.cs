using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class OrdersController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<OrderViewModel> orders = new List<OrderViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orders = JsonConvert.DeserializeObject<List<OrderViewModel>>(apiResponse);
                }
            }
            return View(orders);
        }
    }
}
