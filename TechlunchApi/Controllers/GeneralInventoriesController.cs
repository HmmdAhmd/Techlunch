﻿using System;
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
    public class GeneralInventoriesController : ControllerBase
    {
        private readonly TechlunchDbContext _context;

        public GeneralInventoriesController(TechlunchDbContext context)
        {
            _context = context;
        }

        // GET: api/GeneralInventories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeneralInventory>>> GetGeneralInventory()
        {
            return await _context.GeneralInventory.Include(i => i.IngredientFK).ToListAsync();
        }

        // GET: api/GeneralInventories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeneralInventory>> GetGeneralInventory(int id)
        {
            var generalInventory = await _context.GeneralInventory.Include(i => i.IngredientFK)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (generalInventory == null)
            {
                return NotFound();
            }

            return Ok(generalInventory);
        }

        // GET: api/GeneralInventories/Ingredient/5
        [HttpGet("Ingredient/{id}")]
        public async Task<ActionResult<IngredientHistory>> GetIngredientHistory(int id)
        {
            var ingredientHistory = await _context.GeneralInventory.
                SingleOrDefaultAsync(i => i.IngredientId == id);

            if (ingredientHistory == null)
            {
                var ingredient = await _context.Ingredients.FindAsync(id);
                if (ingredient == null)
                {
                    return BadRequest("Error 400: Ingredient item doesn't exist");
                }
                return NotFound("Error 404: Ingredient history not found in general inventory");
            }

            return Ok(ingredientHistory);
        }
    }
}