using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightChallenge
{
	internal record FlightOutput(
		int FlightId,
		int OriginCityId,
		int DestinationCityId,
		DateTime DepartureTime,
		DateTime ArrivalTime,
		int AirlineId,
		FlightStatus Status
		);

	public enum FlightStatus : short
	{
		New,
		Discontinued
	}
}
