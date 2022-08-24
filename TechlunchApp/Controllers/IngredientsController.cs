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
    public class IngredientsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<IngredientViewModel> ingredients = new List<IngredientViewModel>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}ingredients"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response)) {
                        return Redirect("/logout");
                    }
                    ingredients = JsonConvert.DeserializeObject<List<IngredientViewModel>>(apiResponse);
                }
            }

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


            using (var httpClient = new HttpClient())
            {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(ingredientObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}ingredients", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }


                }
                return RedirectToAction("Index");
            }

            return View(ingredientObj);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            IngredientViewModel ingredient = new IngredientViewModel();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}ingredients/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    ingredient = JsonConvert.DeserializeObject<IngredientViewModel>(apiResponse);
                }
            }
            return View(ingredient);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IngredientViewModel ingredientObj)
        {
            if (ModelState.IsValid)
            {

                using (var httpClient = new HttpClient())
                {
                     httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(ingredientObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}ingredients/{ingredientObj.Id}", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }


                }
                return RedirectToAction("Index");
            }

            return View(ingredientObj);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.DeleteAsync($"{Constants.ApiUrl}ingredients/{id}"))
                {
                    await response.Content.ReadAsStringAsync();
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