
namespace SudokuApi.Models;
public class Partida
{
    public int Id { get; set; }                // Clave primaria
    public int SudokuID { get; set; }          // Clave foránea que referencia a 'Sudoku'
    public string? EstadoTablero { get; set; }  // Estado actual del tablero, en formato NVARCHAR(81)
    public DateTime? TiempoInicio { get; set; } // Fecha y hora en la que empezó la partida
    public TimeSpan? TiempoResolucion { get; set; }  // Tiempo total de resolución (puede ser nulo si no está finalizado)
}

