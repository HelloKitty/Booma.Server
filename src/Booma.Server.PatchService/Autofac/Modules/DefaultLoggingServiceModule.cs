using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Logging;

namespace Booma
{
	/// <summary>
	/// Default logging module that registers <see cref="ILog"/>.
	/// </summary>
	public sealed class DefaultLoggingServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			ILog logger = new ConsoleLogger(LogLevel.All, true);

			builder.RegisterInstance(logger)
				.As<ILog>()
				.SingleInstance();
		}
	}
}
