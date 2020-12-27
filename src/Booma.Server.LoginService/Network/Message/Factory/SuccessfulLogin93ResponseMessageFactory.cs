using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Booma.Proxy;
using Glader.ASP.Authentication;
using Glader.Essentials;

namespace Booma
{
	/// <summary>
	/// Creation context for a successful <see cref="SharedLoginResponsePayload"/>.
	/// </summary>
	public class SuccessfulLoginResponseCreationContext
	{
		/// <summary>
		/// The auth token generated.
		/// </summary>
		public JWTModel AuthenticationToken { get; }

		/// <summary>
		/// The matching request that the <see cref="AuthenticationToken"/> was generated from.
		/// </summary>
		public SharedLoginRequest93Payload Request { get; }

		public SuccessfulLoginResponseCreationContext([NotNull] JWTModel authenticationToken, [NotNull] SharedLoginRequest93Payload request)
		{
			AuthenticationToken = authenticationToken ?? throw new ArgumentNullException(nameof(authenticationToken));
			Request = request ?? throw new ArgumentNullException(nameof(request));
		}
	}

	/// <summary>
	/// Contract for types that can create a successful <see cref="SharedLoginResponsePayload"/>.
	/// </summary>
	public interface ISuccessfulLogin93ResponseMessageFactory : IFactoryCreatable<SharedLoginResponsePayload, SuccessfulLoginResponseCreationContext>
	{

	}

	public sealed class SuccessfulLogin93ResponseMessageFactory : ISuccessfulLogin93ResponseMessageFactory
	{
		public SharedLoginResponsePayload Create(SuccessfulLoginResponseCreationContext context)
		{
			if (!context.AuthenticationToken.isTokenValid)
				throw new ArgumentException($"Content must have a valid {nameof(JWTModel)} token.", nameof(context));

			//JWT Access Token should have a Subject field like:
			/*{
			"sub": "1",
			}*/
			//This subject id represents the account id and will be the guild card identifier
			//for the PSOBB server.
			JwtSecurityToken token = new JwtSecurityToken(context.AuthenticationToken.AccessToken);
			uint accountId = uint.Parse(token.Subject);

			//Team and security data nonsense is ignorable for Login.
			return new SharedLoginResponsePayload(accountId, 0, new byte[40]);
		}
	}
}
