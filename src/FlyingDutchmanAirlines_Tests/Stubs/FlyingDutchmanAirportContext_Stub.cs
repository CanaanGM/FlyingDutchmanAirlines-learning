using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FlyingDutchmanAirlines_Tests.Stubs
{
    public class FlyingDutchmanAirportContext_Stub : FlyingDutchmanAirlinesContext
    {
        public FlyingDutchmanAirportContext_Stub(DbContextOptions<FlyingDutchmanAirlinesContext> options) : base(options)
        {
            // clear out the database
            base.Database.EnsureDeleted();
        }
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> pendingChanges = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
            IEnumerable<Airport> airports = pendingChanges.Select(x => x.Entity).OfType<Airport>();

            if (!airports.Any(x => x.AirportId == 10)) throw new Exception("dun goofed!");

            await base.SaveChangesAsync(cancellationToken);
            return 1;
        }
    }
}
