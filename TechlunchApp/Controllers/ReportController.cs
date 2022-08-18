using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(DateTime StartingTime, DateTime EndingTime)
        {
            string St = StartingTime.ToString("MM/dd/yyyy hh:mm:ss tt");
            string Et = EndingTime.ToString("MM/dd/yyyy hh:mm:ss tt");
            ReportViewModel report = new ReportViewModel();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}report/getreport?StartingTime={St}&EndingTime={Et}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    report = JsonConvert.DeserializeObject<ReportViewModel>(apiResponse);
                }
            }
            ViewData["StartingTime"] = StartingTime;
            ViewData["EndingTime"] = EndingTime;
            return View("ShowReport",report);
        }

    }
}
