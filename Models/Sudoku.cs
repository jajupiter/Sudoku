namespace SudokuApi.Models;
public class Sudoku
{
    public int SudokuID { get; set; }   
    public string? Tablero { get; set; }     // Representación del tablero del Sudoku en formato NVARCHAR(81)
    public string? Solucion { get; set; }    // Solución del Sudoku en formato NVARCHAR(81)

    // Relación con la entidad 'Partida' (1 Sudoku puede tener muchas Partidas)
    public List<Partida>? Partidas { get; set; } 
}
