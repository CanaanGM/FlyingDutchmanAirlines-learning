using FlyingDuchmanAirlines.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDutchmanAirlines_Tests.Views
{
    [TestClass]
    public class FlightViewTests
    {

        [TestMethod]
        public void Constructor_FlightView_Success()
        {
            string flightNumber = "0";
            string originCity = "Divieon";
            string originCityCode = "DVN";

            string destinationCity = "Baldurs Gate";
            string destinationCityCode = "BDGE";

            FlightView view = new FlightView(flightNumber, (originCity, originCityCode), (destinationCity, destinationCityCode));
            Assert.IsNotNull(view);
            Assert.AreEqual(view.FlightNumber, flightNumber);
            Assert.AreEqual(view.Origin.City, originCity);
            Assert.AreEqual(view.Origin.Code, originCityCode);
            Assert.AreEqual(view.Destination.City, destinationCity);
            Assert.AreEqual(view.Destination.Code, destinationCityCode);
        }

        [TestMethod]
        public void ConstructorFlightView_Success_FlightNumber_Null()
        {
            string originCity = "Divieon";
            string originCityCode = "DVN";

            string destinationCity = "Baldurs Gate";
            string destinationCityCode = "BDGE";

            FlightView view = new FlightView(null, (originCity, originCityCode), (destinationCity, destinationCityCode));
            Assert.IsNotNull(view);

            Assert.AreEqual(view.FlightNumber, "No Flight number found");
            Assert.AreEqual(view.Origin.City, originCity);
            Assert.AreEqual(view.Destination.Code, destinationCityCode);

        }

        [TestMethod]
        public void Constructor_AirportInfo_Success_City_EmptyString()
        {
            string destinationCity = string.Empty;
            string destinationCityCode = "SYD";

            AirportInfo airportInfo =
                new AirportInfo((destinationCity, destinationCityCode));
            Assert.IsNotNull(airportInfo);

            Assert.AreEqual(airportInfo.City, "No city found");
            Assert.AreEqual(airportInfo.Code, destinationCityCode);
        }

        [TestMethod]
        public void Constructor_AirportInfo_Success_Code_EmptyString()
        {
            string destinationCity = "Ushuaia";
            string destinationCityCode = string.Empty;

            AirportInfo airportInfo =
                new AirportInfo((destinationCity, destinationCityCode));
            Assert.IsNotNull(airportInfo);

            Assert.AreEqual(airportInfo.City, destinationCity);
            Assert.AreEqual(airportInfo.Code, "No code found");
        }
    }
}
