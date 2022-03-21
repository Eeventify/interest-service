#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using interest_service.Models;

namespace interest_service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InterestsController : ControllerBase
    {
        private readonly InterestContext _context;

        public InterestsController(InterestContext context)
        {
            _context = context;
        }

        // GET: Interests
        /// <summary>
        /// Get a list of Interests
        /// </summary>
        /// <param name="search">Return only Interests starting with search term (optional)</param>
        /// <param name="sort">Sort results alphabetically on name (default = false)</param>
        /// <returns>A list of all Interests</returns>
        /// <remarks>
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Interest>>> GetInterests(string search = null, bool sort = false)
        {
            var query = from x in _context.Interests
                        select x;

            if (search != null)
            {
                query = query.Where(interest => interest.Name.ToUpper().StartsWith(search.ToUpper()));
            }
            
            if (sort)
            {
                query = query.OrderBy(interest => interest.Name);
            }

            return await query.ToListAsync();
        }

        // GET: Interests/5
        /// <summary>
        /// Get a specific Interest
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The requested Interest</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="404">Item not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Interest>> GetInterest(long id)
        {
            var interest = await _context.Interests.FindAsync(id);

            if (interest == null)
            {
                return NotFound();
            }

            return interest;
        }

        // PUT: Interests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates an Interest
        /// </summary>
        /// <param name="id"></param>
        /// <param name="interest"></param>
        /// <returns>The updated Interest</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Interest
        ///     {
        ///        "id": 1,
        ///        "name": "Tennis",
        ///        "description": "Tennis is a racket sport that can be played individually against a single opponent or between two teams of two players each."
        ///     }
        ///
        /// </remarks>
        /// <response code="204">No Content</response>
        /// <response code="400">If the item is null</response>
        /// <response code="404">Item not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutInterest(long id, Interest interest)
        {
            if (id != interest.Id)
            {
                return BadRequest();
            }

            _context.Entry(interest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InterestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Interests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates an Interest
        /// </summary>
        /// <param name="interest"></param>
        /// <returns>A newly created Interest</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Interest
        ///     {
        ///        "name": "Tennis",
        ///        "description": "Tennis is a racket sport that can be played individually against a single opponent or between two teams of two players each."
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Interest>> PostInterest(Interest interest)
        {
            _context.Interests.Add(interest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInterest), new { id = interest.Id }, interest);
        }

        // DELETE: Interests/5
        /// <summary>
        /// Deletes an Interest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="204">No content</response>
        /// <response code="404">Item not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInterest(long id)
        {
            var interest = await _context.Interests.FindAsync(id);
            if (interest == null)
            {
                return NotFound();
            }

            _context.Interests.Remove(interest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InterestExists(long id)
        {
            return _context.Interests.Any(e => e.Id == id);
        }
    }
}
