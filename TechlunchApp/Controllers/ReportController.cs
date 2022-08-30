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
        private readonly IApiHelper _apiHelper;

        public ReportController(IApiHelper ApiHelper)
        {
            _apiHelper = ApiHelper;
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

                string St = st.ToString("MM/dd/yyyy hh:mm tt");
                string Et = et.ToString("MM/dd/yyyy") + " 11:59:59 pm";

                ReportViewModel report = await _apiHelper.Get<ReportViewModel>($"report/getreport?StartingTime={St}&EndingTime={Et}");

                ViewData["StartingTime"] = st.ToString(Constants.DateFormat);
                ViewData["EndingTime"] = et.ToString(Constants.DateFormat);

                return View("ShowReport", report);
            }

            ViewBag.Message = message;
            return View("Index", generateReportObj);
        }

    }
}
