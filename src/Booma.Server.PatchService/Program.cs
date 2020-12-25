using System;
using System.Net;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace Booma
{
	class Program
	{
		public static IPAddress Address { get; } = IPAddress.Parse("127.0.0.1");

		static async Task Main(string[] args)
		{
			ILog logger = new ConsoleLogger(LogLevel.All, true);
			logger.Info($"Starting Patch Service.");

			try
			{
				await new BoomaPatchServerApplication(new NetworkAddressInfo(Address, 11000), logger)
					.BeginListeningAsync();
			}
			finally
			{
				logger.Warn($"Shutting down Patch Service.");
			}
		}
	}
}
