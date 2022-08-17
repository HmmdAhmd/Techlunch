using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class IngredientsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<IngredientViewModel> ingredients = new List<IngredientViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}ingredients"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ingredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(apiResponse);
                }
            }
            return View(ingredients);
        }

    }
}