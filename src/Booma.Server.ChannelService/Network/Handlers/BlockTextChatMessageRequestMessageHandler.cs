using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using GladNet;

namespace Booma.Server
{
	/// <summary>
	/// Chat message handler. Client sends: <see cref="BlockTextChatMessageRequestPayload"/>
	/// </summary>
	public sealed class BlockTextChatMessageRequestMessageHandler : GameMessageHandler<BlockTextChatMessageRequestPayload>
	{
		private GGDBFInitializer DataInitializer { get; }

		private IBoomaGGDBFData Data { get; }

		private IServiceResolver<IItemInstanceService> ItemInstanceServiceResolver { get; }

		private IServiceResolver<ICharacterInventoryService> CharacterInventoryServiceResolver { get; }

		private ICharacterActorReferenceContainer ActorReference { get; }

		public BlockTextChatMessageRequestMessageHandler(GGDBFInitializer dataInitializer, 
			IBoomaGGDBFData data, 
			IServiceResolver<IItemInstanceService> itemInstanceServiceResolver, 
			IServiceResolver<ICharacterInventoryService> characterInventoryServiceResolver, 
			ICharacterActorReferenceContainer actorReference)
		{
			if (itemInstanceServiceResolver == null) throw new ArgumentNullException(nameof(itemInstanceServiceResolver));
			DataInitializer = dataInitializer ?? throw new ArgumentNullException(nameof(dataInitializer));
			Data = data ?? throw new ArgumentNullException(nameof(data));
			ItemInstanceServiceResolver = itemInstanceServiceResolver ?? throw new ArgumentNullException(nameof(itemInstanceServiceResolver));
			CharacterInventoryServiceResolver = characterInventoryServiceResolver ?? throw new ArgumentNullException(nameof(characterInventoryServiceResolver));
			ActorReference = actorReference ?? throw new ArgumentNullException(nameof(actorReference));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockTextChatMessageRequestPayload message, CancellationToken token = default)
		{
			if (message.ChatMessage.ToUpper() == "/GGDBF RELOAD")
			{
				await DataInitializer.InitializeAsync(token);
				await context.MessageService.SendMessageAsync(new SharedCreateMessageBoxEventPayload("GGDBF system reloaded."), token);
			}
			else if (message.ChatMessage.ToUpper().Contains("/ADD"))
			{
				//Adding an item I guess
				var itemName = message.ChatMessage
					.ToUpper()
					.Replace("/ADD", "")
					.Trim();

				foreach (var itemTemplate in Data.ItemTemplate.Values)
				{
					if (itemTemplate.VisualName.ToUpper() != itemName)
						continue;

					//Found the item, create the instance
					var itemInstanceServiceResult = await ItemInstanceServiceResolver.Create(token);
					var characterInventoryServiceResult = await CharacterInventoryServiceResolver.Create(token);

					if (!itemInstanceServiceResult.isAvailable || !characterInventoryServiceResult.isAvailable)
						break;

					var instanceCreateResult = await itemInstanceServiceResult.Instance.CreateItemAsync(new RPGCreateItemInstanceRequest(itemTemplate.Id), token);

					if (!instanceCreateResult.isSuccessful)
						break;

					//If we have an unattached instance we can then add to the inventory
					if (!await characterInventoryServiceResult.Instance
						.AddItemAsync(ActorReference.EntityGuid.Identifier, instanceCreateResult.Result.InstanceId, token))
					{
						break;
					}

					//Successfully added
					var item = new InventoryItem((uint)instanceCreateResult.Result.InstanceId, 0, 0, 0);
					item.SetWeaponType((byte)itemTemplate.SubClassId);
					item.ItemData1[2] = (byte)(itemTemplate.Id & 0xFF);

					await context.MessageService.SendMessageAsync(new BlockNetworkCommand60EventServerPayload(new Sub60CreateInventoryItemCommand(0, item.ItemId, item.ItemData1, item.ItemData2)), token);
					break;
				}
			}
		}
	}
}
