﻿using FlyingDuchmanAirlines.DatabaseLayer;
using FlyingDuchmanAirlines.RepositoryLayer;
using FlyingDuchmanAirlines.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FlyingDuchmanAirlines
{
    internal class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(swagger => swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Flying Dutchman Airlines"));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient(typeof(FlightRepository), typeof(FlightRepository));
            services.AddTransient(typeof(AirportRepository), typeof(AirportRepository));
            services.AddTransient(typeof(BookingRepository), typeof(BookingRepository));
            services.AddTransient(typeof(BookingService), typeof(BookingService));
            services.AddTransient(typeof(FlightService), typeof(FlightService));
            services.AddTransient(typeof(CustomerRepository), typeof(CustomerRepository));
            services.AddTransient(typeof(FlyingDutchmanAirlinesContext), typeof(FlyingDutchmanAirlinesContext));
            services.AddSwaggerGen();
        }
    }
}