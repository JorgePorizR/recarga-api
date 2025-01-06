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
    public class MovimientosController : ControllerBase
    {
        private readonly RecargaApiContext _context;

        public MovimientosController(RecargaApiContext context)
        {
            _context = context;
        }

        // GET: api/Movimientos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimiento()
        {
            return await _context.Movimiento.ToListAsync();
        }

        // GET: api/Movimientos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movimiento>> GetMovimiento(int id)
        {
            var movimiento = await _context.Movimiento.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return movimiento;
        }

        // PUT: api/Movimientos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovimiento(int id, Movimiento movimiento)
        {
            if (id != movimiento.Id)
            {
                return BadRequest();
            }

            _context.Entry(movimiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovimientoExists(id))
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

        // POST: api/Movimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movimiento>> PostMovimiento(Movimiento movimiento)
        {
            _context.Movimiento.Add(movimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovimiento", new { id = movimiento.Id }, movimiento);
        }

        // DELETE: api/Movimientos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            var movimiento = await _context.Movimiento.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }

            _context.Movimiento.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Movimientos/recargar
        [HttpPost("recargar")]
        [Authorize(Roles = "recarga")]
        public async Task<ActionResult<Movimiento>> RecargaMovimiento([FromForm] RecargaDto model)
        {
            if (!User.IsInRole("recarga"))
            {
                return new JsonResult(new { message = "User does not have the 'recarga' role." }) { StatusCode = StatusCodes.Status403Forbidden };
            }

            // Si el archivo está presente, procesarlo
            if (model.File == null || model.File.Length == 0)
                return BadRequest("No se ha cargado ningún archivo.");

            // Guardar el archivo en wwwroot/comprobantes
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/comprobantes");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.File.FileName)}"; // Usar un nombre único para el archivo
            var filePath = Path.Combine(folderPath, fileName);

            // Guardar el archivo en el sistema de archivos
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream); // Copiar el contenido del archivo al sistema de archivos
            }

            // Construir la URL del archivo subido
            var comprobanteUrl = $"{Request.Scheme}://{Request.Host}/comprobantes/{fileName}";

            // Crear el movimiento de tipo Ingreso
            var movimiento = new Movimiento
            {
                UserId = model.UserId,
                Monto = model.Monto,
                Tipo = 1, // 1 representa Ingreso
                Descripcion = model.Descripcion,
                Comprobante = comprobanteUrl
            };

            // Agregar el movimiento a la base de datos
            _context.Movimiento.Add(movimiento);
            await _context.SaveChangesAsync();

            // Buscar la billetera del usuario por UserId
            var billetera = await _context.Billetera.FirstOrDefaultAsync(b => b.UserId == model.UserId);
            if (billetera != null)
            {
                billetera.Saldo += model.Monto; // Actualizar el saldo de la billetera
                _context.Entry(billetera).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            // Devolver la respuesta con el movimiento creado
            return CreatedAtAction(nameof(GetMovimiento), new { id = movimiento.Id }, movimiento);
        }

        // POST: api/Movimientos/Ingreso
        [HttpPost("Ingreso")]
        public async Task<ActionResult<Movimiento>> IngresoMovimiento(Movimiento movimiento)
        {
            // Crear el movimiento para salida
            movimiento.Tipo = 1; // 1 representa Ingreso
            movimiento.Comprobante = ""; // No se necesita comprobante para salidas

            _context.Movimiento.Add(movimiento);
            await _context.SaveChangesAsync();

            // buscar la billetera por el userId
            var billetera = await _context.Billetera.FirstOrDefaultAsync(b => b.UserId == movimiento.UserId);
            if (billetera != null)
            {
                billetera.Saldo += movimiento.Monto;
                _context.Entry(billetera).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetMovimiento), new { id = movimiento.Id }, movimiento);
        }

        // POST: api/Movimientos/Salida
        [HttpPost("Salida")]
        public async Task<ActionResult<Movimiento>> SalidaMovimiento(Movimiento movimiento)
        {
            // Crear el movimiento para salida
            movimiento.Tipo = 0; // 0 representa Salida
            movimiento.Comprobante = ""; // No se necesita comprobante para salidas

            // buscar la billetera por el userId
            var billetera = await _context.Billetera.FirstOrDefaultAsync(b => b.UserId == movimiento.UserId);
            if (billetera != null)
            {
                if (billetera.Saldo < movimiento.Monto)
                    return BadRequest("Saldo insuficiente.");
                billetera.Saldo -= movimiento.Monto;
                _context.Entry(billetera).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            _context.Movimiento.Add(movimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovimiento), new { id = movimiento.Id }, movimiento);
        }

        // GET: api/Movimientos/usuario/{userId}
        [HttpGet("usuario/{userId}")]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientoByUserId(int userId)
        {
            var movimientos = await _context.Movimiento
                .Where(m => m.UserId == userId && !string.IsNullOrEmpty(m.Comprobante))
                .ToListAsync();

            if (movimientos == null)
            {
                return NotFound();
            }
            return movimientos;
        }

        private bool MovimientoExists(int id)
        {
            return _context.Movimiento.Any(e => e.Id == id);
        }
    }
}
