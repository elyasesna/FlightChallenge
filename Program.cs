using System.Text.Json;

namespace FlightChallenge
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			if (args.Length != 3)
			{
				Console.WriteLine("Usage: FlightChallenge.exe <start-date> <end-date> <agency-id>");
				return;
			}


			if (!DateOnly.TryParse(args[0], out DateOnly startDate) || !DateOnly.TryParse(args[1], out DateOnly endDate) || !int.TryParse(args[2], out int agencyId))
			{
				Console.WriteLine("Invalid arguments.");
				return;
			}

			using var db = new FlightDbContext();

			var detector = new ChangeDetection(db);
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();

			var result = await detector.DetectChanges(startDate, endDate, agencyId);

			stopwatch.Stop();
			Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms, found {result.Count()} options");
			Console.WriteLine(JsonSerializer.Serialize(result));
		}
	}
}
