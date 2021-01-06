using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Util;
using Autofac.Features.AttributeFilters;
using GladNet;
using MEAKKA;

namespace Booma
{
	public sealed class ActorMessageHandlerService<TActorType> : 
		IMessageHandlerService<EntityActorMessage, EntityActorMessageContext>, 
		ITypeBinder<IMessageHandler<EntityActorMessage, EntityActorMessageContext>, EntityActorMessage>
	{
		private Dictionary<Type, List<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>> MessageHandlerMap { get; }

		public ActorMessageHandlerService(IEnumerable<IMessageHandler<EntityActorMessage, EntityActorMessageContext>> handlers)
		{
			if (handlers == null) throw new ArgumentNullException(nameof(handlers));

			//TODO: better default size.
			MessageHandlerMap = new Dictionary<Type, List<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>>(30);

			foreach(var handler in handlers)
				handler.BindTo(this);
		}

		public async Task<bool> HandleMessageAsync(EntityActorMessageContext context, EntityActorMessage message, CancellationToken token = default)
		{
			if (!MessageHandlerMap.ContainsKey(message.GetType()))
				return false;

			foreach (var handler in MessageHandlerMap[message.GetType()])
				await handler.HandleMessageAsync(context, message, token);

			return true;
		}

		/// <inheritdoc />
		public bool Bind<TBindType>(IMessageHandler<EntityActorMessage, EntityActorMessageContext> bindable) 
			where TBindType : EntityActorMessage
		{
			if (bindable == null) throw new ArgumentNullException(nameof(bindable));

			if (MessageHandlerMap.ContainsKey(typeof(TBindType)))
				MessageHandlerMap[typeof(TBindType)].Add(bindable);
			else
			{
				MessageHandlerMap[typeof(TBindType)] = new List<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>(4);
				return Bind<TBindType>(bindable);
			}

			return true;
		}
	}
}
