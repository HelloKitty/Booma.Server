using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Booma.Content;

namespace Booma
{
	/// <summary>
	/// Service container that registers a <see cref="IParameterContentLoadable"/> implementation.
	/// </summary>
	public sealed class ParameterContentServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<FileStorageParameterContentLoader>()
				.As<IParameterContentLoadable>()
				.SingleInstance();
		}
	}
}
