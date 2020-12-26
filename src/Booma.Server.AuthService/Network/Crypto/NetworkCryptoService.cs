using System;
using System.Collections.Generic;
using System.Text;
using Booma.Proxy;

namespace Booma
{
	/// <summary>
	/// <see cref="INetworkCryptoService"/> and <see cref="INetworkCryptoInitializable"/> blowfish implementation.
	/// </summary>
	public sealed class BlowFishNetworkCryptoService : INetworkCryptoService, INetworkCryptoInitializable
	{
		/// <inheritdoc />
		public ICryptoServiceProvider EncryptionProvider { get; } = new BlowfishEncryptionService();

		/// <inheritdoc />
		public ICryptoServiceProvider DecryptionProvider { get; } = new BlowfishDecryptionService();

		//TODO: When I'm not feeling lazy add a backing field.
		/// <inheritdoc />
		public ICryptoKeyInitializable<byte[]> EncryptionInitializable => (ICryptoKeyInitializable<byte[]>) EncryptionProvider;

		/// <inheritdoc />
		public ICryptoKeyInitializable<byte[]> DecryptionInitializable => (ICryptoKeyInitializable<byte[]>) DecryptionProvider;
	}
}
