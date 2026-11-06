using Microsoft.EntityFrameworkCore;
using Mka.Api.Models;

namespace Mka.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(e =>
        {
            e.Property(x => x.Tag).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Tag).IsUnique();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Type).HasMaxLength(100);
            e.Property(x => x.Location).HasMaxLength(100);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

            e.ToTable(t => t.HasCheckConstraint(
                "CK_Equipment_Status",
                "[Status] IN ('Operational', 'UnderMaintenance', 'OutOfService')"));

            e.HasData(
                new Equipment
                {
                    Id = 1,
                    Tag = "P-101",
                    Name = "Crude Transfer Pump",
                    Type = "Pump",
                    Location = "CPF",
                    Status = EquipmentStatus.Operational,
                    InstalledOn = new DateOnly(2019, 5, 12)
                },
                new Equipment
                {
                    Id = 2,
                    Tag = "C-201",
                    Name = "Gas Compressor",
                    Type = "Compressor",
                    Location = "CPF",
                    Status = EquipmentStatus.UnderMaintenance,
                    InstalledOn = new DateOnly(2020, 3, 1)
                });
        });

        modelBuilder.Entity<WorkOrder>(w =>
        {
            w.Property(x => x.Title).HasMaxLength(200).IsRequired();
            w.Property(x => x.Description).HasMaxLength(2000);
            w.Property(x => x.AssignedTo).HasMaxLength(100);
            w.Property(x => x.Priority).HasConversion<string>().HasMaxLength(20);
            w.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

            w.HasOne<Equipment>()
             .WithMany()
             .HasForeignKey(x => x.EquipmentId)
             .OnDelete(DeleteBehavior.Restrict);

            w.ToTable(t =>
            {
                t.HasCheckConstraint("CK_WorkOrders_Status",
                    "[Status] IN ('Open', 'InProgress', 'Completed', 'Cancelled')");
                t.HasCheckConstraint("CK_WorkOrders_Priority",
                    "[Priority] IN ('Low', 'Medium', 'High', 'Critical')");
            });
        });
    }
}