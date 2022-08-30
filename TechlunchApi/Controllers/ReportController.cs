using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TechlunchApi.Data;

namespace TechlunchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {

        private readonly TechlunchDbContext _context;

        public ReportController(TechlunchDbContext context)
        {
            _context = context;
        }

        [HttpGet("{StartingTime, EndingTime}")]
        public string GetReport(DateTime StartingTime, DateTime EndingTime)
        {
            int TotalOrders = _context.Orders.Where(tbl => tbl.Status == true && tbl.CreatedAt <= EndingTime && tbl.CreatedAt >= StartingTime).Count();
            float TotalSales = _context.Orders.Where(tbl => tbl.Status == true && tbl.CreatedAt <= EndingTime && tbl.CreatedAt >= StartingTime).Sum(p => p.TotalPrice);
            var q =
            from O in _context.Orders
            join OD in _context.OrderDetail on O.Id equals OD.OrderId into OS
            from OD in OS.DefaultIfEmpty()
            where O.CreatedAt <= EndingTime && O.CreatedAt >= StartingTime && O.Status == true
            select OD;
            float TotalCost = q.Sum(p => p.EstimatedCost);
            var report = new
            {
                TotalOrders = TotalOrders,
                TotalSales = TotalSales,
                TotalProfit = TotalSales - TotalCost
            };
            string jsonData = JsonConvert.SerializeObject(report);
            return jsonData;
        }
    }
}
