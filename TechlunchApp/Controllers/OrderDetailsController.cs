using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using TechlunchApp.Common;

namespace TechlunchApp.Controllers
{
    public class OrderDetailsController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //using (var httpClient = new HttpClient())
            //{
            //    using (var response = await httpClient.DeleteAsync($"{Constants.ApiUrl}orderdetails/{id}"))
            //    {
            //        await response.Content.ReadAsStringAsync();
            //    }
            //}
            return RedirectToAction("AddItems", "Orders", new {id=1 });
        }
    }
}
