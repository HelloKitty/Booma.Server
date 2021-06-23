using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Glader.ASP.Authentication;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using OpenIddict.Abstractions;

namespace Booma
{
	[Obsolete("Remove eventually.")]
	enum StubEnum
	{

	}

	internal sealed class AuthorizedCharacterClaimAppender : IAuthorizedClaimsAppender
	{
		private IServiceResolver<ICharacterDataQueryService<StubEnum, StubEnum>> CharacterServiceResolver { get; }

		public AuthorizedCharacterClaimAppender(IServiceResolver<ICharacterDataQueryService<StubEnum, StubEnum>> characterServiceResolver)
		{
			CharacterServiceResolver = characterServiceResolver ?? throw new ArgumentNullException(nameof(characterServiceResolver));
		}

		public async Task AppendClaimsAsync(AuthorizationClaimsAppenderContext context, CancellationToken token = default)
		{
			//Do we have a subaccount in the request?
			SubAccountHeaderParser parser = new SubAccountHeaderParser();

			if(!parser.HasHeader(context.Request))
				return;

			int characterId = parser.Parse(context.Request);

			var result = await CharacterServiceResolver.Create(token);

			//TODO: How should we expose this failure? To client?
			if(!result.isAvailable)
				throw new InvalidOperationException($"Failed to retrieve required service for claims appending.");

			ResponseModel<RPGCharacterAccountData, CharacterDataQueryResponseCode> accountInfo = await result.Instance.RetrieveAccountAsync(characterId, token);

			if(!accountInfo.isSuccessful)
				throw new InvalidOperationException($"Failed to retrieve account info for Character: {characterId}");

			if(accountInfo.Result.AccountId.ToString() == context.Principal.GetClaim("sub"))
			{
				ClaimsIdentity identity = (ClaimsIdentity)context.Principal.Identity;
				Claim newClaim = new Claim(GladerEssentialsASPSecurityConstants.SUB_ACCOUNT_CLAIM, characterId.ToString(), ClaimValueTypes.Integer32);
				newClaim.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);
				identity.AddClaim(newClaim);
			}
		}
	}
}