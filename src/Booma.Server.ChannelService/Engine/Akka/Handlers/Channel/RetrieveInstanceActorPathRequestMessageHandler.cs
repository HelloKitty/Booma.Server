using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Handler for mostly <see cref="RootChannelActor"/> which handles instance path GET requests.
	/// </summary>
	[ActorMessageHandler(typeof(RootChannelActor))]
	public sealed class RetrieveInstanceActorPathRequestMessageHandler : ActorRequestMessageHandler<RetrieveInstanceActorPathRequest, ResponseModel<string, RetrieveInstanceActorPathResponseCode>>
	{
		/// <summary>
		/// Repository for instances.
		/// </summary>
		private IInstanceEntryRepository InstanceRepository { get; }

		public RetrieveInstanceActorPathRequestMessageHandler(IInstanceEntryRepository instanceRepository)
		{
			InstanceRepository = instanceRepository ?? throw new ArgumentNullException(nameof(instanceRepository));
		}

		protected override async Task<ResponseModel<string, RetrieveInstanceActorPathResponseCode>> HandleRequestAsync(EntityActorMessageContext context, RetrieveInstanceActorPathRequest message, CancellationToken token = default)
		{
			if (!await InstanceRepository.ContainsAsync(message.InstanceId, token))
				return new ResponseModel<string, RetrieveInstanceActorPathResponseCode>(RetrieveInstanceActorPathResponseCode.UnknownInstance);

			var instance = await InstanceRepository.RetrieveAsync(message.InstanceId, token);

			return new ResponseModel<string, RetrieveInstanceActorPathResponseCode>(instance.InstanceActorPath);
		}
	}
}
