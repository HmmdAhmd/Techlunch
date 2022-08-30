using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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


        private readonly IApiHelper _apiHelper;

        public OrdersController(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public async Task<IActionResult> Index()
        {
            List<OrderViewModel> orders = await _apiHelper.Get<List<OrderViewModel>>("orders");
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            OrderViewModel orderObj = await _apiHelper.Get<OrderViewModel>($"orders/{id}");
            orderObj.Confirmed = true;
            await _apiHelper.Put<OrderViewModel>(orderObj, $"orders/{id}");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            OrderViewModel orderObj = await _apiHelper.Get<OrderViewModel>($"orders/{id}");
            List<OrderDetailsViewModel> orderDetailsList = await _apiHelper.Get<List<OrderDetailsViewModel>>($"orderdetails/order/{id}");

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

            await _apiHelper.Post<OrderViewModel>(orderObj, "orders");
            orderObj = await _apiHelper.Get<OrderViewModel>($"orders/latest");
            return RedirectToAction("AddItems", "Orders", new { id = orderObj.Id });
        }

        [HttpGet]
        public async Task<IActionResult> AddItems(int id)
        {
            OrderViewModel orderObj = await _apiHelper.Get<OrderViewModel>($"orders/{id}");
            if (orderObj.Confirmed)
            {
                return RedirectToAction("Index");
            }

            List<FoodItemViewModel> foodItems = await _apiHelper.Get<List<FoodItemViewModel>>("fooditems");
            List<OrderDetailsViewModel> orderDetailsList = await _apiHelper.Get<List<OrderDetailsViewModel>>($"orderdetails/order/{id}");

            for (int foodItemIndex = 0; foodItemIndex < foodItems.Count;)
            {
                List<FoodItemIngredientViewModel> temp = await _apiHelper.Get<List<FoodItemIngredientViewModel>>($"fooditemingredients/{foodItems[foodItemIndex].Id}");
                
                if (temp.Count == 0)
                {
                    foodItems.Remove(foodItems[foodItemIndex]);
                }
                else
                {
                    int availableQuantity = int.MaxValue;

                    foreach (FoodItemIngredientViewModel foodItemIng in temp)
                    {

                        GeneralInventoryViewModel generalInvObj  = await _apiHelper.Get<GeneralInventoryViewModel>($"generalinventories/ingredientid/{foodItemIng.IngredientId}");

                        int q = (int)(generalInvObj.AvailableQuantity / foodItemIng.Quantity);
                        if (q < availableQuantity)
                        {
                            availableQuantity = q;
                        }
                    }
                    foodItems[foodItemIndex].AvailableQuantity = availableQuantity;

                    bool cont = true;
                    foreach (OrderDetailsViewModel orderDetail in orderDetailsList)
                    {
                        if (foodItems[foodItemIndex].Id == orderDetail.FoodItemId)
                        {
                            foodItems[foodItemIndex].Quantity = orderDetail.Quantity;
                            cont = false;
                        }
                        if (!cont)
                        {
                            break;
                        }
                    }
                    foodItemIndex++;
                }
            }

            dynamic Context = new ExpandoObject();
            Context.FoodItems = foodItems;
            Context.OrderDetails = orderDetailsList;
            Context.Order = orderObj;
            return View(Context);
        }

        private async Task UpdateQuantityInGeneralInv(OrderDetailsViewModel orderDetailObj, bool add = false, bool check = true)
        {
            HttpClient httpClient = new HttpClient();

            List<FoodItemIngredientViewModel> temp = await _apiHelper.Get<List<FoodItemIngredientViewModel>>($"fooditemingredients/{orderDetailObj.FoodItemId}");

            

            if (check)
            {
                OrderDetailsViewModel orderDetailBeforeChanges = await _apiHelper.Get<OrderDetailsViewModel>($"orderdetails/{orderDetailObj.Id}");
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
                GeneralInventoryViewModel generalInvObj = await _apiHelper.Get<GeneralInventoryViewModel>($"generalinventories/ingredientid/{foodItemIng.IngredientId}");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", Request.Cookies["token"]);
               
                if (!add)
                {
                    generalInvObj.AvailableQuantity = (int)(generalInvObj.AvailableQuantity - foodItemIng.Quantity);
                }
                else
                {
                    generalInvObj.AvailableQuantity = (int)(generalInvObj.AvailableQuantity + foodItemIng.Quantity);
                }

                await _apiHelper.Put<GeneralInventoryViewModel>(generalInvObj, $"generalinventories/{generalInvObj.Id}");

            }
        }

        private async Task<float> CalcEstimatedCost(OrderDetailsViewModel orderDetail)
        {
            float estPrice = 0;
            HttpClient httpClient = new HttpClient();

            List<FoodItemIngredientViewModel> ingredients = await _apiHelper.Get<List<FoodItemIngredientViewModel>>($"fooditemingredients/{orderDetail.FoodItemId}");

            for (int i = 0; i < ingredients.Count; i++)
            {
                GeneralInventoryViewModel generalInventoryObj = await _apiHelper.Get<GeneralInventoryViewModel>($"generalinventories/ingredient/{ingredients[i].IngredientId}");
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

            HttpClient httpClient = new HttpClient();
            List<OrderDetailsViewModel> orderDetailsList = await _apiHelper.Get<List<OrderDetailsViewModel>>($"orderdetails/order/{orderDetail.OrderId}");

            if (orderDetailsList.Count == 0)
            {
                await _apiHelper.Post<OrderDetailsViewModel>(orderDetail, "orderdetails");
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
                            await _apiHelper.Delete($"orderdetails/{orderDetailObj.Id}");
                        }
                        else
                        {
                            await UpdateQuantityInGeneralInv(orderDetailObj);
                            await _apiHelper.Put<OrderDetailsViewModel>(orderDetailObj, $"orderdetails/{orderDetailObj.Id}");
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
                    await _apiHelper.Post<OrderDetailsViewModel>(orderDetail, "orderdetails");
                }

            }


            orderDetailsList = await _apiHelper.Get<List<OrderDetailsViewModel>>($"orderdetails/order/{orderDetail.OrderId}");

            float TotalPrice = 0;
            foreach (OrderDetailsViewModel orderDetailObj in orderDetailsList)
            {
                TotalPrice += orderDetailObj.Price;
            }

            OrderViewModel orderObj = new OrderViewModel();

            orderObj = await _apiHelper.Get<OrderViewModel>($"orders/{orderDetail.OrderId}");
            orderObj.TotalPrice = TotalPrice;


            await _apiHelper.Put<OrderViewModel>(orderObj, $"orders/{orderDetail.OrderId}");
            return RedirectToAction("AddItems", new { id = orderDetail.OrderId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            OrderViewModel orderObj = await _apiHelper.Get<OrderViewModel>($"orders/{id}");
            return View(orderObj);
        }
    }
}
