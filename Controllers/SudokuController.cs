using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SudokuApi.Context;
using SudokuApi.Models;

namespace SudokuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SudokuController : ControllerBase
    {
        private readonly SudokuDbContext _context;

        public SudokuController(SudokuDbContext context)
        {
            _context = context;
        }

        // GET: api/sudoku
        [HttpGet ("Ver todos los sudokus")]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult<IEnumerable<Sudoku>>> GetAllSudokus()
        {
            return await _context.Sudoku.Include(s => s.Partidas).ToListAsync();
        }

        // GET: api/sudoku/{id}
        [HttpGet("Ver sudoku por {id}")]
        public async Task<ActionResult<Sudoku>> GetSudokuById(int id)
        {
            var sudoku = await _context.Sudoku.FindAsync(id);

            if (sudoku == null)
            {
                return NotFound();
            }

            return sudoku;
        }
    }

}

