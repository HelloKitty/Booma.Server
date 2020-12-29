using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;

namespace Booma
{
	public sealed class MutableSessionTokenStorageRepository : IReadonlyAuthTokenRepository, IAuthTokenRepository
	{
		/// <summary>
		/// The authentication token.
		/// </summary>
		private string AuthToken { get; set; } = null;

		/// <inheritdoc />
		public string Retrieve()
		{
			if(AuthToken == null)
				throw new InvalidOperationException("Auth token it not initialized.");

			return AuthToken;
		}

		/// <summary>
		/// Default ctor, does not initialize the token.
		/// </summary>
		public MutableSessionTokenStorageRepository()
		{
			//No default value
		}

		/// <summary>
		/// Initializes the <see cref="AuthToken"/> with the provided <see cref="AuthToken"/>
		/// </summary>
		/// <param name="authToken"></param>
		public MutableSessionTokenStorageRepository(string authToken)
		{
			AuthToken = authToken ?? throw new ArgumentNullException(nameof(authToken));
		}

		/// <inheritdoc />
		public string RetrieveWithType()
		{
			//TODO: Make this more efficient
			return $"{RetrieveType()} {AuthToken}";
		}

		/// <inheritdoc />
		public string RetrieveType()
		{
			return "Bearer";
		}

		/// <inheritdoc />
		public void Update(string authToken)
		{
			if(string.IsNullOrEmpty(authToken)) throw new ArgumentException("Value cannot be null or empty.", nameof(authToken));

			AuthToken = authToken;
		}
	}
}
