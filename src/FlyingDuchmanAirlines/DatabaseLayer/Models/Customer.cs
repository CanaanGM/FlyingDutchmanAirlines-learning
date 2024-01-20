using System;
using System.Collections.Generic;

namespace FlyingDuchmanAirlines.DatabaseLayer.Models;

public sealed class Customer
{

    public int CustomerId { get; set; }

    public string? Name { get; set; }

    public ICollection<Booking> Bookings { get; set; }

    public Customer(string name)
    {
        Name = name;
        Bookings = new List<Booking>();
    }

    public static bool operator ==(Customer x, Customer y)
    {
        CustomerEqualityComparer comparer = new CustomerEqualityComparer();
        return comparer.Equals(x, y);
    }

    public static bool operator !=(Customer x, Customer y) => !(x == y);
}
