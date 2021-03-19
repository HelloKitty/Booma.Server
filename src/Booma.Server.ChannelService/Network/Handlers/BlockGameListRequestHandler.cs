using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Booma.UI;
using Common.Logging;
using Glader.ASP.RPG;
using Glader.Essentials;
using GladNet;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Message handler for <see cref="BlockGameListRequestPayload"/>.
	/// Responds with <see cref="BlockGameListResponsePayload"/>.
	/// </summary>
	public sealed class BlockGameListRequestHandler : GameRequestMessageHandler<BlockGameListRequestPayload, BlockGameListResponsePayload>
	{
		private IServiceResolver<IGroupManagementService> GroupManagementService { get; }

		private IBoomaEntityNameDictionary EntityNameDictionary { get; }

		public BlockGameListRequestHandler(IServiceResolver<IGroupManagementService> groupManagementService, 
			IBoomaEntityNameDictionary entityNameDictionary) 
			: base(true)
		{
			GroupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
			EntityNameDictionary = entityNameDictionary ?? throw new ArgumentNullException(nameof(entityNameDictionary));
		}

		protected override async Task<BlockGameListResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockGameListRequestPayload message, CancellationToken token = default)
		{
			ServiceResolveResult<IGroupManagementService> result = await GroupManagementService.Create(token);

			//Episode is for SOME REASON calculated like this
			//pkt->entries[entries].episode = (l->max_clients << 4) | l->episode;
			if (result.isAvailable)
			{
				RPGGroupData[] groupDatas = await result.Instance.RetrieveGroupsAsync(token);
				return new BlockGameListResponsePayload(await groupDatas.Select(ConvertRPGGroupToPSOBBGroupEntry).ToArrayAsync());
			}
			else
				return new BlockGameListResponsePayload(Array.Empty<GameListEntry>());
		}

		private async Task<GameListEntry> ConvertRPGGroupToPSOBBGroupEntry(RPGGroupData group)
		{
			if (group == null) throw new ArgumentNullException(nameof(group));

			string groupName = await EntityNameDictionary.QueryEntityNameAsync(new NetworkEntityGuid(EntityType.Group, group.Id));

			//new GameListEntry(new MenuItemIdentifier((int)KnownMenuIdentifier.GAME_TYPE, 1), 0x22 + DifficultyType.Normal, 1, "Test", (EpisodeType)(4 << 4 | (int)EpisodeType.EpisodeI), 0)
			return new GameListEntry(new MenuItemIdentifier((int) KnownMenuIdentifier.GAME_TYPE, (uint) group.Id), 0x22 + DifficultyType.Normal, 1, groupName, (EpisodeType) (4 << 4 | (int) EpisodeType.EpisodeI), 0);
		}
	}
}
