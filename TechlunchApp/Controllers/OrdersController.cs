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
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    orders = JsonConvert.DeserializeObject<List<OrderViewModel>>(apiResponse);
                }
            }
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
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

                    orderObj.Confirmed = true;

                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{Constants.ApiUrl}orders/{id}", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }

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
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/order/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
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
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}orders", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var Response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/latest"))
                {
                    if (!Response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(Response))
                    {
                        return Redirect("/logout");
                    }
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
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{id}"))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);

                    if (orderObj.Confirmed)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            List<FoodItemViewModel> foodItems = new List<FoodItemViewModel>();
            List<OrderDetailsViewModel> orderDetailsList = new List<OrderDetailsViewModel>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditems"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    foodItems = JsonConvert.DeserializeObject<List<FoodItemViewModel>>(apiResponse);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/order/{id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                    orderDetailsList = JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(apiResponse);
                }

                for (int i = 0; i < foodItems.Count;)
                {
                    List<FoodItemIngredientViewModel> temp = new List<FoodItemIngredientViewModel>();

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                    using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{foodItems[i].Id}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (!ApiAuthorization.IsAuthorized(response))
                        {
                            return Redirect("/logout");
                        }
                        temp = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
                    }
                    if (temp.Count == 0)
                    {
                        foodItems.Remove(foodItems[i]);
                    }
                    else
                    {
                        int availableQuantity = int.MaxValue;

                        foreach (FoodItemIngredientViewModel foodItemIng in temp)
                        {

                            GeneralInventoryViewModel generalInvObj = new GeneralInventoryViewModel();

                            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                            using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories/ingredientid/{foodItemIng.IngredientId}"))
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                if (!ApiAuthorization.IsAuthorized(response))
                                {
                                    return Redirect("/logout");
                                }
                                generalInvObj = JsonConvert.DeserializeObject<GeneralInventoryViewModel>(apiResponse);
                            }

                            int q = (int)(generalInvObj.AvailableQuantity / foodItemIng.Quantity);
                            if (q < availableQuantity)
                            {
                                availableQuantity = q;
                            }
                        }
                        foodItems[i].AvailableQuantity = availableQuantity;

                        bool cont = true;
                        foreach (OrderDetailsViewModel orderDetail in orderDetailsList)
                        {
                            if (foodItems[i].Id == orderDetail.FoodItemId)
                            {
                                foodItems[i].Quantity = orderDetail.Quantity;
                                cont = false;
                            }
                            if (!cont)
                            {
                                break;
                            }
                        }
                        i++;
                    }
                }
            }

            dynamic Context = new ExpandoObject();
            Context.FoodItems = foodItems;
            Context.OrderDetails = orderDetailsList;
            Context.Order = orderObj;
            ViewData["message"] = message;
            message = "";
            return View(Context);
        }

        private async Task UpdateQuantityInGeneralInv(OrderDetailsViewModel orderDetailObj, bool add = false, bool check = true)
        {
            HttpClient httpClient = new HttpClient();
            List<FoodItemIngredientViewModel> temp = new List<FoodItemIngredientViewModel>();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
            using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{orderDetailObj.FoodItemId}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    RedirectToAction("Logout", "Authentication");
                }
                temp = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
            }

            if (check)
            {
                OrderDetailsViewModel orderDetailBeforeChanges = new OrderDetailsViewModel();

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/{orderDetailObj.Id}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        RedirectToAction("Logout", "Authentication");
                    }
                    orderDetailBeforeChanges = JsonConvert.DeserializeObject<OrderDetailsViewModel>(apiResponse);
                }
                if (orderDetailBeforeChanges.Quantity > orderDetailObj.Quantity)
                {
                    add = true;
                }
                else if (orderDetailBeforeChanges.Quantity == orderDetailObj.Quantity)
                {
                    return;
                }
            }
            foreach (FoodItemIngredientViewModel foodItemIng in temp)
            {
                GeneralInventoryViewModel generalInvObj = new GeneralInventoryViewModel();

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories/ingredientid/{foodItemIng.IngredientId}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        RedirectToAction("Logout", "Authentication");
                    }
                    generalInvObj = JsonConvert.DeserializeObject<GeneralInventoryViewModel>(apiResponse);
                }
                if (!add)
                {
                    generalInvObj.AvailableQuantity = (int)(generalInvObj.AvailableQuantity - foodItemIng.Quantity);
                }
                else
                {
                    generalInvObj.AvailableQuantity = (int)(generalInvObj.AvailableQuantity + foodItemIng.Quantity);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(generalInvObj), Encoding.UTF8, "application/json"); ;
                var Response = await httpClient.PutAsync($"{Constants.ApiUrl}generalinventories/{generalInvObj.Id}", content);
                if (!ApiAuthorization.IsAuthorized(Response))
                {
                    RedirectToAction("Logout", "Authentication");
                }
            }
        }

        private async Task<float> CalcEstimatedCost(OrderDetailsViewModel orderDetail)
        {
            float estPrice = 0;
            HttpClient httpClient = new HttpClient();

            List<FoodItemIngredientViewModel> ingredients = new List<FoodItemIngredientViewModel>();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
            using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}fooditemingredients/{orderDetail.FoodItemId}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    RedirectToAction("Logout", "Authentication");
                }
                ingredients = JsonConvert.DeserializeObject<List<FoodItemIngredientViewModel>>(apiResponse);
            }

            for (int i = 0; i < ingredients.Count; i++)
            {
                GeneralInventoryViewModel generalInventoryObj = new GeneralInventoryViewModel();

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}generalinventories/ingredient/{ingredients[i].IngredientId}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        RedirectToAction("Logout", "Authentication");
                    }
                    generalInventoryObj = JsonConvert.DeserializeObject<GeneralInventoryViewModel>(apiResponse);
                }

                int Quantity = (int)(orderDetail.Quantity * ingredients[i].Quantity);
                estPrice += generalInventoryObj.AveragePrice * Quantity;
            }

            return estPrice;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderDetail(OrderDetailsViewModel orderDetail)
        {
            orderDetail.Price = orderDetail.Price * orderDetail.Quantity;

            orderDetail.EstimatedCost = await CalcEstimatedCost(orderDetail);

            List<OrderDetailsViewModel> orderDetailsList = new List<OrderDetailsViewModel>();
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
            using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/order/{orderDetail.OrderId}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }
                orderDetailsList = JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(apiResponse);
            }

            if (orderDetailsList.Count == 0)
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                StringContent content = new StringContent(JsonConvert.SerializeObject(orderDetail), Encoding.UTF8, "application/json"); ;
                var response = await httpClient.PostAsync($"{Constants.ApiUrl}orderdetails", content);
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }

                await UpdateQuantityInGeneralInv(orderDetail, false, false);
            }
            else
            {
                bool cont = true;

                foreach (OrderDetailsViewModel orderDetailObj in orderDetailsList)
                {
                    if (orderDetailObj.FoodItemId == orderDetail.FoodItemId)
                    {
                        orderDetailObj.Quantity = orderDetail.Quantity;
                        orderDetailObj.Price = orderDetail.Price;
                        orderDetailObj.EstimatedCost = orderDetail.EstimatedCost;

                        if (orderDetail.Quantity == 0)
                        {
                            await UpdateQuantityInGeneralInv(orderDetailObj, false);

                            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                            var response = await httpClient.DeleteAsync($"{Constants.ApiUrl}orderdetails/{orderDetailObj.Id}");
                            if (!ApiAuthorization.IsAuthorized(response))
                            {
                                return Redirect("/logout");
                            }

                        }
                        else
                        {
                            await UpdateQuantityInGeneralInv(orderDetailObj);

                            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                            StringContent content = new StringContent(JsonConvert.SerializeObject(orderDetailObj), Encoding.UTF8, "application/json"); ;
                            var response = await httpClient.PutAsync($"{Constants.ApiUrl}orderdetails/{orderDetailObj.Id}", content);
                            if (!ApiAuthorization.IsAuthorized(response))
                            {
                                return Redirect("/logout");
                            }
                        }

                        cont = false;
                    }
                    if (!cont)
                    {
                        break;
                    }
                }

                if (cont)
                {

                    await UpdateQuantityInGeneralInv(orderDetail, false, false);

                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(orderDetail), Encoding.UTF8, "application/json"); ;
                    var response = await httpClient.PostAsync($"{Constants.ApiUrl}orderdetails", content);
                    if (!ApiAuthorization.IsAuthorized(response))
                    {
                        return Redirect("/logout");
                    }
                }

            }

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
            using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orderdetails/order/{orderDetail.OrderId}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }

                orderDetailsList = JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(apiResponse);
            }

            float TotalPrice = 0;
            foreach (OrderDetailsViewModel orderDetailObj in orderDetailsList)
            {
                TotalPrice += orderDetailObj.Price;
            }

            OrderViewModel orderObj = new OrderViewModel();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
            using (var response = await httpClient.GetAsync($"{Constants.ApiUrl}orders/{orderDetail.OrderId}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                if (!ApiAuthorization.IsAuthorized(response))
                {
                    return Redirect("/logout");
                }
                orderObj = JsonConvert.DeserializeObject<OrderViewModel>(apiResponse);
            }
            orderObj.TotalPrice = TotalPrice;

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
            StringContent Content = new StringContent(JsonConvert.SerializeObject(orderObj), Encoding.UTF8, "application/json"); ;
            var Response = await httpClient.PutAsync($"{Constants.ApiUrl}orders/{orderDetail.OrderId}", Content);
            if (!ApiAuthorization.IsAuthorized(Response))
            {
                return Redirect("/logout");
            }

            return RedirectToAction("AddItems", new { id = orderDetail.OrderId });
        }

        public async Task<IActionResult> Delete(int id)
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
            }

            return View(orderObj);
        }
    }
}
