﻿using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Booma;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Patch service message handler service module.
	/// Registers the services and handling for message handlers.
	/// </summary>
	public sealed class PatchMessageHandlerServiceModule 
		: ServerMessageHandlerServiceModule<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer, DefaultBoomaMessageHandler<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer, PatchNetworkOperationCode>>
	{

	}
}
