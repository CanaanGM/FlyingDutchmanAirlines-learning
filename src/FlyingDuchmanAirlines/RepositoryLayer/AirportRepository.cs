using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyingDuchmanAirlines.RepositoryLayer;

public class AirportRepository
{
    private readonly FlyingDutchmanAirlinesContext _context;

    public AirportRepository(FlyingDutchmanAirlinesContext context)
    {
        _context = context;
    }

    // see compiler method inlining in code c# like a pro chapter 10 - 10.3.3 mocking a class with moq
    [MethodImpl(MethodImplOptions.NoInlining)]
    public AirportRepository()
    {
        if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
        {
            throw new Exception("This constructor should only be used for testing!");
        }
    }
    public virtual async Task<Airport> GetAirportByID(int iD)
    {
        if (!iD.IsPositive())
        {
            Console.WriteLine($"[ERROR]: could not retrieve an Airport with the ID: {iD}");
            throw new ArgumentException("Invalid Arguments provided.");
        }


            return await _context.Airports.FindAsync(iD)
                ?? throw new AirportNotFoundException();

    }

}