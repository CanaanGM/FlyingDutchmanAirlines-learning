using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.RepositoryLayer;

using System.Runtime.ExceptionServices;

namespace FlyingDuchmanAirlines.Services
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly FlightRepository _flightRepository;

        public BookingService(
            BookingRepository bookingRepository,
            CustomerRepository customerRepository,
            FlightRepository flightRepository
            )
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _flightRepository = flightRepository;
        }

        public async Task<(bool, Exception?)> CreateBooking(string customerName, int flightNumber)
        {

            if (string.IsNullOrEmpty(customerName) || !flightNumber.IsPositive())
                return (false, new ArgumentException());

            try
            {
                Customer customer = await GetCustomerFromDatabase(customerName)
                    ?? await AddCustomerToDatabase(customerName);

                if (!await FlightExistsInDatabase(flightNumber))
                    throw new CouldNotAddBookingToDataBaseException();


                try
                {
                    customer = await _customerRepository.GetCustomerByName(customerName);
                }
                catch (CustomerNotFoundException)
                {
                    await _customerRepository.CreateCustomer(customerName);
                    return await CreateBooking(customerName, flightNumber);
                }

                await _bookingRepository.CreateBooking(customer.CustomerId, flightNumber);
                return (true, null);

            }
            catch (Exception ex)
            {
                return (false, ex);
            }


        }

        private async Task<bool> FlightExistsInDatabase(int flightNumber)
        {
            try
            {
                return await _flightRepository.GetFlightByFlightNumber(flightNumber) is not null;
            }
            catch (FlightNotFoundException)
            {

                return false;
            }
        }

        private async Task<Customer> GetCustomerFromDatabase(string name)
        {
            try
            {
                return await _customerRepository.GetCustomerByName(name);
            }
            catch (CustomerNotFoundException)
            {
                return null;
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException ?? new Exception()).Throw();
                return null;
            }

        }

        private async Task<Customer> AddCustomerToDatabase(string name)
        {
            await _customerRepository.CreateCustomer(name);
            return await _customerRepository.GetCustomerByName(name);
        }
    }
}
