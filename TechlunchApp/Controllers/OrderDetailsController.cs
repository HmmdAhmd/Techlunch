using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    
        private readonly IApiHelper _apiHelper;

        public OrderDetailsController(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        private async Task DecrementOrderPrice(int orderId, float price)
        {
            OrderViewModel orderObj = await _apiHelper.Get<OrderViewModel>($"orders/{orderId}");
            orderObj.TotalPrice -= price;
            await _apiHelper.Put<OrderViewModel>(orderObj, $"orders/{orderId}");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int foodItemId)
        {
            int orderId = 0;
            

            OrderDetailsViewModel orderDetailObj = await _apiHelper.Get<OrderDetailsViewModel>($"orderdetails/{id}");
            orderId = orderDetailObj.OrderId;
            await DecrementOrderPrice(orderId, orderDetailObj.Price);

            List<FoodItemIngredientViewModel> foodItemIngredients = await _apiHelper.Get<List<FoodItemIngredientViewModel>>($"fooditemingredients/{foodItemId}");

            foreach (FoodItemIngredientViewModel foodItemIngredient in foodItemIngredients)
            {
                int Quantity = (int)(orderDetailObj.Quantity * foodItemIngredient.Quantity);
                GeneralInventoryViewModel generalInventoryObj = new GeneralInventoryViewModel();
                generalInventoryObj = await _apiHelper.Get<GeneralInventoryViewModel>($"generalinventories/ingredientid/{foodItemIngredient.IngredientId}");
                generalInventoryObj.AvailableQuantity += Quantity;
                await UpdateQuantityInGeneralInventory(generalInventoryObj);
            }

            await _apiHelper.Delete($"orderdetails/{id}");

            return RedirectToAction("AddItems", "Orders", new { id = orderId });
        }

        private async Task UpdateQuantityInGeneralInventory(GeneralInventoryViewModel generalInventoryObj)
        {
            await _apiHelper.Put<GeneralInventoryViewModel>(generalInventoryObj, $"generalinventories/{generalInventoryObj.Id}");
        }
    }
}
