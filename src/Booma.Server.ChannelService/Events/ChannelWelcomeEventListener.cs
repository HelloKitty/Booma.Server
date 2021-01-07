using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Booma;
using Booma.UI;
using Common.Logging;
using Glader.Essentials;
using Glader;
using Glader.ASP.ServiceDiscovery;
using GladNet;
using MEAKKA;

namespace Booma
{
	public sealed class ChannelWelcomeEventListener : LoginResponseSentEventListener
	{
		private ICharacterOptionsConfigurationFactory OptionsFactory { get; }

		private IEntityActorRef<RootChannelActor> ChannelActor { get; }

		private ILog Logger { get; }

		public ChannelWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService, 
			ICharacterOptionsConfigurationFactory optionsFactory, 
			IEntityActorRef<RootChannelActor> channelActor, 
			ILog logger) 
			: base(subscriptionService)
		{
			OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
			ChannelActor = channelActor ?? throw new ArgumentNullException(nameof(channelActor));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			//Disconnect if not successful
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
			{
				await args.MessageContext.ConnectionService.DisconnectAsync();
				return;
			}

			if (!await SendLobbyListAsync(args.MessageContext)) 
				return;

			CharacterOptionsConfiguration configuration = await OptionsFactory.Create(CancellationToken.None);

			await args.MessageContext.MessageService.SendMessageAsync(new InitializeCharacterDataEventPayload(new CharacterInventoryData(0, 0, 0, 1, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), CreateDefaultCharacterData(), 0, new CharacterBankData(0, Enumerable.Repeat(new BankItem(), 200).ToArray()), new GuildCardEntry(1, "Glader", String.Empty, String.Empty, 1, SectionId.Viridia, CharacterClass.HUmar), 0, configuration));
			await args.MessageContext.MessageService.SendMessageAsync(new BlockCharacterDataInitializationServerRequestPayload());
		}

		private async Task<bool> SendLobbyListAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context)
		{
			try
			{
				var lobbyData = (await ChannelActor.Actor
						.Ask<LobbyListResponseMessage>(new LobbyListRequestMessage(), TimeSpan.FromSeconds(15)))
					.Select(l => new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, (uint) l.LobbyId))
					.ToArray();

				if (Logger.IsDebugEnabled)
					Logger.Debug($"Sending Lobby Data Count: {lobbyData.Length}");

				await context.MessageService.SendMessageAsync(new LobbyListEventPayload(lobbyData));
			}
			catch (Exception e)
			{
				if (Logger.IsErrorEnabled)
					Logger.Error($"Failed to send LobbyList. Reason: {e}");

				await context.ConnectionService.DisconnectAsync();
				return false;
			}

			return true;
		}

		public static LobbyCharacterData CreateDefaultCharacterData()
		{
			return new LobbyCharacterData(new CharacterStats(Enumerable.Repeat((ushort)100, 7).ToArray()), 0, 0, new CharacterProgress(0, 0), 0, "1", 0, new CharacterSpecialCustomInfo(), SectionId.Viridia, CharacterClass.HUmar, new CharacterVersionData(), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(), new Vector2<float>()), "Glader");
		}
	}
}
