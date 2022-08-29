using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace TechlunchApp.Common
{
    public interface IApiHelper
    {
        Task<List<DynamicViewModel>> Get<DynamicViewModel>(string PartialUrl);
    }

    public class ApiHelper : IApiHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ApiHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<DynamicViewModel>> Get<DynamicViewModel>(string PartialUrl)
        {

            List<DynamicViewModel> Model = new List<DynamicViewModel>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", _httpContextAccessor.HttpContext.Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}{PartialUrl}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                        _httpContextAccessor.HttpContext.Response.Redirect("/logout");
                     
                    Model = JsonConvert.DeserializeObject<List<DynamicViewModel>>(apiResponse);
                    return Model;
                }
            }
        }
    }
}
