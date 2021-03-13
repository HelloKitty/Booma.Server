using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// The Instance actor can be thought of as a Game or Party actor which is just an instance of the game.
	/// </summary>
	public sealed class InstanceActor : BaseEntityActor<EmptyFactoryContext>
	{
		//We have to use specific type because it contains actor type information.
		public InstanceActor(ILog logger, DefaultActorMessageHandlerService<InstanceActor> messageHandlerService)
			: base(logger, messageHandlerService)
		{

		}

		protected override void OnInitialized(EntityActorInitializationSuccessMessage successMessage)
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"{nameof(InstanceActor)} Initialized. Path: {Self.Path}");
		}
	}
}
