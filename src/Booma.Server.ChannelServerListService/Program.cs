using System;
using System.Net;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace Booma
{
	class Program : ServiceProgram
	{
		public static IPAddress Address { get; } = IPAddress.Parse("127.0.0.1");

		public static short Port { get; } = 12003;

		static async Task Main(string[] args)
		{
			//Ship is basically a list of channels.
			await StartServiceAsync(BoomaServiceType.ShipService, CreateApplication());
		}

		static BoomaChannelServerListServerApplication CreateApplication()
		{
			ILog logger = new ConsoleLogger(LogLevel.All, true);
			return new BoomaChannelServerListServerApplication(new NetworkAddressInfo(Address, Port), logger);
		}
	}
}
