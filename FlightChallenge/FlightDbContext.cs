using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightChallenge
{
	public class FlightDbContext : DbContext
	{
		public DbSet<Route> Routes { get; set; }
		public DbSet<Flight> Flights { get; set; }
		public DbSet<Subscription> Subscriptions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"data source=ELES;initial catalog=FlightChallenge;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false");
		}
	}

	public class Route
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int RouteId { get; set; }
		public int OriginCityId { get; set; }
		public int DestinationCityId { get; set; }
		public DateOnly DepartureDate { get; set; }
	}

	public class Flight
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int FlightId { get; set; }
		public int RouteId { get; set; }
		[ForeignKey("RouteId")]
		public Route Route { get; set; }
		public DateTime DepartureTime { get; set; }
		public DateTime ArrivalTime { get; set; }
		public int AirlineId { get; set; }
	}

	[Keyless]
	public class Subscription
	{
		public int AgencyId { get; set; }
		public int OriginCityId { get; set; }
		public int DestinationCityId { get; set; }
	}
}