using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Booma.Proxy;
using Glader.Essentials;

namespace Booma
{
	/// <summary>
	/// Contract for types that are <see cref="IFactoryCreatable{TCreateType,TContextType}"/>s
	/// for <see cref="SharedWelcomePayload"/> messages.
	/// </summary>
	public interface IWelcomeMessageFactory : IFactoryCreatable<SharedWelcomePayload, EmptyFactoryContext>
	{

	}

	/// <summary>
	/// <see cref="IWelcomeMessageFactory"/>
	/// </summary>
	public sealed class DefaultWelcomeMessageFactory : IWelcomeMessageFactory, IDisposable
	{
		private INetworkCryptoInitializable CryptoInitializable { get; }

		/// <summary>
		/// Secure random generator for key generation.
		/// </summary>
		private RNGCryptoServiceProvider RandomGenerator { get; } = new RNGCryptoServiceProvider();

		/// <summary>
		/// The string message to send with the welcome message.
		/// </summary>
		public const string WelcomeMessage = "Phantasy Star Online Blue Burst Game Server. Copyright 1999-2004 SONICTEAM.";

		public DefaultWelcomeMessageFactory(INetworkCryptoInitializable cryptoInitializable)
		{
			CryptoInitializable = cryptoInitializable ?? throw new ArgumentNullException(nameof(cryptoInitializable));
		}

		public SharedWelcomePayload Create(EmptyFactoryContext context)
		{
			byte[] clientIV = new byte[SharedWelcomePayload.ENCRYPTION_VECTOR_SIZE];
			RandomGenerator.GetNonZeroBytes(clientIV);

			byte[] serverIV = new byte[SharedWelcomePayload.ENCRYPTION_VECTOR_SIZE];
			RandomGenerator.GetNonZeroBytes(serverIV);

			//ALWAYS COPY THE IVs!! Crypto may modify them, (PSOBB blowfish will!)
			CryptoInitializable.EncryptionInitializable.Initialize(serverIV.ToArray());
			CryptoInitializable.DecryptionInitializable.Initialize(clientIV.ToArray());

			return new SharedWelcomePayload(WelcomeMessage, serverIV.ToArray(), clientIV.ToArray());
		}

		/// <inheritdoc />
		public void Dispose()
		{
			RandomGenerator.Dispose();
		}
	}
}
