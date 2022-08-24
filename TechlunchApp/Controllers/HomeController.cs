using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.Models;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (Request.Cookies["token"] != null) {
                return Redirect("/dashboard");
            }
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {

            string st = DateTime.Now.StartOfWeek(DayOfWeek.Monday).ToString("MM/dd/yyyy hh:mm tt");
            string et = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

            ReportViewModel report = new ReportViewModel();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}report/getreport?StartingTime={st}&EndingTime={et}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    report = JsonConvert.DeserializeObject<ReportViewModel>(apiResponse);
                }
            }

            return View("dashboard", report);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
