using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Booma
{
	/// <summary>
	/// Constants class that contains constant values related to
	/// endpoints that should be unchanging.
	/// </summary>
	public static class BoomaEndpointConstants
	{
		/// <summary>
		/// Represents the default and constant Booma service discovery endpoint.
		/// </summary>
		public const string BOOMA_SERVICE_DISCOVERY_ENDPOINT = @"https://localhost:5001/";

		/// <summary>
		/// Represents the constant string identifier for a Booma service name.
		/// </summary>
		public const string BOOMA_AUTHENTICATION_SERVICE_NAME = "AUTHENTICATION";

		/// <summary>
		/// Gets a constant service name based on the service type.
		/// </summary>
		/// <param name="type">The service name.</param>
		/// <returns></returns>
		public static string GetServiceIdentifier(BoomaServiceType type)
		{
			if (!Enum.IsDefined(typeof(BoomaServiceType), type)) throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(BoomaServiceType));

			switch (type)
			{
				case BoomaServiceType.AuthService:
					return BOOMA_AUTHENTICATION_SERVICE_NAME;
				case BoomaServiceType.PatchService:
				case BoomaServiceType.LoginService:
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}
