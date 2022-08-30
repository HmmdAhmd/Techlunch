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
    public class DeleteOrderController : Controller
    {
        private readonly IApiHelper _apiHelper;

        public DeleteOrderController(IApiHelper ApiHelper)
        {
            _apiHelper = ApiHelper;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWithoutConfirm(int id)
        {
            OrderViewModel orderObj = await _apiHelper.Get<OrderViewModel>($"orders/{id}");
            orderObj.Status = false;
            await _apiHelper.Put<OrderViewModel>(orderObj, $"orders/{orderObj.Id}");
            return RedirectToAction("Index", "Orders");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            List<OrderDetailsViewModel> orderDetailList = await _apiHelper.Get<List<OrderDetailsViewModel>>($"orderdetails/order/{id}");
            foreach (OrderDetailsViewModel orderDetailObj in orderDetailList)
            {
                await DeleteOrderItem(orderDetailObj);
            }

            OrderViewModel orderObj = await _apiHelper.Get<OrderViewModel>($"orders/{id}");
            orderObj.Status = false;
            await _apiHelper.Put<OrderViewModel>(orderObj, $"orders/{orderObj.Id}");

            return RedirectToAction("Index", "Orders");
        }



        private async Task DeleteOrderItem(OrderDetailsViewModel orderDetailObj)
        {
            List<FoodItemIngredientViewModel> foodItemIngredients = await _apiHelper.Get<List<FoodItemIngredientViewModel>>($"fooditemingredients/{orderDetailObj.FoodItemId}");
            GeneralInventoryViewModel generalInventoryObj = new GeneralInventoryViewModel();
            foreach (FoodItemIngredientViewModel foodItemIngredient in foodItemIngredients)
            {
                int Quantity = (int)(orderDetailObj.Quantity * foodItemIngredient.Quantity);
                generalInventoryObj = await _apiHelper.Get<GeneralInventoryViewModel> ($"generalinventories/ingredientid/{foodItemIngredient.IngredientId}");
                generalInventoryObj.AvailableQuantity += Quantity;
                await UpdateQuantityInGeneralInventory(generalInventoryObj);

            }
        }

        private async Task UpdateQuantityInGeneralInventory(GeneralInventoryViewModel generalInventoryObj)
        {
            await _apiHelper.Put<GeneralInventoryViewModel>(generalInventoryObj, $"generalinventories/{generalInventoryObj.Id}");
        }
    }
}
