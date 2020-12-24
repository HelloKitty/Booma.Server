using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Booma.Proxy;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Patch service message handler service module.
	/// Registers the services and handling for message handlers.
	/// </summary>
	public sealed class PatchMessageHandlerServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Handlers AND handler service should be stateless so we only need single instance.
			builder
				.RegisterType<DefaultMessageHandlerService<PSOBBPatchPacketPayloadClient, SessionMessageContext<PSOBBPatchPacketPayloadServer>>>()
				.As<IMessageHandlerService<PSOBBPatchPacketPayloadClient, SessionMessageContext<PSOBBPatchPacketPayloadServer>>>()
				.OnActivated(args =>
				{
					//TODO: Autodiscovery handlers via reflection.
					//Bind one of the default handlers
					args.Instance.Bind<PSOBBPatchPacketPayloadClient>(new DefaultPatchMessageHandler(args.Context.Resolve<ILog>()));
				})
				.SingleInstance();
		}
	}
}
