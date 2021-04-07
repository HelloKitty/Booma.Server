using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.ASP.GameConfig;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;

namespace Booma
{
	public interface ICharacterOptionsConfigurationFactory : IAsyncFactoryCreatable<CharacterOptionsConfiguration, CancellationToken>
	{

	}

	public sealed class DefaultCharacterOptionsConfigurationFactory : ICharacterOptionsConfigurationFactory
	{
		private IServiceResolver<IKeybindConfigurationService> ConfigServiceResolver { get; }

		public DefaultCharacterOptionsConfigurationFactory(IServiceResolver<IKeybindConfigurationService> configServiceResolver)
		{
			ConfigServiceResolver = configServiceResolver ?? throw new ArgumentNullException(nameof(configServiceResolver));
		}

		public async Task<CharacterOptionsConfiguration> Create(CancellationToken token)
		{
			var result = await ConfigServiceResolver
				.Create(token);

			if (!result.isAvailable)
				throw new InvalidOperationException($"Failed to reach required Service: {nameof(IKeybindConfigurationService)}");

			//We have to ASSUME we have actual keybind data.
			//Exceptional case if we DO NOT
			var keybindData = await result
				.Instance
				.RetrieveAccountBindsAsync(token);

			BindingsConfig bindings = CreateBindingConfig(keybindData.Result);

			//TODO: Send correct GCN
			return new CharacterOptionsConfiguration(bindings, 1, new AccountTeamInformation(0, Array.Empty<uint>(), 0, 0, String.Empty, 0));
		}

		private BindingsConfig CreateBindingConfig(KeybindConfigurationResult keybindConfig)
		{
			if (keybindConfig == null) throw new ArgumentNullException(nameof(keybindConfig));

			return new BindingsConfig(keybindConfig.KeybindData.Take(364).ToArray(), keybindConfig.KeybindData.Skip(364).ToArray());
		}
	}
}
