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
    public class IngredientsController : ControllerBase
    {
        private readonly TechlunchDbContext _context;

        public IngredientsController(TechlunchDbContext context)
        {
            _context = context;
        }

        // GET: api/Ingredients

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredients()
        {
            return await _context.Ingredients.Where(ingredient => ingredient.Status == true).OrderByDescending(ingredient => ingredient.Id).ToListAsync();
        }

        // GET: api/Ingredients/5

        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null || !ingredient.Status)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        // POST: api/Ingredients

        [HttpPost]
        public async Task<ActionResult<Ingredient>> PostIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIngredient", new { id = ingredient.Id }, ingredient);
        }

        // DELETE: api/Ingredients/5

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null || !ingredient.Status)
            {
                return NotFound();
            }

            ingredient.Status = false;
            await _context.SaveChangesAsync();

            return Ok();
        }


        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(ingredient => ingredient.Id == id && ingredient.Status);
        }

        // PUT: api/Ingredients/5

        [HttpPut("{id}")]
        public async Task<IActionResult> EditIngredient(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return BadRequest();
            }

            var Ingredient = await _context.Ingredients.FindAsync(id);
            if (Ingredient == null || !Ingredient.Status)
            {
                return NotFound();
            }

            Ingredient.Name = ingredient.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetIngredient", new { id = id }, Ingredient);
        }
    }
}
