using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_Tests.Stubs;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer
{
    [TestClass]
    public class FlightRepositoryTests
    {
        private FlyingDutchmanFlightContext_Stub _context;
        private FlightRepository _repository;


        [TestInitialize]
        public async Task Setup()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
                  .UseInMemoryDatabase("FlyingDutchman")
                  .Options;

            _context = new FlyingDutchmanFlightContext_Stub(dbContextOptions);


            Flight flight = new Flight
            {
                Origin = 1,
                FlightNumber = 1,
                Destination = 2
            };



            Flight flight2 = new Flight
            {
                FlightNumber = 2,
                Origin = 2,
                Destination = 2
            };
            _context.Flights.Add(flight);
            _context.Flights.Add(flight2);
            await _context.SaveChangesAsync();


            _repository = new FlightRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Success()
        {
            Flight flight = await _repository.GetFlightByFlightNumber(1);
            Assert.IsNotNull(flight);

            Flight dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
            Assert.IsNotNull(dbFlight);

            Assert.AreEqual(dbFlight.FlightNumber, flight.FlightNumber);
            Assert.AreEqual(dbFlight.Origin, flight.Origin);
            Assert.AreEqual(dbFlight.Destination, flight.Destination);

        }


        [TestMethod]
        public async Task GetFlights_Success()
        {
            var flights =  _repository.GetFlights();

            Assert.IsNotNull(flights);
            Assert.AreEqual(2, flights.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FlightsNotFoundException))]
        public async Task GetFlights_Failure_NoFlightsInDatabase()
        {
            _context.Database.EnsureDeleted();

            _ = _repository.GetFlights();

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightId_Failure_InvalidFlightNumber()
        {
            await _repository.GetFlightByFlightNumber(-1);

        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_DatabaseError()
        {
            await _repository.GetFlightByFlightNumber(6);
        }
    }
}
