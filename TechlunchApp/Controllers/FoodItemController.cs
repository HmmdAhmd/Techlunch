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
       
        private readonly IApiHelper _apiHelper;

        public FoodItemController(IApiHelper ApiHelper)
        {
            _apiHelper = ApiHelper;
        }
        public async Task<IActionResult> Index()
        {
            List<FoodItemViewModel> FoodItems  = await _apiHelper.Get<List<FoodItemViewModel>>("fooditems");
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
                await _apiHelper.Post<FoodItemViewModel>(FoodItemObj, "fooditems");
                return RedirectToAction("Index");
            }

            return View(FoodItemObj);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int ItemId)
        {
            await _apiHelper.Delete($"fooditems/{ItemId}");
            return RedirectToAction("Index");
        }

    }
}
