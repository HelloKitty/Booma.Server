using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	[ActorMessageHandler(typeof(InstanceActor))]
	public sealed class PlayerJoinInstanceRequestMessageHandler : ActorRequestMessageHandler<PlayerJoinInstanceRequest, ResponseModel<string, PlayerJoinInstanceResponseCode>>
	{
		private IActorFactory<InstanceCharacterActor> InstanceCharacterFactory { get; }

		public PlayerJoinInstanceRequestMessageHandler(IActorFactory<InstanceCharacterActor> instanceCharacterFactory)
		{
			InstanceCharacterFactory = instanceCharacterFactory ?? throw new ArgumentNullException(nameof(instanceCharacterFactory));
		}

		/// <inheritdoc />
		protected override async Task<ResponseModel<string, PlayerJoinInstanceResponseCode>> HandleRequestAsync(EntityActorMessageContext context, PlayerJoinInstanceRequest message, CancellationToken token = default)
		{
			var character = InstanceCharacterFactory.Create(new ActorCreationContext(context.ActorContext));

			return new ResponseModel<string, PlayerJoinInstanceResponseCode>(character.Actor.Path.ToString());
		}
	}
}
