using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Generic.Math;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Simple <see cref="BaseDefaultMessageHandler{TMessageType,TMessageContext}"/> implementation that just logs the message opcode and type.
	/// </summary>
	/// <typeparam name="TMessageReadType">Read type.</typeparam>
	/// <typeparam name="TMessageWriteType">Write type.</typeparam>
	public sealed class DefaultBoomaMessageHandler<TMessageReadType, TMessageWriteType, TOperationCodeType> : BaseDefaultMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>> 
		where TMessageReadType : class, IOperationCodeable<TOperationCodeType>
		where TMessageWriteType : class
		where TOperationCodeType : Enum
	{
		/// <summary>
		/// Logging service.
		/// </summary>
		private ILog Logger { get; }

		public DefaultBoomaMessageHandler(ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override Task HandleMessageAsync(SessionMessageContext<TMessageWriteType> context, TMessageReadType message, CancellationToken token = new CancellationToken())
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"Unhandled: {message.OperationCode}:{CalculateOpcodeValue(message):X4} Type: {message.GetType().Name}");

			return Task.CompletedTask;
		}

		private static short CalculateOpcodeValue(TMessageReadType message)
		{
			return GenericMath.Convert<TOperationCodeType, short>(message.OperationCode);
		}
	}
}
