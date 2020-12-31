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

		public static short Port { get; } = 12001;

		static async Task Main(string[] args)
		{
			await StartServiceAsync(BoomaServiceType.CharacterService, CreateApplication());
		}

		static BoomaCharacterServerApplication CreateApplication()
		{
			ILog logger = new ConsoleLogger(LogLevel.All, true);
			return new BoomaCharacterServerApplication(new NetworkAddressInfo(Address, Port), logger);
		}
	}
}
