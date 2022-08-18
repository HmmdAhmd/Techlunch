using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechlunchApi.Data;
using TechlunchApi.Models;

namespace TechlunchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            float TotalSales = _context.Orders.Where(tbl => tbl.Status == true && tbl.CreatedAt <= EndingTime && tbl.CreatedAt >= StartingTime).Sum(p=>p.TotalPrice);
            
            //it depends upon the task 7.1.1 (Cost Estimation Function)
            float Cost = 0;

            var report = new
            {
                TotalOrders = TotalOrders,
                TotalSales = TotalSales,
                TotalProfit = TotalSales - Cost
            };
            string jsonData = JsonConvert.SerializeObject(report);
            return jsonData;
        }
    }
}
