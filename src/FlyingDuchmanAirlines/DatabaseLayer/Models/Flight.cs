using System;
using System.Collections.Generic;

namespace FlyingDuchmanAirlines.DatabaseLayer.Models;

public partial class Flight
{
    public int FlightNumber { get; set; }

    public int Origin { get; set; }

    public int Destination { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Airport? DestinationNavigation { get; set; }

    public virtual Airport? OriginNavigation { get; set; }
}
