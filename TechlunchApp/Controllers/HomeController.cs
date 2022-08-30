using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IApiHelper _apiHelper;

        public HomeController(IApiHelper ApiHelper)
        {
            _apiHelper = ApiHelper;
        }
        public IActionResult Index()
        {
            if (Request.Cookies["token"] != null)
            {
                return Redirect("/dashboard");
            }
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {

            string reportStartDateTime = DateTime.Now.StartOfWeek(DayOfWeek.Monday).ToString("MM/dd/yyyy hh:mm tt");
            string reportEndDateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            ReportViewModel report = await _apiHelper.Get<ReportViewModel>($"report/getreport?StartingTime={reportStartDateTime}&EndingTime={reportEndDateTime}");
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
