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
    }
}
