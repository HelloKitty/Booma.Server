using System;
using System.Collections.Generic;
using System.Text;
using Booma;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Base <see cref="BaseSpecificMessageHandler{TMessageType,TBaseMessageType,TMessageContext}"/> for <see cref="PSOBBGamePacketPayloadClient"/>s.
	/// </summary>
	/// <typeparam name="TMessageType">The specific client message.</typeparam>
	public abstract class GameMessageHandler<TMessageType> : BaseSpecificMessageHandler<TMessageType, PSOBBGamePacketPayloadClient, SessionMessageContext<PSOBBGamePacketPayloadServer>> 
		where TMessageType : PSOBBGamePacketPayloadClient
	{

	}
}
