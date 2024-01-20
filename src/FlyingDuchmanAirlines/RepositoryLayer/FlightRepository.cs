using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Flight> GetFlightByFlightNumber(int flightNumber, int originAirport, int destinationAirportId)
        {
            if (!flightNumber.IsPositive() || !originAirport.IsPositive() || !destinationAirportId.IsPositive())
            {
                Console.WriteLine($"[ERRROR]: invalid arguments in GetFlightByFlightNumber with the provided args: originAirport = {originAirport}, destinationAirportId = {destinationAirportId}");
                throw new ArgumentException("Invalid argument provided!");
            }

            return _context.Flights.FirstOrDefault(x => x.FlightNumber == flightNumber)
                    ?? throw new FlightNotFoundException();
        }
    }
}
