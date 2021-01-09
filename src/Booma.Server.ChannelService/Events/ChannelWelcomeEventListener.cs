using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka;
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
		private IEntityActorRef<RootChannelActor> ChannelActor { get; }

		private ILog Logger { get; }

		private ILobbyEntryService LobbyEntryService { get; }

		public ChannelWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService, 
			ILog logger, 
			ILobbyEntryService lobbyEntryService, 
			IEntityActorRef<RootChannelActor> channelActor) 
			: base(subscriptionService)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			LobbyEntryService = lobbyEntryService ?? throw new ArgumentNullException(nameof(lobbyEntryService));
			ChannelActor = channelActor ?? throw new ArgumentNullException(nameof(channelActor));
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

			await LobbyEntryService.TryEnterLobbyAsync(args.MessageContext, args.CharacterSlot, 0);
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
	}
}
