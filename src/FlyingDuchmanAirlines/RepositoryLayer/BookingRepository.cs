using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;

namespace FlyingDuchmanAirlines.RepositoryLayer
{
    public class BookingRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        public BookingRepository(FlyingDutchmanAirlinesContext context)
        {
            _context = context;
        }

        public async Task CreateBooking(int customerID, int flightNumber)
        {

            if (!customerID.IsPositive() || !flightNumber.IsPositive())
            {
                Console.WriteLine($"Argument Exception in CreateBooking! CustomerID = {customerID}, FlightNumber = {flightNumber}.");
                throw new ArgumentException("Invalid arguments provided.");
            }

            Booking newBooking = new()
            {
                CustomerId = customerID,
                FlightNumber = flightNumber
            };

            try
            {
                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR]: exception during database query: {ex.Message}");

                throw new CouldNotAddBookingToDataBaseException();
            }
        }
    }
}
