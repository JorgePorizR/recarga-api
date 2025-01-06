using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecargaApi.Data;
using RecargaApi.Models;

namespace RecargaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BilleterasController : ControllerBase
    {
        private readonly RecargaApiContext _context;

        public BilleterasController(RecargaApiContext context)
        {
            _context = context;
        }

        // GET: api/Billeteras
        [HttpGet]
        //[Authorize(Roles = "recarga")]
        public async Task<ActionResult<IEnumerable<Billetera>>> GetBilletera()
        {
            // Si el grupo no está presente, retornar 403 Forbidden
            //if (!User.IsInRole("recarga"))
            //{
            //   return new JsonResult(new { message = "User does not have the 'recarga' role." }) { StatusCode = StatusCodes.Status403Forbidden };
            //}

            return await _context.Billetera.ToListAsync();
        }

        // GET: api/Billeteras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Billetera>> GetBilletera(int id)
        {
            var billetera = await _context.Billetera.FindAsync(id);

            if (billetera == null)
            {
                return NotFound();
            }

            return billetera;
        }

        // PUT: api/Billeteras/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBilletera(int id, Billetera billetera)
        {
            if (id != billetera.Id)
            {
                return BadRequest();
            }

            _context.Entry(billetera).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BilleteraExists(id))
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

        // POST: api/Billeteras
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Billetera>> PostBilletera(Billetera billetera)
        {
            // Initialize the wallet balance to 0
            billetera.Saldo = 0;

            _context.Billetera.Add(billetera);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBilletera", new { id = billetera.Id }, billetera);
        }

        // DELETE: api/Billeteras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBilletera(int id)
        {
            var billetera = await _context.Billetera.FindAsync(id);
            if (billetera == null)
            {
                return NotFound();
            }

            _context.Billetera.Remove(billetera);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Billeteras/5/User
        [HttpDelete("{id}/User")]
        [Authorize(Roles = "administrador de usuarios")]
        public async Task<IActionResult> DeleteBilleteraByUserId(int id)
        {
            // Si el grupo no está presente, retornar 403 Forbidden
            if (!User.IsInRole("administrador de usuarios"))
            {
                return new JsonResult(new { message = "User does not have the 'administrador de usuarios' role." }) { StatusCode = StatusCodes.Status403Forbidden };
            }
            // find the wallet by user id
            var billetera = await _context.Billetera.FirstOrDefaultAsync(b => b.UserId == id);

            if (billetera == null)
            {
                return NotFound();
            }

            // Eliminar movimientos asociados a la billetera con userId
            var movimientos = await _context.Movimiento.Where(m => m.UserId == billetera.UserId).ToListAsync();
            foreach (var movimiento in movimientos)
            {
                _context.Movimiento.Remove(movimiento);
            }
            await _context.SaveChangesAsync();

            _context.Billetera.Remove(billetera);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Billeteras/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Billetera>>> SearchBilletera(string search)
        {
            var searchLower = search.ToLower();

            var billeteras = await _context.Billetera
                .Where(b => 
                    b.FirstName.ToLower().Contains(searchLower) ||
                    b.LastName.ToLower().Contains(searchLower) ||
                    b.Email.ToLower().Contains(searchLower))
                .ToListAsync();

            if (billeteras == null)
            {
                return NotFound();
            }
            return billeteras;
        }

        // GET: api/Billeteras/usuario/{userId}
        [HttpGet("usuario/{userId}")]
        public async Task<ActionResult<Billetera>> GetBilleteraByUserId(int userId)
        {
            // Buscar la billetera por UserId
            var billetera = await _context.Billetera.FirstOrDefaultAsync(b => b.UserId == userId);

            // Si no se encuentra, devolver 404 NotFound
            if (billetera == null)
            {
                return NotFound(new { message = $"No se encontró una billetera para el UserId {userId}." });
            }

            // Devolver la billetera encontrada
            return billetera;
        }


        private bool BilleteraExists(int id)
        {
            return _context.Billetera.Any(e => e.Id == id);
        }
    }
}

/* imprimir los roles
var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
var roleClaims = claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList();
Console.WriteLine($"Roles: {string.Join(", ", roleClaims)}");
 */
