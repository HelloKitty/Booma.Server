using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	[ActorMessageHandler(typeof(InstanceActor))]
	public sealed class PlayerEnterWorldRequestMessageHandler : BaseActorMessageHandler<JoinWorldRequestMessage>
	{
		private IActorState<IActorMessageBroadcaster<WorldActorGroupType>> MessageBroadcaster { get; }

		//TODO: This is a demo hack
		private int demo_id_hack = -1;

		public PlayerEnterWorldRequestMessageHandler(IActorState<IActorMessageBroadcaster<WorldActorGroupType>> messageBroadcaster)
		{
			MessageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, JoinWorldRequestMessage message, CancellationToken token = new CancellationToken())
		{
			//TODO: This is a demo hack
			int id = Interlocked.Increment(ref demo_id_hack);

			//TODO: We need to go through the COMPLEX process of creating a GameLobby actor in AKKA and transfering our session's
			//actor from the current lobby (see lobby change/switch code) and create a new Player actor in the GameLobby
			context.Sender.TellEntity(new SendGamePacketMessage(new BlockGameJoinEventPayload((byte)id, 0, new GameSettings(DifficultyType.Normal, false, 0, SectionId.Viridia, false, 0, EpisodeType.EpisodeI, false), BuildPlayerInformationHeaders(id))));

			//Let all other players know about the joiner before we join the broadcast group ourselves
			MessageBroadcaster.Data.BroadcastMessage(WorldActorGroupType.Players, new SendGamePacketMessage(new BlockGamePlayerJoinedEventPayload((byte)id, 0, CreateJoinData(id))));

			MessageBroadcaster.Data.AddToGroup(WorldActorGroupType.Players, context.Sender);
			message.Answer(context.Sender, WorldJoinResponseCode.Success);
		}

		private CharacterJoinData CreateJoinData(int id)
		{
			return new CharacterJoinData(new PlayerInformationHeader((uint)id + 1, id, "Unknown"), new CharacterInventoryData(0, 0, 0, 1, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), new LobbyCharacterData(CreateEmptyStats(), 0, 0, new CharacterProgress((uint) 0, 1), 0, String.Empty, 0, new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), SectionId.Viridia, CharacterClass.HUmar,
				new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)), "Unknown"));
		}

		private PlayerInformationHeader[] BuildPlayerInformationHeaders(int id)
		{
			var headers = new PlayerInformationHeader[id];

			for (int i = 0; i < id; i++)
				headers[i] = new PlayerInformationHeader((uint) (i + 1), i, "Unknown");

			return headers;
		}

		private static CharacterStats CreateEmptyStats()
		{
			return new CharacterStats(Enumerable.Repeat((ushort)1, 7).ToArray());
		}
	}
}
