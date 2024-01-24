# FlyingDutchmans

## [Code like a pro in c#](https://www.goodreads.com/book/show/54968076-code-like-a-pro-in-c) follow along!

### run

- set `flyingDuchmanAirlines` environment variable OS wide and make it be the database connection string u wanna use.
- run a database container
    - `docker run -p 5432:5432 -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin1234 --name flying-dutchman postgres`
- add in the tables and some dummy data:

```sql
create table Customer(
	CustomerID int primary key identity, 
	Name varchar(255)
);
create table Airport (
	AirportID int primary key identity,
	City varchar(255),
	IATA varchar(64)
);

CREATE table Flight (
	FlightNumber int primary key identity,
	Origin int,
	Destination int,
	
	foreign key (Origin) references Airport(AirportID),
	foreign key (Destination) references Airport(AirportID)
);

create table Booking (
	BookingID int primary key identity,
	FlightNumber int ,
	CustomerID int, 
	
	foreign key(FlightNumber) references Flight(FlightNumber),
	foreign key(CustomerID) references Customer(CustomerID)
);



insert into Airport  values ('Baldurs Gate', 'BDG'), ('Redgrave', 'REG');
insert into Flight values (1,2), (2,1)
insert into Booking values (1, 1)
```

- run to scaffold the models for the database (not needed as they're already generated):
    -  `dotnet ef dbcontext scaffold [CONNECTION STRING] [DATABASE DRIVER] [FLAGS]`
	- `dotnet ef dbcontext scaffold [connection string] Microsoft.EntityFrameworkCore.[SqlServer ||other driver] --context-dir DatabaseLayer --output-dir DatabaseLay  
er/Models`


- build, run -> go to `http://[host]:[port]/swagger`
