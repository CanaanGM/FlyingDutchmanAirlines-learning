using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.RepositoryLayer;
using FlyingDuchmanAirlines.Views;

using System.Reflection;
using System.Runtime.CompilerServices;

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
        // see compiler method inlining in code c# like a pro chapter 10 - 10.3.3 mocking a class with moq
        [MethodImpl(MethodImplOptions.NoInlining)]
        public FlightService()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing!");
            }
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
                        (originAirport.City!, originAirport.Iata!),
                        (destinationAirport.City!, destinationAirport.Iata!)
                    );


            }
        }

        public virtual async Task<FlightView> GetFlightByFlightNumber(int flightNumber)
        {
            try
            {
                Flight flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);
                Airport originAirport = await _airportRepository.GetAirportByID(flight.Origin);
                Airport destinationAirport = await _airportRepository.GetAirportByID(flight.Destination);

                FlightView view = new(
                        flightNumber.ToString(),
                        (
                            originAirport.City!,
                            originAirport.Iata!
                            ),
                        (
                            destinationAirport.City!,
                            destinationAirport.Iata!
                            )
                        );

                return view;
            }
            catch (FlightNotFoundException)
            {
                throw new FlightNotFoundException();
            }
            catch (Exception)
            {

                throw new ArgumentException();
            }
        }
    }
}
