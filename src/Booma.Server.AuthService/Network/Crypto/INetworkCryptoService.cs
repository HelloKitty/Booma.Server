using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	/// <summary>
	/// Contract for service that provides network (double-sided) encryption.
	/// </summary>
	public interface INetworkCryptoService
	{
		/// <summary>
		/// The encryption (out-going) crypto provider.
		/// </summary>
		ICryptoServiceProvider EncryptionProvider { get; }

		/// <summary>
		/// The decryption (in-coming) crypto provider.
		/// </summary>
		ICryptoServiceProvider DecryptionProvider { get; }
	}
}
