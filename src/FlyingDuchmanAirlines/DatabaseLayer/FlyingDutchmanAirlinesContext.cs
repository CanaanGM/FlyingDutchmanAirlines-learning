using FlyingDuchmanAirlines.DatabaseLayer.Models;

using Microsoft.EntityFrameworkCore;

namespace FlyingDuchmanAirlines.DatabaseLayer;

public class FlyingDutchmanAirlinesContext : DbContext
{
    public FlyingDutchmanAirlinesContext()
    {
    }

    public FlyingDutchmanAirlinesContext(DbContextOptions<FlyingDutchmanAirlinesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = Environment.GetEnvironmentVariable("flyingDuchmanAirlines", EnvironmentVariableTarget.User) ?? string.Empty;
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.AirportId).HasName("PK__Airport__E3DBE08ACA258963");

            entity.ToTable("Airport");

            entity.Property(e => e.AirportId).HasColumnName("AirportID");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Iata)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("IATA");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951ACD7000A586");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Booking__Custome__2F10007B");

            entity.HasOne(d => d.FlightNumberNavigation).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FlightNumber)
                .HasConstraintName("FK__Booking__FlightN__2E1BDC42");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8EA27C37F");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightNumber).HasName("PK__Flight__2EAE6F51C9A9759F");

            entity.ToTable("Flight");

            entity.HasOne(d => d.DestinationNavigation).WithMany(p => p.FlightDestinationNavigations)
                .HasForeignKey(d => d.Destination)
                .HasConstraintName("FK__Flight__Destinat__29572725");

            entity.HasOne(d => d.OriginNavigation).WithMany(p => p.FlightOriginNavigations)
                .HasForeignKey(d => d.Origin)
                .HasConstraintName("FK__Flight__Origin__286302EC");
        });

    }

}
