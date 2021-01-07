using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Booma.UI;
using Glader.Essentials;
using Glader;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	public sealed class ChannelWelcomeEventListener : LoginResponseSentEventListener
	{
		private ICharacterOptionsConfigurationFactory OptionsFactory { get; }

		public ChannelWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService, 
			ICharacterOptionsConfigurationFactory optionsFactory) 
			: base(subscriptionService)
		{
			OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			//Disconnect if not successful
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
			{
				await args.MessageContext.ConnectionService.DisconnectAsync();
				return;
			}

			await args.MessageContext.MessageService.SendMessageAsync(new LobbyListEventPayload(new LobbyMenuEntry[0x0F]
			{
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 0),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 1),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 2),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 3),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 4),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 5),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 6),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 7),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 8),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 9),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 10),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 11),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 12),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 13),
				new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, 14),
			}));

			CharacterOptionsConfiguration configuration = await OptionsFactory.Create(CancellationToken.None);

			await args.MessageContext.MessageService.SendMessageAsync(new InitializeCharacterDataEventPayload(new CharacterInventoryData(0, 0, 0, 1, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), CreateDefaultCharacterData(), 0, new CharacterBankData(0, Enumerable.Repeat(new BankItem(), 200).ToArray()), new GuildCardEntry(1, "Glader", String.Empty, String.Empty, 1, SectionId.Viridia, CharacterClass.HUmar), 0, configuration));
			await args.MessageContext.MessageService.SendMessageAsync(new BlockCharacterDataInitializationServerRequestPayload());
		}

		public static LobbyCharacterData CreateDefaultCharacterData()
		{
			return new LobbyCharacterData(new CharacterStats(Enumerable.Repeat((ushort)100, 7).ToArray()), 0, 0, new CharacterProgress(0, 0), 0, "1", 0, new CharacterSpecialCustomInfo(), SectionId.Viridia, CharacterClass.HUmar, new CharacterVersionData(), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(), new Vector2<float>()), "Glader");
		}
	}
}
