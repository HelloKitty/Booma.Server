using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Booma;

namespace Booma
{
	/// <summary>
	/// Game message handler implementation of <see cref="AssemblyMessageHandlerServiceModule{TMessageReadType,TMessageWriteType}"/>
	/// </summary>
	public sealed class GameAssemblyMessageHandlerServiceModule : AssemblyMessageHandlerServiceModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>
	{
		public GameAssemblyMessageHandlerServiceModule(Assembly targetAssembly) 
			: base(targetAssembly)
		{

		}
	}
}
