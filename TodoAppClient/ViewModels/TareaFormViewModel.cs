using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoAppClient.Models;
using TodoAppClient.Services;

namespace TodoAppClient.ViewModels;

[QueryProperty(nameof(Id), "id")]
public partial class TareaFormViewModel : ObservableObject
{
    private readonly ITareaApi _api;

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string? descripcion;

    [ObservableProperty]
    private bool completada;

    [ObservableProperty]
    private Prioridad prioridad = Prioridad.Normal;

    [ObservableProperty]
    private DateTime fechaVencimientoDate = DateTime.Today;

    [ObservableProperty]
    private TimeSpan fechaVencimientoTime = TimeSpan.FromHours(12);

    [ObservableProperty]
    private bool tieneVencimiento;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    public bool IsEditMode => Id > 0;
    public string Titulo => IsEditMode ? "Editar tarea" : "Nueva tarea";

    public IReadOnlyList<Prioridad> Prioridades { get; } = new[]
    {
        Prioridad.Baja,
        Prioridad.Normal,
        Prioridad.Alta,
        Prioridad.Urgente
    };

    public TareaFormViewModel(ITareaApi api)
    {
        _api = api;
    }

    partial void OnIdChanged(int value)
    {
        OnPropertyChanged(nameof(IsEditMode));
        OnPropertyChanged(nameof(Titulo));
        if (value > 0)
            _ = LoadAsync(value);
    }

    private async Task LoadAsync(int tareaId)
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            var tarea = await _api.GetByIdAsync(tareaId);
            if (tarea is null)
            {
                ErrorMessage = "Tarea no encontrada";
                return;
            }
            Nombre = tarea.Nombre;
            Descripcion = tarea.Descripcion;
            Completada = tarea.Completada;
            Prioridad = tarea.Prioridad;
            if (tarea.FechaVencimiento.HasValue)
            {
                var local = tarea.FechaVencimiento.Value.ToLocalTime();
                FechaVencimientoDate = local.Date;
                FechaVencimientoTime = local.TimeOfDay;
                TieneVencimiento = true;
            }
            else
            {
                TieneVencimiento = false;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            ErrorMessage = "El nombre es obligatorio";
            return;
        }
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            DateTime? venc = null;
            if (TieneVencimiento)
            {
                var local = FechaVencimientoDate.Date + FechaVencimientoTime;
                venc = local.ToUniversalTime();
            }

            if (IsEditMode)
            {
                var dto = new TareaUpdate
                {
                    Nombre = Nombre.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion.Trim(),
                    Completada = Completada,
                    Prioridad = Prioridad,
                    FechaVencimiento = venc
                };
                await _api.UpdateAsync(Id, dto);
            }
            else
            {
                var dto = new TareaCreate
                {
                    Nombre = Nombre.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion.Trim(),
                    Prioridad = Prioridad,
                    FechaVencimiento = venc
                };
                await _api.CreateAsync(dto);
            }
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al guardar: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
