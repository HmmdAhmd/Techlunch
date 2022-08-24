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

        private static bool message = false;
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
                DateTime st = (DateTime)generateReportObj.StartingTime;
                DateTime et = (DateTime)generateReportObj.EndingTime;

                if (st > et)
                {
                    ViewBag.Message = true;
                    return View("Index", generateReportObj);
                }

                
                string St = st.ToString("MM/dd/yyyy hh:mm tt");
                string Et = et.ToString("MM/dd/yyyy hh:mm tt");
                ReportViewModel report = new ReportViewModel();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}report/getreport?StartingTime={St}&EndingTime={Et}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        report = JsonConvert.DeserializeObject<ReportViewModel>(apiResponse);
                    }
                }
                ViewData["StartingTime"] = generateReportObj.StartingTime;
                ViewData["EndingTime"] = generateReportObj.EndingTime;
                return View("ShowReport", report);
            }

            ViewBag.Message = message;
            return View("Index", generateReportObj);
        }

    }
}
