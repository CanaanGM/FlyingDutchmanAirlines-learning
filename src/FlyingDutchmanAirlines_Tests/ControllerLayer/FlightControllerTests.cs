using Azure;

using FlyingDuchmanAirlines.ControllerLayer;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.Services;
using FlyingDuchmanAirlines.Views;

using Microsoft.AspNetCore.Mvc;

using Moq;

using System.Net;

namespace FlyingDutchmanAirlines_Tests.ControllerLayer
{
    [TestClass]
    public class FlightControllerTests
    {

        Mock<FlightService> _flightServiceMock;
        List<FlightView> returnFlightViews;

        [TestInitialize]
        public async Task Setup()
        {
            _flightServiceMock = new Mock<FlightService>();

            returnFlightViews = new List<FlightView>(2)
            {
                new FlightView("1932", ("Baldurs Gate", "BDG"), ("Divieon", "DIVI") ),
                new FlightView("841", ("Divieon", "DIVI"), ("Baldurs Gate", "BGG") ),
            };

            _flightServiceMock
                .Setup(repo =>
                    repo.GetFlights())
                .Returns(FlightViewAsyncGenerator(returnFlightViews));

            _flightServiceMock
                .Setup(repo =>
                    repo.GetFlightByFlightNumber(1))
                .ReturnsAsync(
                    new FlightView(
                        "1",
                        ("Divieon", "DIVI"),
                        ("Baldurs Gate", "BDG")
                        )
                );
        }

        [TestMethod]
        public async Task GetFlights_Success()
        {
            FlightController flightController = new FlightController(_flightServiceMock.Object);
            ObjectResult response = await flightController.GetFlights() as ObjectResult;

            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);

            var content = response.Value as Queue<FlightView>;
            Assert.IsNotNull(content);

            Assert.IsTrue(returnFlightViews.All(flight => content.Contains(flight)));
        }

        [TestMethod]
        public async Task GetFlights_Failure_FlightNotFoundException_404()
        {
            _flightServiceMock
                .Setup(repo => repo.GetFlights())
                .Throws(new FlightNotFoundException());

            FlightController flightController = new FlightController(_flightServiceMock.Object);
            ObjectResult response = await flightController.GetFlights() as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual("No flights were found in the database", response.Value);
        }

        [TestMethod]
        public async Task GetFlights_Failure_ArgumentExeption_500()
        {
            _flightServiceMock
                .Setup(repo => repo.GetFlights())
                .Throws(new Exception());

            FlightController flightController = new FlightController(_flightServiceMock.Object);
            ObjectResult response = await flightController.GetFlights() as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual("An error occurred", response.Value);
        }



        [TestMethod]
        public async Task GetFlightByFlightNuymber_Success()
        {
            FlightController flightController = new FlightController(_flightServiceMock.Object);
            ObjectResult response = await flightController.GetFlightByFlightNumber(1) as ObjectResult;

            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);

            FlightView flightView = response.Value as FlightView;
            Assert.IsNotNull(flightView);
            Assert.AreEqual("1", flightView.FlightNumber);
            Assert.AreEqual("Baldurs Gate", flightView.Destination.City);
            Assert.AreEqual("BDG", flightView.Destination.Code);
            Assert.AreEqual("DIVI", flightView.Origin.Code);
            Assert.AreEqual("Divieon", flightView.Origin.City);

        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Failure_FlightNotFoundException_404()
        {
            _flightServiceMock
                .Setup(repo => repo.GetFlightByFlightNumber(1))
                .Throws(new FlightNotFoundException());

            FlightController flightController = new FlightController(_flightServiceMock.Object);
            ObjectResult response = await flightController.GetFlightByFlightNumber(1) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual("No flight were found in the database", response.Value);
        }



        [TestMethod]
        [DataRow(1)]
        [DataRow(-1)]
        public async Task GetFlightById_Failure_ArgumentException_400(int flightNumber)
        {
            _flightServiceMock
                .Setup(repo => repo.GetFlightByFlightNumber(1))
                .Throws(new ArgumentException());

            FlightController flightController = new FlightController(_flightServiceMock.Object);
            ObjectResult response = await flightController.GetFlightByFlightNumber(flightNumber) as ObjectResult;
            Assert.IsNotNull(response);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("Bad request", response.Value);
        }

        private async IAsyncEnumerable<FlightView> FlightViewAsyncGenerator(IEnumerable<FlightView> views)
        {
            foreach (var view in views)
            {
                yield return view;
            }
        }
    }
}
