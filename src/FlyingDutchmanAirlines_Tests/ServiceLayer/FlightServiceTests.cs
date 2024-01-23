using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.RepositoryLayer;
using FlyingDuchmanAirlines.Services;
using FlyingDuchmanAirlines.Views;

using FlyingDutchmanAirlines_Tests.Stubs;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer
{
    [TestClass]
    public class FlightServiceTests
    {
        private FlyingDutchmanFlightContext_Stub _context;
        private Mock<FlightRepository> _mockFlightRepository;
        private FlightRepository _repository;
        private Mock<AirportRepository> _mockAirportRepository;

        [TestInitialize]
        public async Task Setup()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
                new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
                    .UseInMemoryDatabase("FlyingDutchman")
                    .Options;

            _context = new FlyingDutchmanFlightContext_Stub(dbContextOptions);

            _mockFlightRepository = new Mock<FlightRepository>();

            _mockAirportRepository = new Mock<AirportRepository>();


            Flight flight = new Flight
            {
                FlightNumber = 1,
                Origin = 1,
                Destination = 1

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
        public async Task GetFlights_Success()
        {
            Flight flightInDatabase = new Flight
            {
                FlightNumber = 148,
                Origin = 31,
                Destination = 92
            };

            Queue<Flight> mockReturn = new Queue<Flight>(1);
            mockReturn.Enqueue(flightInDatabase);

            _mockFlightRepository.Setup(repository =>
           repository.GetFlights()).Returns(mockReturn);

            _mockAirportRepository.Setup(repository =>
           repository.GetAirportByID(31)).ReturnsAsync(new Airport
           {
               AirportId = 31,
               City = "Mexico City",
               Iata = "MEX"
           });

            _mockAirportRepository.Setup(repository =>
           repository.GetAirportByID(92)).ReturnsAsync(new Airport
           {
               AirportId = 92,
               City = "Ulaanbaataar",
               Iata = "UBN"
           });
            FlightService service = new FlightService(_mockFlightRepository.Object,
           _mockAirportRepository.Object);

            await foreach (FlightView flightView in service.GetFlights())
            {
                Assert.IsNotNull(flightView);
                Assert.AreEqual(flightView.FlightNumber, "148");
                Assert.AreEqual(flightView.Origin.City, "Mexico City");
                Assert.AreEqual(flightView.Origin.Code, "MEX");
                Assert.AreEqual(flightView.Destination.City, "Ulaanbaataar");
                Assert.AreEqual(flightView.Destination.Code, "UBN");
            }
        }

    }
}
