using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FlyingDutchmanAirlines_Tests.Stubs
{
    public class FlyingDutchmanFlightContext_Stub : FlyingDutchmanAirlinesContext
    {
        public FlyingDutchmanFlightContext_Stub(DbContextOptions<FlyingDutchmanAirlinesContext> options) : base(options)
        {
            base.Database.EnsureDeleted();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            IEnumerable<EntityEntry> pendingChanges = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
            IEnumerable<Flight> flights = pendingChanges.Select(x => x.Entity).OfType<Flight>();

            if (flights.Any(x => x.FlightNumber == 6))
                throw new Exception("Database caught the flu!");

            await base.SaveChangesAsync(cancellationToken);
            return 1;
        }
    }
}
