using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess;

public class StudentContext
    : DbContext
{
    public DbSet<PromoCode> PromoCodes { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Preference> Preferences { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public StudentContext()
    {

    }

    public StudentContext(DbContextOptions<StudentContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerPreference>(entity => 
        { 
            entity.HasKey(bc => new { bc.CustomerId, bc.PreferenceId });

            entity.HasOne(bc => bc.Customer)
                  .WithMany(b => b.CustomerPreferences)
                  .HasForeignKey(bc => bc.CustomerId);

            entity.HasOne(bc => bc.Preference)
                  .WithMany(bc => bc.CustomerPreferences)
                  .HasForeignKey(bc => bc.PreferenceId); 
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(50);
            entity.Property(x => x.ServiceInfo).HasMaxLength(200);
            entity.Property(x => x.PartnerName).HasMaxLength(200);

            entity.HasOne(bc => bc.Customer)
                  .WithMany(b => b.PromoCodes)
                  .HasForeignKey(bc => bc.CustomerId);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.FirstName).HasMaxLength(200);
            entity.Property(x => x.LastName).HasMaxLength(200);
            entity.Property(x => x.Email).HasMaxLength(200);
        });
    }
}