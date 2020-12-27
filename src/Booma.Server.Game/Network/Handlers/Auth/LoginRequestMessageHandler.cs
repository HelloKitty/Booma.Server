﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using Common.Logging;
using Glader.ASP.Authentication;
using Glader.Essentials;
using GladNet;
using Refit;

namespace Booma
{
	/// <summary>
	/// <see cref="GameRequestMessageHandler{TMessageRequestType,TMessageResponseType}"/> for the <see cref="SharedLoginRequest93Payload"/>.
	/// </summary>
	[AdditionalRegistrationAs(typeof(ILoginResponseSentEventSubscribable))]
	public sealed class LoginRequestMessageHandler : GameRequestMessageHandler<SharedLoginRequest93Payload, SharedLoginResponsePayload>, ILoginResponseSentEventSubscribable
	{
		/// <summary>
		/// Logging service.
		/// </summary>
		private ILog Logger { get; }

		/// <summary>
		/// The service resolver for an <see cref="IAuthenticationService"/>.
		/// </summary>
		private IServiceResolver<IAuthenticationService> AuthenticationServiceResolver { get; }

		/// <summary>
		/// Factory that can build successful <see cref="SharedLoginResponsePayload"/> messages.
		/// </summary>
		private ISuccessfulLogin93ResponseMessageFactory SuccessResponseFactory { get; }

		/// <inheritdoc />
		public event EventHandler<LoginResponseSentEventArgs> OnLoginResponseSent;

		public LoginRequestMessageHandler([NotNull] ILog logger,
			[NotNull] IServiceResolver<IAuthenticationService> authenticationServiceResolver,
			[NotNull] ISuccessfulLogin93ResponseMessageFactory successResponseFactory)
			: base(true)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			AuthenticationServiceResolver = authenticationServiceResolver ?? throw new ArgumentNullException(nameof(authenticationServiceResolver));
			SuccessResponseFactory = successResponseFactory ?? throw new ArgumentNullException(nameof(successResponseFactory));
		}

		/// <inheritdoc />
		protected override async Task<SharedLoginResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, 
			SharedLoginRequest93Payload message, CancellationToken token = default)
		{
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Attempting to Authenticate: {message.UserName}");

			try
			{
				return await TryAuthenticateUserAsync(message, token);
			}
			catch (Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Authentication exception. Reason: {e}");

				return new SharedLoginResponsePayload(AuthenticationResponseCode.LOGIN_93BB_UNKNOWN_ERROR);
			}
		}

		private async Task<SharedLoginResponsePayload> TryAuthenticateUserAsync([NotNull] SharedLoginRequest93Payload message, CancellationToken token)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			//Resolve the service, or try to.
			ServiceResolveResult<IAuthenticationService> authResolutionResult =
				await AuthenticationServiceResolver.Create(token);

			//If we have no auth service then we cannot auth the user
			//there isn't really a good response code for this so unknown error will have to do.
			if (!authResolutionResult.isAvailable)
				return new SharedLoginResponsePayload(AuthenticationResponseCode.LOGIN_93BB_UNKNOWN_ERROR);

			IAuthenticationService authService = authResolutionResult.Instance;

			try
			{
				JWTModel authResult = await authService.AuthenticateAsync(new AuthenticationRequest(message.UserName, message.Password));

				//TODO: Send more accurate error response.
				//If token is valid then auth was successful, otherwise something is wrong with credentials.
				if (authResult.isTokenValid)
					return SuccessResponseFactory.Create(new SuccessfulLoginResponseCreationContext(authResult, message));
				else
					return new SharedLoginResponsePayload(AuthenticationResponseCode.LOGIN_93BB_BAD_USER_PWD);
			}
			catch (ApiException e)
			{
				//This is an expected exception type due to the specification sending 400 BAD REQUEST on failure.
				if (e.StatusCode != HttpStatusCode.BadRequest)
					throw;

				JWTModel authResult = await e.GetContentAsAsync<JWTModel>();

				if (Logger.IsInfoEnabled)
					Logger.Info($"Account: {message.UserName} failed Auth. Reason: {authResult.Error} Message: {authResult.ErrorDescription}");

				//TODO: Handle error cases
				return new SharedLoginResponsePayload(AuthenticationResponseCode.LOGIN_93BB_BAD_USER_PWD);
			}
		}

		/// <inheritdoc />
		protected override Task OnResponseMessageSendAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, SharedLoginResponsePayload response, SendResult sendResult)
		{
			//broadcast the login response.
			OnLoginResponseSent?.Invoke(this, new LoginResponseSentEventArgs(response.ResponseCode, context));

			return base.OnResponseMessageSendAsync(context, response, sendResult);
		}
	}
}