using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace FlyingDuchmanAirlines.RepositoryLayer
{
    public class CustomerRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        public CustomerRepository(FlyingDutchmanAirlinesContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateCustomer(string name)
        {
            if (IsInvalidCustomerName(name))
                throw new CustomerNotFoundException();

            try
            {
                Customer newCustomer = new Customer(name);
                using (_context)
                {
                    _context.Customers.Add(newCustomer);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {

                return false;
            }
            return true;
        }

        public async Task<Customer> GetCustomerByName(string name)
        {
            if (IsInvalidCustomerName(name)) throw new CustomerNotFoundException();

            return await _context.Customers.FirstOrDefaultAsync(x=> x.Name == name) ?? throw new CustomerNotFoundException();



        }

        private bool IsInvalidCustomerName(string name)
        {
            char[] forbiddenCharacters = { '!', '@', '#', '$', '%', '&', '*' };
            return string.IsNullOrEmpty(name) || name.Any(x => forbiddenCharacters.Contains(x));
        }
    }
}
