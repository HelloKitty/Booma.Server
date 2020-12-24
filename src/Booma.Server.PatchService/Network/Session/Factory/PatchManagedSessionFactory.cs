using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Glader.Essentials;
using GladNet;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// Simplified interface for <see cref="IFactoryCreatable{TCreateType,TContextType}"/>.
	/// </summary>
	public interface IPatchManagedSessionFactory : IFactoryCreatable<BoomaPatchManagedSession, SessionCreationContext>
	{

	}

	/// <summary>
	/// <see cref="IFactoryCreatable{TCreateType,TContextType}"/> implementation that can create <see cref="BoomaPatchManagedSession"/>s
	/// from <see cref="SessionCreationContext"/> using an <see cref="IContainer"/> to resolve dependencies.
	/// </summary>
	public sealed class PatchManagedSessionFactory : IPatchManagedSessionFactory
	{
		/// <summary>
		/// Service container.
		/// </summary>
		private IContainer Container { get; }

		public PatchManagedSessionFactory([NotNull] IContainer container)
		{
			Container = container ?? throw new ArgumentNullException(nameof(container));
		}

		/// <inheritdoc />
		public BoomaPatchManagedSession Create(SessionCreationContext context)
		{
			//We don't actually dispose the lifetime scope right away, we attach it to the session.
			ILifetimeScope scope = Container.BeginLifetimeScope(c =>
			{
				c.RegisterInstance(context.Connection)
					.AsSelf()
					.ExternallyOwned();

				c.RegisterInstance(context.Details)
					.AsSelf()
					.ExternallyOwned();
			});

			try
			{
				//We build the session then attach the container's lifetime scope it was built from to the session right away
				//so it's disposed when the session is disposed.
				BoomaPatchManagedSession session = scope.Resolve<BoomaPatchManagedSession>();
				session.AttachDisposableResource(scope);
				return session;
			}
			catch(Exception)
			{
				//Important to dispose of container scope if we fail to build.
				scope.Dispose();
				throw;
			}
		}
	}
}
