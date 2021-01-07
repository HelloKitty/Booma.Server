using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class RootChannelActor : BaseEntityActor<EmptyFactoryContext>
	{
		//We have to use specific type because it contains actor type information.
		public RootChannelActor(ILog logger, DefaultActorMessageHandlerService<RootChannelActor> messageHandlerService)
			: base(logger, messageHandlerService)
		{

		}

		protected override void OnInitialized(EntityActorInitializationSuccessMessage successMessage)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info($"Channel Actor Initialized. Path: {Self.Path}");
		}
	}
}
