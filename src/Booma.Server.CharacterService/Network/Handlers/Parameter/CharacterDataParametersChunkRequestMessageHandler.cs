using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Message handler for <see cref="CharacterDataParametersChunkRequestPayload"/>. Sends <see cref="CharacterDataParametersChunkResponsePayload"/> as a response.
	/// This basically streams the binary content for parameter data to the client based on chunk id.
	/// </summary>
	public sealed class CharacterDataParametersChunkRequestMessageHandler : GameRequestMessageHandler<CharacterDataParametersChunkRequestPayload, CharacterDataParametersChunkResponsePayload>
	{
		private IParameterContentLoadable ParameterLoader { get; }

		public CharacterDataParametersChunkRequestMessageHandler(IParameterContentLoadable parameterLoader) 
			: base(true)
		{
			ParameterLoader = parameterLoader ?? throw new ArgumentNullException(nameof(parameterLoader));
		}

		protected override async Task<CharacterDataParametersChunkResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterDataParametersChunkRequestPayload message, CancellationToken token = default)
		{
			//See reference: https://github.com/Sylverant/login_server/blob/12631a7035c68d6a8a99e3d0f524e7892554e6e7/src/bbcharacter.c#L943
			byte[] bytes = await ParameterLoader.GetParameterDataChunkAsync((int) message.ChunkNumber);

			return new CharacterDataParametersChunkResponsePayload(message.ChunkNumber, bytes);
		}
	}
}
