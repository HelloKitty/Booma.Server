using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public enum RetrieveInstanceActorPathResponseCode
	{
		Success = GladerEssentialsModelConstants.RESPONSE_CODE_SUCCESS_VALUE,
		GeneralServerError = 2,
		UnknownInstance = 3
	}

	public sealed class RetrieveInstanceActorPathRequest : EntityActorMessage, IActorRequestMessage<ResponseModel<string, RetrieveInstanceActorPathResponseCode>>
	{
		public int InstanceId { get; init; }
	}
}
