using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class InventoryController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<InventoryViewModel> inventoryList = new List<InventoryViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    inventoryList = JsonConvert.DeserializeObject<List<InventoryViewModel>>(apiResponse);
                }
            }
            return View(inventoryList);
        }
    }
}
