using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	/// <summary>
	/// Contract for service that provides network (double-sided) encryption initializables.
	/// </summary>
	public interface INetworkCryptoInitializable
	{
		/// <summary>
		/// The encryption initializer for (out-going) crypto provider.
		/// </summary>
		ICryptoKeyInitializable<byte[]> EncryptionInitializable { get; }

		/// <summary>
		/// The decryption initializer for (in-coming) crypto provider.
		/// </summary>
		ICryptoKeyInitializable<byte[]> DecryptionInitializable { get; }
	}
}