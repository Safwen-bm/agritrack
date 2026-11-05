// Built with help from Claude
using AgriTrack.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgriTrack.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Field> Fields => Set<Field>();
    public DbSet<CropType> CropTypes => Set<CropType>();
    public DbSet<Planting> Plantings => Set<Planting>();
    public DbSet<Harvest> Harvests => Set<Harvest>();
    public DbSet<IrrigationLog> IrrigationLogs => Set<IrrigationLog>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Revenue> Revenues => Set<Revenue>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // needed for the Identity tables

        // Starter crop data (the assumptions the calculators will use)
        builder.Entity<CropType>().HasData(
            new CropType { Id = 1, Name = "Olive", SeasonWaterNeedMm = 600, GrowthDays = 240, TypicalYieldKgPerHectare = 3000 },
            new CropType { Id = 2, Name = "Tomato", SeasonWaterNeedMm = 500, GrowthDays = 120, TypicalYieldKgPerHectare = 50000 },
            new CropType { Id = 3, Name = "Wheat", SeasonWaterNeedMm = 450, GrowthDays = 180, TypicalYieldKgPerHectare = 3500 }
        );
    }
}