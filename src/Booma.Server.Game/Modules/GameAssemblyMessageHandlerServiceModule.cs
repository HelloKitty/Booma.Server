using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Booma.Proxy;

namespace Booma
{
	/// <summary>
	/// Game message handler implementation of <see cref="AssemblyMessageHandlerServiceModule{TMessageReadType,TMessageWriteType}"/>
	/// </summary>
	public sealed class GameAssemblyMessageHandlerServiceModule : AssemblyMessageHandlerServiceModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>
	{
		public GameAssemblyMessageHandlerServiceModule([NotNull] Assembly targetAssembly) 
			: base(targetAssembly)
		{

		}
	}
}
