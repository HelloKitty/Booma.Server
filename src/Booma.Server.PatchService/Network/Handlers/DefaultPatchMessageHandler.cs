using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// Default message handler for <see cref="PSOBBPatchPacketPayloadClient"/>
	/// </summary>
	public sealed class DefaultPatchMessageHandler : BaseDefaultMessageHandler<PSOBBPatchPacketPayloadClient, SessionMessageContext<PSOBBPatchPacketPayloadServer>>
	{
		/// <summary>
		/// Logging service.
		/// </summary>
		private ILog Logger { get; }

		public DefaultPatchMessageHandler([NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBPatchPacketPayloadServer> context, PSOBBPatchPacketPayloadClient message, CancellationToken token = new CancellationToken())
		{
			if (Logger.IsInfoEnabled)
				Logger.Info($"Unhandled: {message.OperationCode} Type: {message.GetType().Name}");
		}
	}
}
