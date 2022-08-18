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
        public async Task<IActionResult> AddItems(int id)
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
            return View(Context);
        }

        private async Task<bool> CheckIngredientsAvailability(int foodItemId, int quantity)
        {
            List<FoodItemIngredientViewModel> ingredients = new List<FoodItemIngredientViewModel>();

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

                    if (generalInventoryObj == null || generalInventoryObj.AvailableQuantity < (quantity * ingredients[i].Quantity))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [HttpPost]
        public async Task<IActionResult> AddItems(OrderDetailsViewModel orderDetail)
        {
            bool available = await CheckIngredientsAvailability(orderDetail.FoodItemId, orderDetail.Quantity);

            if (available)
            {
                using (var httpClient = new HttpClient())
                {
                    FoodItemViewModel foodItem = new FoodItemViewModel();

                    using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditems/{orderDetail.FoodItemId}"))
                    {
                        string apiResponse = await Response.Content.ReadAsStringAsync();
                        foodItem = JsonConvert.DeserializeObject<FoodItemViewModel>(apiResponse);
                    }

                    orderDetail.Price = foodItem.Price * orderDetail.Quantity;

                    StringContent content = new StringContent(JsonConvert.SerializeObject(orderDetail), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{Constants.ApiUrl}orderdetails", content);
                }
            }
            else
            {
                ViewData["message"] = "hello g";
            }

            return RedirectToPage($"/AddItems/{orderDetail.OrderId}");
        }
    }
}
