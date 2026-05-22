namespace TodoAppClient.Models;

public class Tarea
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Completada { get; set; }
    public Prioridad Prioridad { get; set; } = Prioridad.Normal;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
}

public class TareaCreate
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Prioridad Prioridad { get; set; } = Prioridad.Normal;
    public DateTime? FechaVencimiento { get; set; }
}

public class TareaUpdate
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Completada { get; set; }
    public Prioridad Prioridad { get; set; } = Prioridad.Normal;
    public DateTime? FechaVencimiento { get; set; }
}

public class CompletadaPatch
{
    public bool Completada { get; set; }
}
