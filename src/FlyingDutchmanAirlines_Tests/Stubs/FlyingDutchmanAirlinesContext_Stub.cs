using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FlyingDutchmanAirlines_Tests.Stubs
{
    public class FlyingDutchmanAirlinesContext_Stub : FlyingDutchmanAirlinesContext
    {
        public FlyingDutchmanAirlinesContext_Stub(DbContextOptions<FlyingDutchmanAirlinesContext> options) : base(options)
        {
            // clear out the database
            base.Database.EnsureDeleted();
        }
        public async override Task<int> SaveChangesAsync( CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry> pendingChanges = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
            IEnumerable<Booking> bookings = pendingChanges.Select(x => x.Entity).OfType<Booking>();
            if(bookings.Any(b=> b.CustomerId != 1)){
                throw new Exception("Database Error!");
            }


            await base.SaveChangesAsync(cancellationToken);
            return 1;
        }
    }
}
