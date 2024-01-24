using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
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
            Queue<Flight> mockReturn = new Queue<Flight>(1);

            Flight flightForQueue = new Flight
            {
                FlightNumber = 123,
                Origin = 32,
                Destination = 92
            };
            _mockAirportRepository
                .Setup(repo =>
                    repo.GetAirportByID(32))
                .ReturnsAsync(
                    new Airport
                    {
                        AirportId = 32,
                        City = "Baldurs Gate",
                        Iata = "BDG"
                    }
                );

            _mockAirportRepository
                .Setup(repo =>
                    repo.GetAirportByID(92))
                .ReturnsAsync(
                    new Airport
                    {
                        AirportId = 92,
                        City = "Divieon",
                        Iata = "DVN"
                    }
                );

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

            mockReturn.Enqueue(flightForQueue);

            _mockFlightRepository
                .Setup(repo =>
                    repo.GetFlights())
                .Returns(mockReturn);

            _mockFlightRepository
                .Setup(repo =>
                    repo.GetFlightByFlightNumber(148))
                .Returns(Task.FromResult(flightForQueue));

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

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlights_Failure_RepositoryException()
        {

            _mockAirportRepository
                .Setup(repo =>
                    repo.GetAirportByID(32))
                .ThrowsAsync(new FlightNotFoundException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

            await foreach (FlightView _ in service.GetFlights())
            {
                ;
            }
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlights_Failure_ReqularException()
        {


            _mockAirportRepository
                    .Setup(repo =>
                        repo.GetAirportByID(32))
                    .ThrowsAsync(new NullReferenceException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

            await foreach (FlightView _ in service.GetFlights())
            {
                ;
            }
        }


        [TestMethod]
        public async Task GetFlightByFlightNumber_Success()
        {


            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

            FlightView flightView = await service.GetFlightByFlightNumber(148);

            Assert.IsNotNull(flightView);
            Assert.AreEqual(flightView.FlightNumber, "148");
            Assert.AreEqual(flightView.Origin.City, "Baldurs Gate");
            Assert.AreEqual(flightView.Origin.Code, "BDG");
            Assert.AreEqual(flightView.Destination.City, "Divieon");
            Assert.AreEqual(flightView.Destination.Code, "DVN");
        }
    }
}
