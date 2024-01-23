using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.RepositoryLayer;
using FlyingDuchmanAirlines.Services;

using Moq;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer
{
    [TestClass]
    public class BookingServiceTests
    {
        private Mock<BookingRepository> _mockBookingRepository;
        private Mock<CustomerRepository> _mockCustomerRepository;
        private Mock<FlightRepository> _mockFlightRepository;

        [TestInitialize]
        public async Task Setup()
        {
            _mockBookingRepository = new Mock<BookingRepository>();
            _mockCustomerRepository = new Mock<CustomerRepository>();
            _mockFlightRepository = new Mock<FlightRepository>();
        }

        [TestMethod]
        public async Task CreateBooking_Success()
        {
            _mockBookingRepository
                .Setup(repository =>
                    repository.CreateBooking(0, 0))
                .Returns(Task.CompletedTask);

            _mockCustomerRepository
                .Setup(repo =>
                    repo.GetCustomerByName("Ruby Sanguine"))
                .Returns(Task.FromResult(new Customer("Ruby Sanguine")));


            _mockFlightRepository
                    .Setup(repo =>
                        repo.GetFlightByFlightNumber(0))
                    .Returns(Task.FromResult(new Flight()));

            BookingService bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockCustomerRepository.Object,
                _mockFlightRepository.Object
                );

            (bool result, Exception? exception) = await bookingService.CreateBooking("Ruby Sanguine", 0);

            Assert.IsTrue(result);
            Assert.IsNull(exception);

        }
        [TestMethod]
        public async Task CreateBooking_Success_CustomerNotInDatabase()
        {
            _mockBookingRepository
                .Setup(repository =>
                    repository.CreateBooking(0, 0))
                .Returns(Task.CompletedTask);

            _mockCustomerRepository
                .Setup(repo =>
                    repo.GetCustomerByName("Ruby Sanguine"))
                .Throws(new CustomerNotFoundException());

            BookingService bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockCustomerRepository.Object,
                _mockFlightRepository.Object
                );

            (bool result, Exception? exception) = await bookingService.CreateBooking("Ruby Sanguine", 0);


            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CustomerNotFoundException));
        }

        [TestMethod]
        public async Task CreateBooking_Failure_CustomerNotInDatabase_RepositoryFailure()
        {
            _mockBookingRepository
                .Setup(repo =>
                    repo.CreateBooking(0, 0))
                .Throws(new CouldNotAddBookingToDataBaseException());

            _mockFlightRepository
                .Setup(repo =>
                    repo.GetFlightByFlightNumber(1))
                .ReturnsAsync(new Flight());

            _mockCustomerRepository
                .Setup(repo =>
                    repo.GetCustomerByName("Ruby Sanguine"))
                .ReturnsAsync(new Customer("Ruby Sanguine"));

            BookingService bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockCustomerRepository.Object,
                _mockFlightRepository.Object
                );

            (bool result, Exception? exception) = await bookingService.CreateBooking("Ruby Sanguine", 0);

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDataBaseException));
        }

        [TestMethod]
        [DataRow("", 0)]
        [DataRow(null, -1)]
        [DataRow("Galileo Galilei", -1)]
        public async Task CreateBooking_Failure_InvalidInputArguments(string name, int flightNumber)
        {

            BookingService bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockCustomerRepository.Object,
                _mockFlightRepository.Object
                );
            (bool result, Exception? exception) = await bookingService.CreateBooking(name, flightNumber);

            Assert.IsFalse(result);
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public async Task CreateBooking_Failure_RepositoryException()
        {

            _mockBookingRepository
                .Setup(repository =>
                    repository.CreateBooking(0, 1))
                .Throws(new ArgumentException());

            _mockCustomerRepository
                .Setup(repo =>
                    repo.GetCustomerByName("Galileo Galilei"))
                .Returns(Task.FromResult(new Customer("Galileo Galilei") { CustomerId = 0 }));

            _mockFlightRepository
                .Setup(repo => repo.GetFlightByFlightNumber(1))
                .ReturnsAsync(new Flight());

            BookingService bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockCustomerRepository.Object,
                _mockFlightRepository.Object
                );

            (bool result, Exception ex) = await bookingService.CreateBooking("Galileo Galilei", 1);

            Assert.IsFalse(result);
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex, typeof(ArgumentException));

        }

        [TestMethod]
        public async Task CreateBooking_Failure_CouldNotAdsBookingToDatabase()
        {
            _mockBookingRepository
                .Setup(repository =>
                    repository.CreateBooking(1, 2))
                .Throws(new CouldNotAddBookingToDataBaseException());

            _mockCustomerRepository
                .Setup(repo =>
                    repo.GetCustomerByName("Ruby Sanguine"))
                .Returns(Task.FromResult(new Customer("Ruby Sanguine") { CustomerId = 1 }));

            BookingService bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockCustomerRepository.Object,
                _mockFlightRepository.Object
                );
            (bool result, Exception ex) = await bookingService.CreateBooking("Ruby Sanguine", 2);

            Assert.IsFalse(result);
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex, typeof(CouldNotAddBookingToDataBaseException));

        }


        [TestMethod]
        public async Task CreateBooking_Failure_FlightNotInDatabase()
        {
            BookingService bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockCustomerRepository.Object,
                _mockFlightRepository.Object
                );

            _mockFlightRepository
                .Setup(repo =>
                    repo.GetFlightByFlightNumber(-1))
                .Throws(new FlightNotFoundException());

            (bool result, Exception ex) = await bookingService.CreateBooking("Ruby Sanguine", 12);
            Assert.IsFalse(result);
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex, typeof(CouldNotAddBookingToDataBaseException));

        }
    }
}
