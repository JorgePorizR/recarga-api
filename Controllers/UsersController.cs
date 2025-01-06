using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecargaApi.Data;
using RecargaApi.Models;

namespace RecargaApi.Controllers
{
    /*[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RecargaApiContext _context;

        public UsersController(RecargaApiContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "recarga")]
        public async Task<ActionResult<User>> PostUser(User user)
        {

            // Si el grupo no está presente, retornar 403 Forbidden
            if (!User.IsInRole("recarga"))
            {
                return new JsonResult(new { message = "User does not have the 'recarga' role." }) { StatusCode = StatusCodes.Status403Forbidden };
            }

            // Initialize the wallet balance to 0
            user.WalletBalance = 0;

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }*/
}


// Obtener todas las claims del usuario
/*var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

// Imprimir las claims del usuario para depuración
Console.WriteLine("User Claims:");
foreach (var claim in claims)
{
    Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
}

// Verificar si el usuario tiene el rol "administrador de usuarios" en los grupos
var roleClaims = claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList();
Console.WriteLine($"Roles: {string.Join(", ", roleClaims)}");

// Verificar si el grupo "administrador de usuarios" está en las claims
if (roleClaims.Contains("recarga"))
{
    // Si el grupo está presente, retornar OK
    return Ok(new { message = "User has the 'recarga' role." });
}*/