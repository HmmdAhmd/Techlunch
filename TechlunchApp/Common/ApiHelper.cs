using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace TechlunchApp.Common
{
    public interface IApiHelper
    {
        Task<DynamicViewModel> Get<DynamicViewModel>(string PartialUrl);
        Task<HttpResponseMessage> Post<DynamicViewModel>(DynamicViewModel PostObj, string PartialUrl);
        Task<HttpResponseMessage> Put<DynamicViewModel>(DynamicViewModel PostObj, string PartialUrl);
        Task<HttpResponseMessage> Delete(string PartialUrl);
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

        public async Task<DynamicViewModel> Get<DynamicViewModel>(string PartialUrl)
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", _httpContextAccessor.HttpContext.Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{_configuration.GetValue<string>("ApiUrl")}{PartialUrl}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                        _httpContextAccessor.HttpContext.Response.Redirect("/logout");

                    DynamicViewModel Model = JsonConvert.DeserializeObject<DynamicViewModel>(apiResponse);
                    return Model;
                }
            }
        }


        public async Task<HttpResponseMessage> Post<DynamicViewModel>(DynamicViewModel PostObj, string PartialUrl) {

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", _httpContextAccessor.HttpContext.Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(PostObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_configuration.GetValue<string>("ApiUrl")}{PartialUrl}", content);
                if (!ApiAuthorization.IsAuthorized(response))
                    _httpContextAccessor.HttpContext.Response.Redirect("/logout");
                return response;
            }
        }

        public async Task<HttpResponseMessage> Put<DynamicViewModel>(DynamicViewModel PutObj, string PartialUrl)
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", _httpContextAccessor.HttpContext.Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(PutObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{_configuration.GetValue<string>("ApiUrl")}{PartialUrl}", content);
                if (!ApiAuthorization.IsAuthorized(response))
                    _httpContextAccessor.HttpContext.Response.Redirect("/logout");
                return response;
            }
        }

        public async Task<HttpResponseMessage> Delete(string PartialUrl)
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", _httpContextAccessor.HttpContext.Request.Cookies["token"]);
                using (var response = await httpClient.DeleteAsync($"{_configuration.GetValue<string>("ApiUrl")}{PartialUrl}"))
                {
                    await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                        _httpContextAccessor.HttpContext.Response.Redirect("/logout");
                    return response;
                }
            }
        }


    }
}
