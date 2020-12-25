using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using FreecraftCore.Serializer;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Base module for message packet/header serialization services.
	/// Registers the following services:
	/// <para />
	/// <see cref="IMessageDeserializer{TMessageType}"/> for <typeparamref name="TMessageReadType"/>
	/// <para />
	/// <see cref="IMessageSerializer{TMessageType}"/> for <typeparamref name="TMessageWriteType"/>
	/// <para />
	/// <see cref="IMessageSerializer{TMessageType}"/> for <typeparamref name="TPacketHeaderSerializerType"/>
	/// <para />
	/// <see cref="IPacketHeaderFactory"/> for <typeparamref name="THeaderFactoryType"/>
	/// <para />
	/// <see cref="SerializerService"/> for FreecraftCore.
	/// <para />
	/// </summary>
	public abstract class ServerMessageSerializationServiceModule<TMessageReadType, TMessageWriteType, TMessageSerializerType, THeaderFactoryType, TPacketHeaderSerializerType> : Module
			where TMessageSerializerType : IMessageSerializer<TMessageWriteType>, IMessageDeserializer<TMessageReadType>
			where THeaderFactoryType : IPacketHeaderFactory
			where TPacketHeaderSerializerType : IMessageSerializer<PacketHeaderSerializationContext<TMessageWriteType>>
			where TMessageWriteType : class
			where TMessageReadType : class
	{
		/// <inheritdoc />
		protected sealed override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Assume all the serializer stuff is stateless and can be shared
			//so we're going to SingleInstance it.
			builder.RegisterType<TMessageSerializerType>()
				.AsSelf()
				.As<IMessageSerializer<TMessageWriteType>>()
				.As<IMessageDeserializer<TMessageReadType>>()
				.SingleInstance();

			builder.RegisterType<THeaderFactoryType>()
				.AsSelf()
				.As<IPacketHeaderFactory>()
				.SingleInstance();

			builder.RegisterType<TPacketHeaderSerializerType>()
				.AsSelf()
				.As<IMessageSerializer<PacketHeaderSerializationContext<TMessageWriteType>>>()
				.AsImplementedInterfaces()
				.SingleInstance();

			//We create the serializer but we must ALSO dispatch the activation event
			//and allow people to register their polymorphic serializers.
			builder.RegisterType<SerializerService>()
				.AsSelf()
				.As<ISerializerService>()
				.SingleInstance()
				.OnActivated(OnSerializerCreated);

			//This is just message serialization stuff. We don't need to make this instance per, it should be stateless.
			builder.RegisterType<SessionMessageBuildingServiceContext<TMessageReadType, TMessageWriteType>>()
				.AsSelf()
				.SingleInstance();
		}

		/// <summary>
		/// Event fired when the serializer is activated. Implementers can use this event to register or perform any action
		/// on the serializer they want at this point.
		/// </summary>
		/// <param name="serializerActivatedEventArgs">The activation event args.</param>
		protected abstract void OnSerializerCreated(IActivatedEventArgs<SerializerService> serializerActivatedEventArgs);
	}
}
