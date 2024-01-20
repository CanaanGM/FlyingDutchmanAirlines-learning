using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.DatabaseLayer.Models;
using FlyingDuchmanAirlines.Exceptions;

namespace FlyingDuchmanAirlines.RepositoryLayer;

public class AirportRepository
{
    private readonly FlyingDutchmanAirlinesContext _context;

    public AirportRepository(FlyingDutchmanAirlinesContext context)
    {
        _context = context;
    }

    public async Task<Airport> GetAirportByID(int iD)
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