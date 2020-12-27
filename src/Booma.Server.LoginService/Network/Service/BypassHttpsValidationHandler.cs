using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Booma
{
	/// <summary>
	/// <see cref="HttpClientHandler"/> that will ignore certificate errors.
	/// </summary>
	public sealed class BypassHttpsValidationHandler : HttpClientHandler
	{
		public BypassHttpsValidationHandler()
		{
			this.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
		}
	}
}
