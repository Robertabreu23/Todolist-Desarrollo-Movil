using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoAppClient.Models;
using TodoAppClient.Services;
using TodoAppClient.Views;

namespace TodoAppClient.ViewModels;

public partial class TareasListaViewModel : ObservableObject
{
    private readonly ITareaApi _api;

    public ObservableCollection<Tarea> Tareas { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string? errorMessage;

    public TareasListaViewModel(ITareaApi api)
    {
        _api = api;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            var list = await _api.GetAllAsync();
            Tareas.Clear();
            foreach (var t in list)
                Tareas.Add(t);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error cargando: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsRefreshing = true;
            ErrorMessage = null;
            var list = await _api.GetAllAsync();
            Tareas.Clear();
            foreach (var t in list)
                Tareas.Add(t);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al refrescar: {ex.Message}";
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(Tarea tarea)
    {
        if (tarea is null) return;
        try
        {
            await _api.DeleteAsync(tarea.Id);
            Tareas.Remove(tarea);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al eliminar: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ToggleCompletadaAsync(Tarea tarea)
    {
        if (tarea is null) return;
        try
        {
            var nuevoValor = !tarea.Completada;
            await _api.SetCompletadaAsync(tarea.Id, nuevoValor);
            tarea.Completada = nuevoValor;
            var idx = Tareas.IndexOf(tarea);
            if (idx >= 0)
            {
                Tareas.RemoveAt(idx);
                Tareas.Insert(idx, tarea);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al actualizar: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task NuevaAsync()
    {
        await Shell.Current.GoToAsync(nameof(TareaFormPage));
    }

    [RelayCommand]
    private async Task EditarAsync(Tarea tarea)
    {
        if (tarea is null) return;
        await Shell.Current.GoToAsync($"{nameof(TareaFormPage)}?id={tarea.Id}");
    }
}
