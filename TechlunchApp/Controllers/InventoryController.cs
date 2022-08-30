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
    public class InventoryController : Controller
    {
        private static List<IngredientViewModel> Ingredients = new List<IngredientViewModel>();
        private readonly IApiHelper _apiHelper;

        public InventoryController(IApiHelper ApiHelper)
        {
            _apiHelper = ApiHelper;
        }

        public async Task<IActionResult> Index()
        {
            List<GeneralInventoryViewModel> inventoryList = await _apiHelper.Get<List<GeneralInventoryViewModel>>("generalinventories");
            return View(inventoryList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Ingredients = await _apiHelper.Get<List<IngredientViewModel>>("ingredients");
            ViewBag.Ingredients = Ingredients;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(InventoryViewModel inventoryObj)
        {
            if (ModelState.IsValid)
            {
                await _apiHelper.Post<InventoryViewModel>(inventoryObj, "inventories");
                return RedirectToAction("Index");
            }

            ViewBag.Ingredients = Ingredients;
            return View(inventoryObj);
        }

        [HttpGet]
        public async Task<ActionResult> IngredientHistory(int id)
        {
            List<IngredientHistoryViewModel> IngredientHistories = await _apiHelper.Get<List<IngredientHistoryViewModel>>($"Inventories/Ingredient/{id}");
            IngredientViewModel ingredientObj = await _apiHelper.Get<IngredientViewModel>($"Ingredients/{id}");
            ViewData["name"] = ingredientObj.Name;
            return View(IngredientHistories);
        }

    }
}
