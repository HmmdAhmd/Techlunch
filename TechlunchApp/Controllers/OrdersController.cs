using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechlunchApp.Common;
using TechlunchApp.ViewModels;

namespace TechlunchApp.Controllers
{
    public class OrdersController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<OrderViewModel> orders = new List<OrderViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orders = JsonConvert.DeserializeObject<List<OrderViewModel>>(apiResponse);
                }
            }
            return View(orders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel orderObj)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}orders", content);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> AddItems(int id, string message = "")
        {
            List<FoodItemViewModel> foodItems = new List<FoodItemViewModel>();
            List<OrderDetailsViewModel> orderDetailsList = new List<OrderDetailsViewModel>();
            OrderViewModel orderObj = new OrderViewModel();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditems"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    foodItems = JsonConvert.DeserializeObject<List<FoodItemViewModel>>(apiResponse);
                }

                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/order/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orderDetailsList = JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(apiResponse);
                }

                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }

            }

            dynamic Context = new ExpandoObject();
            Context.FoodItems = foodItems;
            Context.OrderDetails = orderDetailsList;
            Context.Order = orderObj;

            ViewData["message"] = message;
            return View(Context);
        }

        [HttpPost]
        public async Task<IActionResult> AddItems(OrderDetailsViewModel orderDetail)
        {
            bool available = await CheckIngredientsAvailability(orderDetail.FoodItemId, orderDetail.Quantity);

            FoodItemViewModel foodItem = new FoodItemViewModel();

            using (var httpClient = new HttpClient())
            {
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditems/{orderDetail.FoodItemId}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    foodItem = JsonConvert.DeserializeObject<FoodItemViewModel>(apiResponse);
                }
            }

            if (available)
            {
                float price = 0;

                using (var httpClient = new HttpClient())
                {
                    orderDetail.Price = foodItem.Price * orderDetail.Quantity;
                    price = orderDetail.Price;

                    StringContent content = new StringContent(JsonConvert.SerializeObject(orderDetail), Encoding.UTF8, "application/json"); ;
                    var response = await httpClient.PostAsync($"{Constants.ApiUrl}orderdetails", content);
                }

                await IncrementOrderPrice(orderDetail.OrderId, price);

                return RedirectToPage($"/AddItems/{orderDetail.OrderId}");
            }

            string message = $"Food Item: {foodItem.Name} cannot be added as specified quantity is unavailable";
            return await AddItems(1, message);
        }

        private async Task UpdateQuantityInGeneralInventory(List<GeneralInventoryViewModel> generalInventoryList)
        {
            foreach (GeneralInventoryViewModel generalInventoryObj in generalInventoryList)
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(generalInventoryObj), Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync($"{Constants.ApiUrl}generalinventories/{generalInventoryObj.Id}", content);

                }
            }
        }

        private async Task<bool> CheckIngredientsAvailability(int foodItemId, int quantity)
        {
            List<FoodItemIngredientViewModel> ingredients = new List<FoodItemIngredientViewModel>();
            List<GeneralInventoryViewModel> generalInventoryList = new List<GeneralInventoryViewModel>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{foodItemId}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ingredients = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
                }

                for (int i = 0; i < ingredients.Count; i++)
                {
                    GeneralInventoryViewModel generalInventoryObj = new GeneralInventoryViewModel();

                    using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories/ingredient/{ingredients[i].IngredientId}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        generalInventoryObj = JsonConvert.DeserializeObject<GeneralInventoryViewModel>(apiResponse);
                    }

                    int Quantity = quantity * ingredients[i].Quantity;
                    if (generalInventoryObj == null || generalInventoryObj.AvailableQuantity < Quantity)
                    {
                        return false;
                    }
                    else
                    {
                        generalInventoryObj.AvailableQuantity -= Quantity;
                        generalInventoryList.Add(generalInventoryObj);
                    }
                }
            }

            await UpdateQuantityInGeneralInventory(generalInventoryList);

            return true;
        }

        private async Task IncrementOrderPrice(int orderId, float price)
        {
            OrderViewModel orderObj = new OrderViewModel();

            using (var httpClient = new HttpClient())
            {
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{orderId}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }

                orderObj.TotalPrice += price;

                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}orders/{orderId}", content);
            }
        }
    }
}
