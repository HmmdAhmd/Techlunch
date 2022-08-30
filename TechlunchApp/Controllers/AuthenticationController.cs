using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
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
                var response = await httpClient.PostAsync($"{_configuration.GetValue<string>("ApiUrl")}authenticate/login", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = Constants.LoginErrorMessage;
                    return RedirectToAction("Index", "Home");
                }
                token_detail = JsonConvert.DeserializeObject<ResponseViewModel>(apiResponse);
            }


            // Add the cookie to the response cookie collection
            Response.Cookies.Append("token", token_detail.Token);
            Response.Cookies.Append("user", token_detail.User);
            return Redirect("/dashboard");
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View("~/Views/Home/Register.cshtml");
        }



        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerObj)
        {

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(registerObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_configuration.GetValue<string>("ApiUrl")}authenticate/register", content);
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    TempData["ErrorMessage"] = Constants.UserExistErrorMessage;
                    return RedirectToAction("Register", "Authentication");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    TempData["ErrorMessage"] = Constants.PasswordErrorMessage;
                    return RedirectToAction("Register", "Authentication");
                }

            }
            TempData["SuccessMessage"] = Constants.SignupSuccessMessage;
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("user");
            return Redirect("/");
        }
    }
}
