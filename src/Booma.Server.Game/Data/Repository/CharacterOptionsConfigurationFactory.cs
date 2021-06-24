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
		private IServiceResolver<IGameConfigurationService<PsobbGameConfigurationType>> ConfigServiceResolver { get; }

		public DefaultCharacterOptionsConfigurationFactory(IServiceResolver<IGameConfigurationService<PsobbGameConfigurationType>> configServiceResolver)
		{
			ConfigServiceResolver = configServiceResolver ?? throw new ArgumentNullException(nameof(configServiceResolver));
		}

		public async Task<CharacterOptionsConfiguration> Create(CancellationToken token)
		{
			var result = await ConfigServiceResolver
				.Create(token);

			if (!result.isAvailable)
				throw new InvalidOperationException($"Failed to reach required Service: {nameof(IGameConfigurationService<PsobbGameConfigurationType>)}");

			var keybindData = await result
				.Instance
				.RetrieveConfigAsync(ConfigurationSourceType.Account, PsobbGameConfigurationType.Key, token);

			var joystickData = await result
				.Instance
				.RetrieveConfigAsync(ConfigurationSourceType.Account, PsobbGameConfigurationType.Joystick, token);

			if (!keybindData.isSuccessful || !joystickData.isSuccessful)
				throw new InvalidOperationException($"Failed to query Config: {ConfigurationSourceType.Account} {PsobbGameConfigurationType.Joystick} and {PsobbGameConfigurationType.Key}");

			BindingsConfig bindings = new BindingsConfig(keybindData.Result.Data, joystickData.Result.Data);

			//TODO: Send correct GCN
			return new CharacterOptionsConfiguration(bindings, 1, new AccountTeamInformation(0, Array.Empty<uint>(), 0, 0, String.Empty, 0));
		}
	}
}
