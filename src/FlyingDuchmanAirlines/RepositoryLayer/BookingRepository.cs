using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDuchmanAirlines.RepositoryLayer
{
    public class BookingRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        public BookingRepository(FlyingDutchmanAirlinesContext context)
        {
            _context = context;
        }
        // see compiler method inlining in code c# like a pro chapter 10 - 10.3.3 mocking a class with moq
        [MethodImpl(MethodImplOptions.NoInlining)]
        public BookingRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing!");
            }
        }
        public virtual async Task CreateBooking(int customerID, int flightNumber)
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
