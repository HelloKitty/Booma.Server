using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Common.Logging;
using Glader.Essentials;
using GladNet;
using JetBrains.Annotations;
using Module = Autofac.Module;

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
					
					//Now we resolve ALL bindable handlers
					//Any handler that is registered will now be bound to this handler service.
					foreach (var bindable in args.Context.Resolve<IEnumerable<ITypeBindable<IMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>>, TMessageReadType>>>())
					{
						bindable.BindTo(args.Instance);
					}
				})
				.SingleInstance();

			RegisterHandlers(builder);
		}

		/// <summary>
		/// Implementers can register additional handlers here.
		/// </summary>
		/// <param name="builder"></param>
		protected virtual void RegisterHandlers(ContainerBuilder builder)
		{

		}

		/// <summary>
		/// Registers a bindable <see cref="IMessageHandler{TMessageType,TMessageContext}"/> in the provider container.
		/// </summary>
		/// <typeparam name="THandlerType">The handler to register.</typeparam>
		/// <param name="builder"></param>
		/// <returns></returns>
		protected ContainerBuilder RegisterHandler<THandlerType>(ContainerBuilder builder)
			where THandlerType : IMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>>, 
			ITypeBindable<IMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>>, TMessageReadType>
		{
			RegisterHandler(builder, typeof(THandlerType));
			return builder;
		}

		/// <summary>
		/// Registers a bindable <see cref="IMessageHandler{TMessageType,TMessageContext}"/> in the provider container.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="handlerType">Type of handler.</param>
		/// <exception cref="ArgumentException">Throws if the handler type isn't a message handler.</exception>
		/// <returns></returns>
		protected static void RegisterHandler(ContainerBuilder builder, Type handlerType)
		{
			//TODO: Throw if invalid handler type.
			var registrationBuilder = builder.RegisterType(handlerType)
				.As<ITypeBindable<IMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>>, TMessageReadType>>()
				.SingleInstance();

			//TODO: Assert it is assignable to.
			foreach (var additional in handlerType.GetCustomAttributes<AdditionalRegistrationAsAttribute>())
				registrationBuilder = registrationBuilder
					.As(additional.ServiceType);
		}

		/// <summary>
		/// Parses the provided <see cref="Assembly"/> to locate all handler types.
		/// </summary>
		/// <param name="assembly">The assembly to parse.</param>
		/// <returns>Enumerable of all available message handler types.</returns>
		protected static IEnumerable<Type> GetHandlerTypes([NotNull] Assembly assembly)
		{
			if (assembly == null) throw new ArgumentNullException(nameof(assembly));

			return assembly.GetTypes()
				.Where(t => !t.IsAbstract)
				.Where(t => !t.IsAssignableTo<TDefaultHandlerType>()) //don't find the default!
				.Where(t => t.IsAssignableTo<ITypeBindable<IMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>>, TMessageReadType>>())
				.Where(t => t.IsAssignableTo<ITypeBindable<IMessageHandler<TMessageReadType, SessionMessageContext<TMessageWriteType>>, TMessageReadType>>())
				.ToArray();
		}
	}
}
