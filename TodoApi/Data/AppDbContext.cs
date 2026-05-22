using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TareaItem> Tareas => Set<TareaItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TareaItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Descripcion).HasMaxLength(2000);
            entity.Property(t => t.Prioridad).HasConversion<int>();
            entity.HasIndex(t => t.Prioridad);
            entity.HasIndex(t => t.FechaVencimiento);
        });
    }
}
