using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Patch service message handler service module.
	/// Registers the services and handling for message handlers.
	/// Registers the following services:
	/// <see cref="BaseDefaultMessageHandler{TMessageType,TMessageContext}"/> for <typeparamref name="TDefaultHandlerType"/>
	/// <para />
	/// <see cref="DefaultMessageHandlerService{TMessageType,TMessageContext}"/> for specified message types parameters.
	/// </summary>
	public abstract class ServerMessageHandlerServiceModule<TMessageReadType, TMessageWriteType, TDefaultHandlerType> : Module 
		where TMessageReadType : class 
		where TMessageWriteType : class
		where TDefaultHandlerType : BaseDefaultMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>>
	{
		/// <inheritdoc />
		protected sealed override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//We Register the default handler because we'll internally bind it to the handler service
			//we create. This simplifies handler discovery abit too.
			builder.RegisterType<TDefaultHandlerType>()
				.AsSelf()
				.As<BaseDefaultMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>>>()
				.SingleInstance();

			//Handlers AND handler service should be stateless so we only need single instance.
			builder
				.RegisterType<DefaultMessageHandlerService<TMessageReadType, SessionMessageContext<TMessageWriteType>>>()
				.As<IMessageHandlerService<TMessageReadType, SessionMessageContext<TMessageWriteType>>>()
				.OnActivated(args =>
				{
					//Bind one of the default handlers
					var handler = args.Context.Resolve<TDefaultHandlerType>();
					args.Instance.Bind<TMessageReadType>(handler);
				})
				.SingleInstance();
		}
	}
}
