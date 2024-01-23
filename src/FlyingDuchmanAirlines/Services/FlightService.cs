using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.RepositoryLayer;
using FlyingDuchmanAirlines.Views;

namespace FlyingDuchmanAirlines.Services
{
    public class FlightService
    {
        private readonly FlightRepository _flightRepository;
        private readonly AirportRepository _airportRepository;

        public FlightService(FlightRepository flightRepository, AirportRepository airportRepository)
        {
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
        }

        public virtual async IAsyncEnumerable<FlightView> GetFlights()
        {
            Queue<Flight> flights = _flightRepository.GetFlights();



            foreach (Flight flight in flights)
            {
                Airport originAirport;
                Airport destinationAirport;

                try
                {
                    originAirport = await _airportRepository.GetAirportByID(flight.Origin);
                    destinationAirport = await _airportRepository.GetAirportByID(flight.Destination);
                }
                catch (FlightNotFoundException)
                {
                    throw new FlightNotFoundException();
                }
                catch (Exception)
                {
                    throw new ArgumentException();
                }

                yield return new FlightView(
                    flight.FlightNumber.ToString(),
                        (originAirport.City, originAirport.Iata),
                        (destinationAirport.City, destinationAirport.Iata)
                    );


            }
        }
    }
}
