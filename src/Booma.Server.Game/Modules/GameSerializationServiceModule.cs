using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using Booma;
using FreecraftCore.Serializer;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// Game service packet/header serialization service module.
	/// </summary>
	public sealed class GameSerializationServiceModule 
		: ServerMessageSerializationServiceModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer, GamePacketHeaderFactory, GamePacketHeaderSerializer>
	{
		protected override void OnSerializerCreated(IActivatedEventArgs<SerializerService> serializerActivatedEventArgs)
		{
			if (serializerActivatedEventArgs == null) throw new ArgumentNullException(nameof(serializerActivatedEventArgs));

			//Only need to register the game serializers.
			serializerActivatedEventArgs.Instance.RegisterGamePacketSerializers();
		}
	}
}
