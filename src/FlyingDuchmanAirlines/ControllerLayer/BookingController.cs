using FlyingDuchmanAirlines.ControllerLayer.JsonData;
using FlyingDuchmanAirlines.Exceptions;
using FlyingDuchmanAirlines.Services;

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
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpPost("{flightNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBooking(int flightNumber, [FromBody] BookingData bookingInfo)
        {
            if(ModelState.IsValid && flightNumber.IsPositive())
            {
                string name = $"{bookingInfo.FirstName} {bookingInfo.LastName}";

                (bool result, Exception ex) = await _bookingService.CreateBooking(name, flightNumber);

                if (result && ex is null)
                    return StatusCode((int)HttpStatusCode.Created);

                return ex is CouldNotAddBookingToDataBaseException
                    ? StatusCode((int)HttpStatusCode.NotFound)
                    : StatusCode((int)HttpStatusCode.InternalServerError, ex.Message)
                    ;


            }

            return StatusCode((int)HttpStatusCode.InternalServerError, ModelState.Root.Errors.First().ErrorMessage);
        }
    }
}
