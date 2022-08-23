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
        private static string message = "";

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

        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            OrderViewModel orderObj = new OrderViewModel();

            using (var httpClient = new HttpClient())
            {
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);

                    orderObj.Confirmed = true;

                }

                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}orders/{id}", content);

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            List<OrderDetailsViewModel> orderDetailsList = new List<OrderDetailsViewModel>();
            OrderViewModel orderObj = new OrderViewModel();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }

                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/order/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orderDetailsList = JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(apiResponse);
                }

            }

            dynamic Context = new ExpandoObject();
            Context.Order = orderObj;
            Context.OrderDetailList = orderDetailsList;
            return View(Context);
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            OrderViewModel orderObj = new OrderViewModel
            {
                TotalPrice = 0,
                Status = true,
                Confirmed = false,
                CreatedAt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm"))
            };
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}orders", content);

                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/latest"))
                {
                    if (!Response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }
            }

            return RedirectToAction("AddItems", "Orders", new { id = orderObj.Id });
        }

        [HttpGet]
        public async Task<IActionResult> AddItems(int id)
        {
            OrderViewModel orderObj = new OrderViewModel();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }
            }

            List<FoodItemViewModel> foodItems = new List<FoodItemViewModel>();
            List<OrderDetailsViewModel> orderDetailsList = new List<OrderDetailsViewModel>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditems"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    foodItems = JsonConvert.DeserializeObject<List<FoodItemViewModel>>(apiResponse);
                }

                for(int i=0;i<foodItems.Count;)
                {
                    List<FoodItemIngredientViewModel> temp = new List<FoodItemIngredientViewModel>();
                    using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{foodItems[i].Id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        temp = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
                    }
                    if (temp.Count == 0)
                    {
                        foodItems.Remove(foodItems[i]);
                    } else
                    {
                        orderDetailsList.Add(new OrderDetailsViewModel
                        {
                            OrderId = id,
                            FoodItemId = foodItems[i].Id,
                            FoodItemFK = foodItems[i],
                            Quantity = 0,
                            Price = 0
                        });
                        i++;
                    }
                }

                //using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/order/{id}"))
                //{
                //    string apiResponse = await response.Content.ReadAsStringAsync();
                //    orderDetailsList = JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(apiResponse);
                //}

            }

            dynamic Context = new ExpandoObject();
            Context.FoodItems = foodItems;
            Context.OrderDetails = orderDetailsList;
            Context.Order = orderObj;

            ViewData["message"] = message;
            message = "";
            return View(Context);
        }

        [HttpPost]
        public async Task<IActionResult> AddItems(List<OrderDetailsViewModel> orderDetails)
        {


            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> AddItems(OrderDetailsViewModel orderDetail)
        //{
        //    float estPrice = await CheckIngredientsAvailability(orderDetail.FoodItemId, orderDetail.Quantity);

        //    FoodItemViewModel foodItem = new FoodItemViewModel();

        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditems/{orderDetail.FoodItemId}"))
        //        {
        //            string apiResponse = await Response.Content.ReadAsStringAsync();
        //            foodItem = JsonConvert.DeserializeObject<FoodItemViewModel>(apiResponse);
        //        }
        //    }

        //    if (estPrice != 0)
        //    {
        //        float price = 0;

        //        using (var httpClient = new HttpClient())
        //        {
        //            orderDetail.Price = foodItem.Price * orderDetail.Quantity;
        //            orderDetail.EstimatedPrice = estPrice;
        //            price = orderDetail.Price;

        //            StringContent content = new StringContent(JsonConvert.SerializeObject(orderDetail), Encoding.UTF8, "application/json"); ;
        //            var response = await httpClient.PostAsync($"{Constants.ApiUrl}orderdetails", content);
        //        }

        //        await IncrementOrderPrice(orderDetail.OrderId, price);


        //    }
        //    else
        //    {
        //        message = $"Food Item: {foodItem.Name} cannot be added as specified quantity is unavailable";
        //    }

        //    return RedirectToAction("AddItems", "Orders", new { id = orderDetail.OrderId });
        //}

        public async Task<IActionResult> Delete(int id)
        {
            OrderViewModel orderObj = new OrderViewModel();

            using (var httpClient = new HttpClient())
            {
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    string apiResponse = await Response.Content.ReadAsStringAsync();
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }
            }

            return View(orderObj);
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

        private async Task<float> CheckIngredientsAvailability(int foodItemId, int quantity)
        {
            List<FoodItemIngredientViewModel> ingredients = new List<FoodItemIngredientViewModel>();
            List<GeneralInventoryViewModel> generalInventoryList = new List<GeneralInventoryViewModel>();

            float estPrice = 0;

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
                        return 0;
                    }
                    else
                    {
                        estPrice += generalInventoryObj.AveragePrice * Quantity;
                        generalInventoryObj.AvailableQuantity -= Quantity;
                        generalInventoryList.Add(generalInventoryObj);
                    }
                }
            }

            await UpdateQuantityInGeneralInventory(generalInventoryList);

            return estPrice;
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
