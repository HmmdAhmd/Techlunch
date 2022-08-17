using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechlunchApi.Data;
using TechlunchApi.Models;

namespace TechlunchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly TechlunchDbContext _context;

        public InventoriesController(TechlunchDbContext context)
        {
            _context = context;
        }

        // GET: api/Inventories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
        {
            return await _context.Inventory.Include(i => i.IngredientFK).ToListAsync();
        }

        // GET: api/Inventories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var inventory = await _context.Inventory.Include(i => i.IngredientFK)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            return Ok(inventory);
        }

        public async Task<bool> AddToGeneralInventory(Inventory inventory)
        {
            var generalInventory = await _context.GeneralInventory.SingleOrDefaultAsync(
                i => i.IngredientId == inventory.IngredientId);

            if (generalInventory == null) // add a new record
            {
                GeneralInventory generalInventoryObj = new GeneralInventory
                {
                    IngredientId = inventory.IngredientId,
                    AvailableQuantity = inventory.Quantity,
                    AveragePrice = inventory.Price / inventory.Quantity
                };
                _context.GeneralInventory.Add(generalInventoryObj);
                
            } else // update existing record
            {
                generalInventory.AvailableQuantity += inventory.Quantity;
                generalInventory.AveragePrice = 
                    (generalInventory.AveragePrice + (inventory.Price / inventory.Quantity)) / 2;
            }

            await _context.SaveChangesAsync();
            return true;
        }
            
        // GET: api/Inventories/Ingredient/5
        [HttpGet("Ingredient/{id}")]
        public async Task<ActionResult<IEnumerable<IngredientHistory>>> GetIngredientHistory(int id)
        {
            var ingredientHistory = await _context.Inventory.Where(i => i.IngredientId == id).Select(i =>
                new IngredientHistory(i.Quantity, i.Price, i.AddedOn)).ToListAsync();

            if (!ingredientHistory.Any())
            {
                var ingredient = await _context.Ingredients.FindAsync(id);
                if (ingredient == null)
                {
                    return BadRequest("Error 400: Ingredient item doesn't exist");
                }
                return NotFound("Error 404: Ingredient history not found");
            }

            return Ok(ingredientHistory);
        }

        // POST: api/Inventories
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {

            var ingredientObj = await _context.Ingredients.FindAsync(inventory.IngredientId);
            
            if (ingredientObj == null || !ingredientObj.Status)
            {
                return NotFound("Error 404: Ingredient item not found");
            }

            await AddToGeneralInventory(inventory);

            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventory", new { id = inventory.Id }, inventory);
        }

    }
}
