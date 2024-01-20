using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.RepositoryLayer;

using FlyingDutchmanAirlines_Tests.Stubs;

using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines_Tests
{
    [TestClass]
    public class AirportRepositoryTest
    {
        private FlyingDutchmanAirlinesContext_Stub _context;
        private AirportRepository _repository;

        [TestInitialize]
        public async Task Setup()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
                .UseInMemoryDatabase("FlyingDutchman").Options;

            _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

            SortedList<string, Airport> airports = new SortedList<string, Airport>()
            {
                {
                    "LNA",
                    new Airport
                     {
                    AirportId = 1,
                    City = "Luna",
                    Iata = "LNA"
                    }
                },
                  {
                    "PHX",
                    new Airport
                    {
                      AirportId = 2,
                      City = "Phoenix",
                      Iata = "PHX"
                    }
                  },
                  {
                    "DDH",
                    new Airport
                    {
                      AirportId = 3,
                      City = "Bennington",
                      Iata = "DDH"
                    }
                  },
                  {
                    "RDU",
                    new Airport
                    {
                      AirportId = 4,
                      City = "Raleigh-Durham",
                      Iata = "RDU"
                    }
                  }
            };

            _context.Airports.AddRange(airports.Values);
            await _context.SaveChangesAsync();

            _repository = new AirportRepository(_context);
            Assert.IsNotNull(_repository);

        }


        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public async Task GetAirportByID_Success(int airportId)
        {
            Airport airport = await _repository.GetAirportByID(airportId);
            Assert.IsNotNull(airport);

            Airport DbAirport =  _context.Airports.First(x => x.AirportId == airport.AirportId);
            Assert.AreEqual(airport.City, DbAirport.City);
            Assert.AreEqual(airport.Iata, DbAirport.Iata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetAirportById_Fail_InvalidInput()
        {
            using StringWriter outputStream = new StringWriter();
            try
            {
                Console.SetOut(outputStream);
                await _repository.GetAirportByID(-1);
            }
            catch (ArgumentException)
            {
                Assert.IsTrue(outputStream.ToString().Contains("[ERROR]: could not retrieve an Airport with the ID: -1"));
                throw new ArgumentException();
            }
            finally { outputStream.Dispose(); }

        }

        [TestMethod]
        [ExpectedException(typeof(AirportNotFoundException))]
        public async Task GetAirportById_Fail_NoAirportFound()
        {
            await _repository.GetAirportByID(20);
        }

        [TestMethod]
        [ExpectedException(typeof(AirportNotFoundException))]
        public async Task GetAirportById_Fail_DatabaseException()
        {
            await _repository.GetAirportByID(10);
        }
    }
}
