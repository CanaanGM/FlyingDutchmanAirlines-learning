using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDuchmanAirlines.RepositoryLayer
{
    public class FlightRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        public FlightRepository(FlyingDutchmanAirlinesContext context)
        {
            _context = context;
        }

        // see compiler method inlining in code c# like a pro chapter 10 - 10.3.3 mocking a class with moq
        [MethodImpl(MethodImplOptions.NoInlining)]
        public FlightRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing!");
            }
        }
        public virtual async Task<Flight> GetFlightByFlightNumber(int flightNumber)
        {
            if (!flightNumber.IsPositive())
            {
                Console.WriteLine($"[ERRROR]: invalid arguments in GetFlightByFlightNumber with the provided args: flightNumber = {flightNumber}");
                throw new ArgumentException("Invalid argument provided!");
            }

            return _context.Flights.FirstOrDefault(x => x.FlightNumber == flightNumber)
                    ?? throw new FlightNotFoundException();
        }

        public virtual Queue<Flight> GetFlights()
        {
            Queue<Flight> flights = new Queue<Flight>();
            //_context.Flights.ForEachAsync(x => flights.Enqueue(x));

            foreach (Flight flight in _context.Flights)
            {
                flights.Enqueue(flight);
            }

            return flights.Count > 0 ? flights : throw new FlightsNotFoundException() ;
        }
    }
}
