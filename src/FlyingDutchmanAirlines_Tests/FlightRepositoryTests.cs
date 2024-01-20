using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.RepositoryLayer;

using FlyingDutchmanAirlines_Tests.Stubs;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines_Tests
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

            Flight flight = new Flight
            {
                Origin = 1,
                FlightNumber = 1,
                Destination = 2
            };


            _context = new FlyingDutchmanFlightContext_Stub(dbContextOptions);

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            _repository = new FlightRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Success()
        {
            Flight flight = await _repository.GetFlightByFlightNumber(1,1,2);
            Assert.IsNotNull(flight);

            Flight dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
            Assert.IsNotNull(dbFlight);

            Assert.AreEqual(dbFlight.FlightNumber, flight.FlightNumber);
            Assert.AreEqual(dbFlight.Origin, flight.Origin);
            Assert.AreEqual(dbFlight.Destination, flight.Destination);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightId_Failure_InvalidOriginAirport()
        {
            await _repository.GetFlightByFlightNumber(1, -1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightId_Failure_InvalidDestinationAirport()
        {
            await _repository.GetFlightByFlightNumber(1, 1, -1);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightId_Failure_InvalidFlightNumber()
        {
            await _repository.GetFlightByFlightNumber(-1, 1, 1);

        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_DatabaseError()
        {
            await _repository.GetFlightByFlightNumber(6, 6, 6);
        }
    }
}
