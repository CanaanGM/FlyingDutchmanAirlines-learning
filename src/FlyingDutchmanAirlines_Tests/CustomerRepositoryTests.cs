using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.RepositoryLayer;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines_Tests
{
    [TestClass]
    public class CustomerRepositoryTests
    {
        private FlyingDutchmanAirlinesContext _context = null!;
        private CustomerRepository _repository = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            // initialize the context of the database
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
                new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
                    .UseInMemoryDatabase("FlyingDuchman").Options;
            _context = new FlyingDutchmanAirlinesContext(dbContextOptions);

            _context.Customers.Add(new Customer(name: "Canaan"));
            _context.SaveChanges();
            _repository = new CustomerRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        public async Task CreateCustomer_Success()
        {
            bool result = await _repository.CreateCustomer("Ruby Sanguine");
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(CustomerNotFoundException))]
        public async Task CreateCustomer_Failure_NameIsEmptyString()
        {
            await _repository.CreateCustomer("");
        }

        [TestMethod]
        [DataRow('#')]
        [DataRow('$')]
        [DataRow('%')]
        [DataRow('&')]
        [DataRow('*')]
        [ExpectedException(typeof(CustomerNotFoundException))]
        public async Task CreateCustomer_Failure_NameContainsInvalidCharacter(char invalidCharacter)
        {
            await  _repository.CreateCustomer("Ruby " + invalidCharacter);
        }

        [TestMethod]
        public async Task GetCustomerByName_Success()
        {
            Customer customer = await _repository.GetCustomerByName("Canaan");
            Assert.IsNotNull(customer);

            Customer dbCustomer = _context.Customers.First();

            bool customersAreEquals = customer == dbCustomer;
            Assert.IsTrue(customersAreEquals);
        }

        [TestMethod]
        [ExpectedException(typeof(CustomerNotFoundException))]
        public async Task GetCustomerByName_Failure_DoesntExist()
        {
            await _repository.GetCustomerByName("Cid the Garlean");
        }

    }
}