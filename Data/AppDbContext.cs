using Microsoft.EntityFrameworkCore;
using DigiturnoAML.Models;

namespace DigiturnoAML.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Tecnico> Tecnicos => Set<Tecnico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Numero).IsRequired().HasMaxLength(20);
            entity.Property(t => t.Categoria).IsRequired().HasMaxLength(100);
            entity.Property(t => t.NombreEmpleado).IsRequired().HasMaxLength(150);
            entity.Property(t => t.CargoEmpleado).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Descripcion).HasMaxLength(500);
            entity.Property(t => t.Estado).HasConversion<string>();
        });

        modelBuilder.Entity<Tecnico>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.Username).IsUnique();
            entity.Property(t => t.Username).IsRequired().HasMaxLength(80);
            entity.Property(t => t.NombreCompleto).IsRequired().HasMaxLength(150);
        });

        // Seed: técnico administrador por defecto
        modelBuilder.Entity<Tecnico>().HasData(new Tecnico
        {
            Id = 1,
            Username = "admin",
            // Contraseña: Admin1234! (cambiar en producción)
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
            NombreCompleto = "Administrador TI"
        });
    }
}
    