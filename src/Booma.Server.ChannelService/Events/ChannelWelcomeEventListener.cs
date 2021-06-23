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
using Glader.ASP.Authentication;
using Glader.ASP.RPG;
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

		private IServiceResolver<ICharacterDataQueryService> CharacterDataServiceResolver { get; }

		//TODO: ASSERT INSTANCE PER LIFETIME SCOPE!!
		private IAuthTokenRepository TokenRepository { get; }

		/// <summary>
		/// The service resolver for an <see cref="IAuthenticationService"/>.
		/// </summary>
		private IServiceResolver<IAuthenticationService> AuthenticationServiceResolver { get; }

		public ChannelWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService, 
			ILog logger, 
			ILobbyEntryService lobbyEntryService, 
			IEntityActorRef<RootChannelActor> channelActor, 
			IServiceResolver<ICharacterDataQueryService> characterDataServiceResolver, 
			IAuthTokenRepository tokenRepository, 
			IServiceResolver<IAuthenticationService> authenticationServiceResolver) 
			: base(subscriptionService)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			LobbyEntryService = lobbyEntryService ?? throw new ArgumentNullException(nameof(lobbyEntryService));
			ChannelActor = channelActor ?? throw new ArgumentNullException(nameof(channelActor));
			CharacterDataServiceResolver = characterDataServiceResolver ?? throw new ArgumentNullException(nameof(characterDataServiceResolver));
			TokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
			AuthenticationServiceResolver = authenticationServiceResolver ?? throw new ArgumentNullException(nameof(authenticationServiceResolver));
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			//Disconnect if not successful
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
			{
				await args.MessageContext.ConnectionService.DisconnectAsync();
				return;
			}

			//Now we must update our token to be a subaccount claim token
			ServiceResolveResult<IAuthenticationService> authServiceQuery = await AuthenticationServiceResolver.Create(CancellationToken.None);

			//TODO: Logging
			if (!authServiceQuery.isAvailable)
			{
				await args.MessageContext.ConnectionService.DisconnectAsync();
				return;
			}

			var secondaryAuthResult = await AuthenticateSubAccountClaimAsync(args.LoginRequest, CancellationToken.None, authServiceQuery.Instance);

			if (secondaryAuthResult != AuthenticationResponseCode.LOGIN_93BB_OK)
			{
				if (Logger.IsInfoEnabled)
					Logger.Info($"Failed to SubAccount Authenticate User: {args.LoginRequest.UserName} Reason: {secondaryAuthResult}");

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

		private async Task<AuthenticationResponseCode> AuthenticateSubAccountClaimAsync(SharedLoginRequest93Payload message, CancellationToken token, IAuthenticationService authService)
		{
			//In the case that we're past CharacterSelection stage we should actually auth the character
			//as the sub-account
			if(message.Stage <= SharedLoginRequest93Payload.SessionStage.CharacterSelection)
				return AuthenticationResponseCode.LOGIN_93BB_OK;

			var serviceQueryResult = await CharacterDataServiceResolver.Create(token);

			if(!serviceQueryResult.isAvailable)
				return AuthenticationResponseCode.LOGIN_93BB_MAINTENANCE;

			int[] characters = await serviceQueryResult.Instance.RetrieveCharacterBasicListAsync(token);

			if(message.SelectedSlot >= characters.Length)
				return AuthenticationResponseCode.LOGIN_93BB_NO_USER_RECORD;

			JWTModel authResult = await authService.AuthenticateAsync(new AuthenticationRequest(message.UserName, message.Password), characters[message.SelectedSlot], token);

			if(!authResult.isTokenValid)
				return AuthenticationResponseCode.LOGIN_93BB_BAD_USER_PWD2;

			TokenRepository.Update(authResult.AccessToken);
			return AuthenticationResponseCode.LOGIN_93BB_OK;
		}
	}
}
