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
    public class FoodItemIngredientsController : ControllerBase
    {
        private readonly TechlunchDbContext _context;
        public FoodItemIngredientsController(TechlunchDbContext context)
        {
            _context = context;
        }
        // GET: api/FoodItemIngredients/5
        //5 will be the Food Item Id

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<FoodItemIngredients>>> GetFoodItemIngredients(int id)
        {
            return await _context.FoodItemIngredients.Include(c => c.IngredientFK).Where(x => x.FoodItemId == id).ToListAsync();
        }
        // POST: api/FoodItemIngredients
        // To protect from overposting attacks, enable the specific properties you want to bind to, for

        [HttpPost]
        public async Task<ActionResult<FoodItemIngredients>> PostFoodItemIngredients(FoodItemIngredients foodItemIngredients)
        {
            var foodItemIngredient = _context.FoodItemIngredients
                          .Where(tbl => tbl.IngredientId == foodItemIngredients.IngredientId &&
                                        tbl.FoodItemId == foodItemIngredients.FoodItemId)
                          .SingleOrDefault();
            //check if ingredient is already added against food item so update the quanitiy otherwise add new record
            if (foodItemIngredient == null)
            {
                _context.FoodItemIngredients.Add(foodItemIngredients);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetFoodItemIngredients", new { id = foodItemIngredients.Id }, foodItemIngredients);
            }
            else
            {
                foodItemIngredient.Quantity = foodItemIngredient.Quantity + foodItemIngredients.Quantity;
                await _context.SaveChangesAsync();
                return foodItemIngredient;
            }
        }
        // DELETE: api/FoodItemIngredients/5

        [HttpDelete("{id}")]
        public async Task<ActionResult<FoodItemIngredients>> DeleteFoodItemIngredients(int id)
        {
            /*this is hard delete because this table contains many to many relationship of ingredient
             and food item, so if we want to remove ingredient from food item so it should be hard
             deleted from the database*/
            var foodItemIngredients = await _context.FoodItemIngredients.FindAsync(id);
            if (foodItemIngredients == null)
            {
                return NotFound();
            }
            _context.FoodItemIngredients.Remove(foodItemIngredients);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}