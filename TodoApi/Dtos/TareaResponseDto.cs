using TodoApi.Models;

namespace TodoApi.Dtos;

public class TareaResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Completada { get; set; }
    public Prioridad Prioridad { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
}
