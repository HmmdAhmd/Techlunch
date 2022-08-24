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
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginObj)
        {
            ResponseViewModel token_detail = new ResponseViewModel();
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(loginObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}authenticate/login", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) {
                    TempData["ErrorMessage"]  = "Invalid Credentials";
                    return RedirectToAction("Index","Home");
                }
                token_detail = JsonConvert.DeserializeObject<ResponseViewModel>(apiResponse);
            }
            

            // Add the cookie to the response cookie collection
            Response.Cookies.Append("token", token_detail.Token);
            return Redirect("/dashboard");
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View("~/Views/Home/Register.cshtml");
        }



        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel ingredientObj)
        {

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(ingredientObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}authenticate/register", content);
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = "User Already Exist";
                    return RedirectToAction("Register", "Authentication");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError) {
                    TempData["ErrorMessage"] = "Password must contain Non-Alphanumeric,Digit and Upper Case letter";
                    return RedirectToAction("Register", "Authentication");
                }

            }
            TempData["SuccessMessage"] = "Account Created Successfully";
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
         public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("token");
            return Redirect("/");
        }
    }
}
