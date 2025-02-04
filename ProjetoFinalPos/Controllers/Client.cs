using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinalPos.Models;
using ProjetoFinalPos.Repositories;

namespace ProjetoFinalPos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Clients : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Clients(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all clients.
        /// </summary>
        /// <returns>A list of clients.</returns>
        /// <response code="200">Returns the list of clients.</response>
        /// <response code="204">If no clients are found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Client>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var result = await _context.Clients.ToListAsync();

            if (!result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific client by ID.
        /// </summary>
        /// <param name="id">The ID of the client to retrieve.</param>
        /// <returns>The client with the specified ID.</returns>
        /// <response code="200">Returns the client with the specified ID.</response>
        /// <response code="404">If the client is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        /// <summary>
        /// Updates an existing client.
        /// </summary>
        /// <param name="id">The ID of the client to update.</param>
        /// <param name="client">The updated client object.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the client is successfully updated.</response>
        /// <response code="400">If the ID in the URL does not match the ID in the client object.</response>
        /// <response code="404">If the client is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="client">The client object to create.</param>
        /// <returns>The created client.</returns>
        /// <response code="201">Returns the newly created client.</response>
        /// <response code="400">If the client object is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Client), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        /// <summary>
        /// Deletes an existing client.
        /// </summary>
        /// <param name="id">The ID of the client to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the client is successfully deleted.</response>
        /// <response code="404">If the client is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
