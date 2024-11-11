using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SudokuApi.Context;
using SudokuApi.Models;

namespace SudokuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartidaController : ControllerBase
    {
        private readonly SudokuDbContext _context;

        public PartidaController(SudokuDbContext context)
        {
            _context = context;
        }

    
        // POST: api/partida/crear
        [HttpPost("crear")]
        public async Task<ActionResult<Partida>> CrearPartida()
        {
    
            var sudokus = await _context.Sudoku.ToListAsync();
            
            // Verificar si hay Sudokus en la base de datos
            if (!sudokus.Any())
            {
                return NotFound("No hay Sudokus disponibles en la base de datos.");
            }
            
            // Seleccionar un Sudoku aleatorio
            var random = new Random();
            var randomIndex = random.Next(sudokus.Count);
            var sudokuSeleccionado = sudokus[randomIndex];

            
            var nuevaPartida = new Partida
            {
                SudokuID = sudokuSeleccionado.SudokuID,
                EstadoTablero = sudokuSeleccionado.Tablero, 
                TiempoInicio = DateTime.UtcNow, 
                TiempoResolucion = TimeSpan.Zero 
            };

            _context.Partida.Add(nuevaPartida);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CrearPartida), new { id = nuevaPartida.Id }, nuevaPartida);
        }




        // POST: api/partida/{id}/validarSolucion
        [HttpPost("{id}/validarSolucion")]
        public async Task<ActionResult<bool>> ValidarSolucionx(int id)
        {
            var partida = await _context.Partida.FindAsync(id);
            if (partida == null)
            {
                return NotFound("Partida no encontrada.");
            }

            var sudoku = await _context.Sudoku.FindAsync(partida.SudokuID);
            if (sudoku == null)
            {
                return NotFound("Sudoku no encontrado.");
            }

            if (partida.TiempoInicio != null)
            {
                var tiempoActual = DateTime.UtcNow;
                partida.TiempoResolucion += tiempoActual - partida.TiempoInicio;
                partida.TiempoInicio = null;
            }

            _context.Partida.Update(partida);
            await _context.SaveChangesAsync();

        
            if (partida.EstadoTablero == sudoku.Solucion)
            {
                // La solución es correcta, elimina la partida de la base de datos
                _context.Partida.Remove(partida);
                await _context.SaveChangesAsync();

                return Ok($"Solución correcta. \n Tiempo total utilizado: {partida.TiempoResolucion}");
            }
            
            else
            {
                char[] arraySol = partida.EstadoTablero.ToCharArray();
                List<int> posicionesIncorrectas = new List<int>();
                
                for (int i = 0; i < arraySol.Length; i++)
                {
                    if (arraySol[i] != '0' && arraySol[i] != sudoku.Solucion[i])
                    {
                        posicionesIncorrectas.Add(i); 
                        arraySol[i] = 'X'; 
                    }
                }
                return Ok("Solucion incorrecta: " + new string(arraySol));

 
            }
        }



        // POST: api/partida/{id}/guardarEstado
        [HttpPost("{id}/guardarEstado")]
        public async Task<ActionResult<Partida>> GuardarEstado(int id, [FromBody] string estadoTablero)
        {
            var partida = await _context.Partida.FindAsync(id);
            if (partida == null)
            {
                return NotFound("Partida no encontrada.");
            }

            if (partida.TiempoInicio == null)
            {
                return BadRequest("La partida no ha sido iniciada o está pausada.");
            }

            
            var tiempoActual = DateTime.UtcNow;
            partida.TiempoResolucion += tiempoActual - partida.TiempoInicio;

            // Reinicia el tiempo de inicio a null hasta que se vuelva a reanudar
            partida.TiempoInicio = null;

            // Guarda el estado del tablero
            partida.EstadoTablero = estadoTablero;
            _context.Partida.Update(partida);
            await _context.SaveChangesAsync();

            
            return Ok(partida);
        }

        // POST: api/partida/{id}/reanudar
        [HttpPost("{id}/reanudar")]
        public async Task<ActionResult<Partida>> ReanudarPartida(int id)
        {
            var partida = await _context.Partida.FindAsync(id);
            if (partida == null)
            {
                return NotFound("Partida no encontrada.");
            }

            // Reinicia el tiempo de inicio
            partida.TiempoInicio = DateTime.UtcNow;
            _context.Partida.Update(partida);
            await _context.SaveChangesAsync();

            return Ok(partida); 
        }


      


        // ver partidas
        [HttpGet("VerTodasLasPartidas")]
        public async Task<ActionResult<IEnumerable<Partida>>> GetAllPartidas()
        {
            var partidas = await _context.Partida.ToListAsync();
            return Ok(partidas);
        }

    }
}