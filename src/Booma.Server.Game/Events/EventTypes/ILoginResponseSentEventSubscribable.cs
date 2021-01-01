using System;
using FreecraftCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Booma;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Contract for a type that implements a subscription service for events that publish <see cref="LoginResponseSentEventArgs"/>
	/// through the <see cref="EventHandler{TEventArgs}"/> <see cref="OnLoginResponseSent"/>
	/// </summary>
	public interface ILoginResponseSentEventSubscribable
	{
		event EventHandler<LoginResponseSentEventArgs> OnLoginResponseSent;
	}

	/// <summary>
	/// Event arguments for the <see cref="ILoginResponseSentEventSubscribable"/> interface.
	/// </summary>
	public sealed class LoginResponseSentEventArgs : EventArgs, IResponseCodePayload<AuthenticationResponseCode>
	{
		/// <inheritdoc />
		public AuthenticationResponseCode ResponseCode { get; }

		//This is dumb, we should flow a connection identifier or something. Not a freaking message context.
		/// <summary>
		/// The context of the login message.
		/// </summary>
		public SessionMessageContext<PSOBBGamePacketPayloadServer> MessageContext { get; }

		public SharedLoginRequest93Payload.SessionStage Stage { get; }

		public LoginResponseSentEventArgs(AuthenticationResponseCode responseCode, SessionMessageContext<PSOBBGamePacketPayloadServer> messageContext, SharedLoginRequest93Payload.SessionStage stage)
		{
			if (!Enum.IsDefined(typeof(AuthenticationResponseCode), responseCode)) throw new InvalidEnumArgumentException(nameof(responseCode), (int) responseCode, typeof(AuthenticationResponseCode));
			ResponseCode = responseCode;
			MessageContext = messageContext ?? throw new ArgumentNullException(nameof(messageContext));
			Stage = stage;
		}
	}
}