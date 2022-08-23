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
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{orderId}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }

                orderObj.TotalPrice -= price;

                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}orders/{orderId}", content);
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
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/{id}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    orderDetailObj = JsonConvert.DeserializeObject<OrderDetailsViewModel>(apiResponse);
                    orderId = orderDetailObj.OrderId;
                }

                await DecrementOrderPrice(orderId, orderDetailObj.Price);

                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{foodItemId}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    foodItemIngredients = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
                }

                foreach (FoodItemIngredientViewModel foodItemIngredient in foodItemIngredients)
                {
                    int Quantity = (int)(orderDetailObj.Quantity * foodItemIngredient.Quantity);

                    GeneralInventoryViewModel generalInventoryObj = new GeneralInventoryViewModel();

                    using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories/ingredientid/{foodItemIngredient.IngredientId}"))
                    {
                        string apiResponse = await Response.Content.ReadAsStringAsync();
                        generalInventoryObj = JsonConvert.DeserializeObject<GeneralInventoryViewModel>(apiResponse);
                    }

                    generalInventoryObj.AvailableQuantity += Quantity;

                    await UpdateQuantityInGeneralInventory(generalInventoryObj);

                }

                using (var response = await httpClient.DeleteAsync($"{Constants.ApiUrl}orderdetails/{id}"))
                {
                    await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("AddItems", "Orders", new { id = orderId });
        }

        private async Task UpdateQuantityInGeneralInventory(GeneralInventoryViewModel generalInventoryObj)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(generalInventoryObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}generalinventories/{generalInventoryObj.Id}", content);

            }
        }
    }
}
