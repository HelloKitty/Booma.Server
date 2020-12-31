using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace Booma
{
	public abstract class ServiceProgram
	{
		/// <summary>
		/// Starts a new service of the provided type based on the provided <see cref="application"/>.
		/// </summary>
		/// <typeparam name="TSessionType"></typeparam>
		/// <param name="serviceType">The service type.</param>
		/// <param name="application">The application instance.</param>
		/// <returns></returns>
		protected static async Task StartServiceAsync<TSessionType>(BoomaServiceType serviceType, 
			BaseBoomaServerApplication<TSessionType> application) 
			where TSessionType : ManagedSession
		{
			if (application == null) throw new ArgumentNullException(nameof(application));
			if (!Enum.IsDefined(typeof(BoomaServiceType), serviceType)) throw new InvalidEnumArgumentException(nameof(serviceType), (int) serviceType, typeof(BoomaServiceType));

			ILog logger = new ConsoleLogger(LogLevel.All, true);
			logger.Info($"Starting {serviceType}.");

			try
			{
				await application
					.BeginListeningAsync();
			}
			finally
			{
				logger.Warn($"Shutting down {serviceType}.");
			}
		}
	}
}
