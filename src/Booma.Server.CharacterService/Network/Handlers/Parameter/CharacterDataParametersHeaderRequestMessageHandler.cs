using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Handler for <see cref="CharacterDataParametersHeaderRequestPayload"/>. Clients send this message when they want to load
	/// the serverside parameter content.
	/// </summary>
	public sealed class CharacterDataParametersHeaderRequestMessageHandler : GameRequestMessageHandler<CharacterDataParametersHeaderRequestPayload, CharacterDataParametersHeaderResponsePayload>
	{
		/// <summary>
		/// Parameter content loading service.
		/// </summary>
		private IParameterContentLoadable ParameterLoader { get; }

		public CharacterDataParametersHeaderRequestMessageHandler(IParameterContentLoadable parameterLoader) 
			: base(true)
		{
			ParameterLoader = parameterLoader ?? throw new ArgumentNullException(nameof(parameterLoader));
		}

		protected override async Task<CharacterDataParametersHeaderResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterDataParametersHeaderRequestPayload message, CancellationToken token = default)
		{
			//For the concept of what this is check documentation on parameter file heads.
			//A reference for concept: https://github.com/Sylverant/login_server/blob/12631a7035c68d6a8a99e3d0f524e7892554e6e7/src/bbcharacter.c#L943
			DataParameterFileHeader[] headers = await ParameterLoader.LoadHeadersAsync();
			return new CharacterDataParametersHeaderResponsePayload(headers);
		}
	}
}
