using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechlunchApi.Data;
using TechlunchApi.Models;

namespace TechlunchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoodItemsController : ControllerBase
    {
        private readonly TechlunchDbContext _context;
        public FoodItemsController(TechlunchDbContext context)
        {
            _context = context;
        }

        // GET: api/FoodItems

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItems()
        {
            return await _context.FoodItems
                .Where(foodItem => foodItem.Status == true).OrderByDescending(foodItem => foodItem.Id).ToListAsync();
        }

        // GET: api/FoodItems/5

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodItem>> GetFoodItem(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);

            if (foodItem == null)
            {
                return NotFound();
            }
            return foodItem;
        }


        // POST: api/FoodItems

        [HttpPost]
        public async Task<ActionResult<FoodItem>> PostFoodItem(FoodItem foodItem)
        {
            _context.FoodItems.Add(foodItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFoodItem", new { id = foodItem.Id }, foodItem);
        }

        // DELETE: api/FoodItems/5

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFoodItem(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound();
            }

            foodItem.Status = false;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
