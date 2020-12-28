﻿using System;
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
			logger.Info($"Starting Character Service.");

			try
			{
				//Ship List port for some reason is 12002
				await new BoomaGameServerListServerApplication(new NetworkAddressInfo(Address, 12002), logger)
					.BeginListeningAsync();
			}
			finally
			{
				logger.Warn($"Shutting down Character Service.");
			}
		}
	}
}
