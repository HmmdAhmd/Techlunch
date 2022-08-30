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
    public class IngredientsController : Controller
    {
        private readonly IApiHelper _apiHelper;
        public IngredientsController(IApiHelper ApiHelper)
        {
            _apiHelper = ApiHelper;
        }
        
        public async Task<IActionResult> Index()
        {
           
            List<IngredientViewModel> ingredients = await _apiHelper.Get<List<IngredientViewModel>>("ingredients");
            return View(ingredients);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IngredientViewModel ingredientObj)
        {
            if (ModelState.IsValid)
            {
                await _apiHelper.Post<IngredientViewModel>(ingredientObj, "ingredients");
                return RedirectToAction("Index");
            }

            return View(ingredientObj);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            IngredientViewModel ingredient = await _apiHelper.Get<IngredientViewModel>($"ingredients/{id}");
            return View(ingredient);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IngredientViewModel ingredientObj)
        {
            if (ModelState.IsValid)
            {
                await _apiHelper.Put<IngredientViewModel>(ingredientObj, $"ingredients/{ingredientObj.Id}");
                return RedirectToAction("Index");
            }

            return View(ingredientObj);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
             await _apiHelper.Delete($"ingredients/{id}");
             return RedirectToAction("Index");
        }

    }
}