﻿using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.Services;
using FlyingDuchmanAirlines.Views;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlyingDuchmanAirlines.ControllerLayer
{
    [Route("[controller]")]
    public class FlightController : Controller
    {
        private readonly FlightService _flightService;

        public FlightController(FlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFlights()
        {
            try
            {
                Queue<FlightView> flights = new Queue<FlightView>();

                await foreach (FlightView flight in _flightService.GetFlights())
                {
                    flights.Enqueue(flight);
                }

                return StatusCode((int)HttpStatusCode.OK, flights);
            }
            catch (FlightNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, "No flights were found in the database");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred");
            }
        }

        [HttpGet("{flightNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFlightByFlightNumber(int flightNumber)
        {
            try
            {
                if (!flightNumber.IsPositive())
                    throw new Exception();

                FlightView fight = await _flightService.GetFlightByFlightNumber(flightNumber);
                return  StatusCode((int)HttpStatusCode.OK, fight);
            }
            catch (FlightNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound, "No flight were found in the database");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "Bad request");
            }
        }
    }
}
