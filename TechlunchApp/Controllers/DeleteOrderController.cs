using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class DeleteOrderController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> DeleteWithoutConfirm(int id)
        {
            OrderViewModel orderObj = new OrderViewModel();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(Response))
                    {
                        return Redirect("/logout");
                    }
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }

                orderObj.Status = false;

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}orders/{orderObj.Id}", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }
            }

            return RedirectToAction("Index", "Orders");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            OrderViewModel orderObj = new OrderViewModel();

            List<OrderDetailsViewModel> orderDetailList = new List<OrderDetailsViewModel>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/order/{id}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(Response))
                    {
                        return Redirect("/logout");
                    }
                    orderDetailList = JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(apiResponse);
                }

                foreach (OrderDetailsViewModel orderDetailObj in orderDetailList)
                {
                    await DeleteOrderItem(orderDetailObj);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(Response))
                    {
                        return Redirect("/logout");
                    }
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }

                orderObj.Status = false;

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}orders/{orderObj.Id}", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }
            }

            return RedirectToAction("Index", "Orders");
        }

        private async Task DeleteOrderItem(OrderDetailsViewModel orderDetailObj)
        {
            List<FoodItemIngredientViewModel> foodItemIngredients =
                new List<FoodItemIngredientViewModel>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{orderDetailObj.FoodItemId}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(Response))
                    {
                        RedirectToAction("Logout", "Authentication");
                    }
                    foodItemIngredients = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
                }

                foreach (FoodItemIngredientViewModel foodItemIngredient in foodItemIngredients)
                {
                    int Quantity = (int)(orderDetailObj.Quantity * foodItemIngredient.Quantity);

                    GeneralInventoryViewModel generalInventoryObj = new GeneralInventoryViewModel();

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                    using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories/ingredientid/{foodItemIngredient.IngredientId}"))
                    {
                        string apiResponse = await Response.Content.ReadAsStringAsync();
                        if (!ApiAuthorization.IsAuthorized(Response))
                        {
                            RedirectToAction("Logout", "Authentication");
                        }
                        generalInventoryObj = JsonConvert.DeserializeObject<GeneralInventoryViewModel>(apiResponse);
                    }

                    generalInventoryObj.AvailableQuantity += Quantity;

                    await UpdateQuantityInGeneralInventory(generalInventoryObj);

                }
            }
        }

        private async Task UpdateQuantityInGeneralInventory(GeneralInventoryViewModel generalInventoryObj)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(generalInventoryObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}generalinventories/{generalInventoryObj.Id}", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    RedirectToAction("Logout", "Authentication");
                }

            }
        }
    }
}
