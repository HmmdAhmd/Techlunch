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
    public class OrderDetailsController : Controller
    {
        private async Task DecrementOrderPrice(int orderId, float price)
        {
            OrderViewModel orderObj = new OrderViewModel();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{orderId}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(Response))
                    {
                        RedirectToAction("Logout", "Authentication");
                    }
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }

                orderObj.TotalPrice -= price;

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}orders/{orderId}", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    RedirectToAction("Logout", "Authentication");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int foodItemId)
        {
            int orderId = 0;
            List<FoodItemIngredientViewModel> foodItemIngredients =
                new List<FoodItemIngredientViewModel>();

            OrderDetailsViewModel orderDetailObj = new OrderDetailsViewModel();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/{id}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(Response))
                    {
                        return Redirect("/logout");
                    }
                    orderDetailObj = JsonConvert.DeserializeObject<OrderDetailsViewModel>(apiResponse);
                    orderId = orderDetailObj.OrderId;
                }

                await DecrementOrderPrice(orderId, orderDetailObj.Price);

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{foodItemId}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(Response))
                    {
                        return Redirect("/logout");
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
                            return Redirect("/logout");
                        }
                        generalInventoryObj = JsonConvert.DeserializeObject<GeneralInventoryViewModel>(apiResponse);
                    }

                    generalInventoryObj.AvailableQuantity += Quantity;

                    await UpdateQuantityInGeneralInventory(generalInventoryObj);

                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.DeleteAsync($"{Constants.ApiUrl}orderdetails/{id}"))
                {
                    await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                }
            }
            return RedirectToAction("AddItems", "Orders", new { id = orderId });
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
