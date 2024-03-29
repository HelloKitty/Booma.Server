﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Booma;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="BoomaGameServerApplication"/> implementation for PSO Booma Channel/Block Service.
	/// </summary>
	public sealed class BoomaChannelServerApplication : BoomaGameServerApplication
	{
		/// <inheritdoc />
		public BoomaChannelServerApplication(NetworkAddressInfo serverAddress, ILog logger) 
			: base(serverAddress, logger)
		{

		}

		/// <inheritdoc />
		protected override ContainerBuilder RegisterServices(ContainerBuilder builder)
		{
			base.RegisterServices(builder);

			//These are the custom service specified modules
			//TODO: We FINALLY need a proper handler, this Channel/Block server DEMANDS IT!
			builder.RegisterModule<InPlaceMessageDispatchingServiceModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>>();

			//Login module that sends ship list on login/auth.
			builder.RegisterModule<LoginServiceModule<ChannelWelcomeEventListener>>();

			//Channel service handlers
			builder.RegisterModule(new GameAssemblyMessageHandlerServiceModule(GetType().Assembly));
			
			//Channel specific stuff
			builder.RegisterModule<MEAKKAServiceModule>();
			builder.RegisterModule<ActorStateServiceModule>();
			builder.RegisterModule<NameQueryServiceModule>();
			builder.RegisterModule<ItemServicesModule>();

			//Actors
			builder.RegisterModule<ChannelActorServiceModule>();
			builder.RegisterModule<LobbyActorServiceModule>();
			builder.RegisterModule<LobbyCharacterActorServiceModule>();
			builder.RegisterModule<InstanceActorServiceModule>();
			builder.RegisterModule<InstanceCharacterActorServiceModule>();

			builder.RegisterModule<CharacterDataServiceModule>();
			builder.RegisterModule<GameConfigServiceModule>();
			builder.RegisterModule<ExternalPlayerDataServiceModule>();

			//GGDBF Data
			builder.RegisterModule<GGDBFDataModule>();

			return builder;
		}

		/// <inheritdoc />
		public override async Task BeginListeningAsync(CancellationToken token = default)
		{
			await Container.Resolve<GGDBFInitializer>().InitializeAsync(token);
			await base.BeginListeningAsync(token);
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}
	}
}
