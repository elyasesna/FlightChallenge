using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Threading.Channels;

namespace FlightChallenge
{
	internal class ChangeDetection
	{
		private readonly FlightDbContext _db;

		public ChangeDetection(FlightDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<FlightOutput>> DetectChanges(DateOnly startDate, DateOnly endDate, int agencyId)
		{
			var routes = await _db.Routes
				.AsNoTracking()
				.Where(f => f.DepartureDate >= startDate && f.DepartureDate <= endDate)
				.Select(p => p.RouteId)
				.ToListAsync();

			if (routes.Count == 0)
			{
				return [];
			}

			var subscribedRoutesq = _db.Subscriptions
			  .AsNoTracking()
			  .Where(s => s.AgencyId == agencyId);

			var flightsq = _db.Flights
				.AsNoTracking()
				.Include(f => f.Route)
				.Where(f => routes.Contains(f.RouteId));

			var flights = await (from f in flightsq
							  join s in subscribedRoutesq
							  on new { f.Route.OriginCityId, f.Route.DestinationCityId } equals new { s.OriginCityId, s.DestinationCityId }
							  select f).ToListAsync();

			// Group flights by route and departure time (rounded to nearest 30 minutes)
			var flightGroups = flights
				 .GroupBy(f => (f.RouteId, RoundedDepartureTime(f.DepartureTime)))
				 .ToDictionary(g => g.Key, g => g.ToList());

			var result = new List<FlightOutput>();

			foreach (var flight in flights)
			{
				var roundedTime = RoundedDepartureTime(flight.DepartureTime);

				// Check for new flights
				if (!flightGroups.ContainsKey((flight.RouteId, roundedTime.AddDays(-7))))
				{
					result.Add(new FlightOutput(
						flight.RouteId,
						flight.Route.OriginCityId,
						flight.Route.DestinationCityId,
						flight.DepartureTime,
						flight.ArrivalTime,
						flight.AirlineId,
						FlightStatus.New));
				}

				// Check for discontinued flights
				if (!flightGroups.ContainsKey((flight.RouteId, roundedTime.AddDays(7))))
				{
					result.Add(new FlightOutput(
						flight.RouteId,
						flight.Route.OriginCityId,
						flight.Route.DestinationCityId,
						flight.DepartureTime,
						flight.ArrivalTime,
						flight.AirlineId,
						FlightStatus.Discontinued));
				}
			}

			return result;
		}

		private DateTime RoundedDepartureTime(DateTime departureTime)
		{
			return new DateTime(departureTime.Year, departureTime.Month, departureTime.Day,
									  departureTime.Hour, (departureTime.Minute / 30) * 30, 0);
		}
	}
}