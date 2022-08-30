using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class ReportController : Controller
    {

        private static bool message = false;
        private readonly IConfiguration _configuration;

        public ReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(GenerateReportViewModel generateReportObj)
        {
            message = false;

            if (ModelState.IsValid)
            {
                DateTime startingDateTime = (DateTime)generateReportObj.StartingTime;
                DateTime endingDateTime = (DateTime)generateReportObj.EndingTime;

                if (startingDateTime > endingDateTime)
                {
                    ViewBag.Message = true;
                    return View("Index", generateReportObj);
                }


                string startingDate = startingDateTime.ToString("MM/dd/yyyy hh:mm tt");
                string endingDate = endingDateTime.ToString("MM/dd/yyyy") + " 11:59:59 pm";
                ReportViewModel report = new ReportViewModel();
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                    using (var response = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}report/getreport?StartingTime={startingDate}&EndingTime={endingDate}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (!ApiAuthorization.IsAuthorized(response))
                        {
                            return Redirect("/logout");
                        }
                        report = JsonConvert.DeserializeObject<ReportViewModel>(apiResponse);
                    }
                }
                ViewData["StartingTime"] = startingDateTime.ToString(Constants.DateFormat);
                ViewData["EndingTime"] = endingDateTime.ToString(Constants.DateFormat);
                return View("ShowReport", report);
            }

            ViewBag.Message = message;
            return View("Index", generateReportObj);
        }

    }
}
