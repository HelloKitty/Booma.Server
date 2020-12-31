using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using Booma.Proxy;
using FreecraftCore.Serializer;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// Patch service packet/header serialization service module.
	/// </summary>
	public sealed class PatchSerializationServiceModule 
		: ServerMessageSerializationServiceModule<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer, PatchPacketHeaderFactory, PatchPacketHeaderSerializer>
	{
		protected override void OnSerializerCreated(IActivatedEventArgs<SerializerService> serializerActivatedEventArgs)
		{
			if (serializerActivatedEventArgs == null) throw new ArgumentNullException(nameof(serializerActivatedEventArgs));

			//Only need to register the patch serializers.
			serializerActivatedEventArgs.Instance.RegisterPatchPacketSerializers();
		}
	}
}
