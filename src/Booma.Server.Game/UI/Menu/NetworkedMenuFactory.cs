using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using Generic.Math;
using Glader.Essentials;

namespace Booma
{
	public abstract class NetworkedMenuFactory<TMenuCodeType, TMenuCreationType> 
		: IEnumerable<MenuListing>, IFactoryCreatable<MenuListing[], TMenuCreationType>
			where TMenuCodeType : Enum
	{
		/// <summary>
		/// Represents the id of the menu.
		/// </summary>
		public TMenuCodeType Id { get; }

		private NetworkedButtonMenuEntry[] InternalMenuEntries { get; set; } = Array.Empty<NetworkedButtonMenuEntry>();

		protected object SyncObj { get; } = new object();

		private int _menuIdentifierCount = -1; //so that the default value is 0, we init to -1.
		protected int MenuIdentifierCount => _menuIdentifierCount;

		protected NetworkedMenuFactory(TMenuCodeType id)
		{
			Id = id;
		}

		/// <inheritdoc />
		public IEnumerator<MenuListing> GetEnumerator()
		{
			foreach (var entry in InternalMenuEntries)
				yield return entry.Entry;
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Creates an array of menu listings that can be sent over a network.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public MenuListing[] Create(TMenuCreationType context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//TODO: This is kinda inefficient
			InternalMenuEntries = CreateMenu(context);
			return InternalMenuEntries.Select(m => m.Entry).ToArray();
		}

		/// <summary>
		/// Creates an array of menu listings that can be sent over a network.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected abstract NetworkedButtonMenuEntry[] CreateMenu(TMenuCreationType context);

		/// <summary>
		/// Implementers can call this method to create a new menu item.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected MenuListing CreateMenuItem(string name)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));

			int itemId = Interlocked.Increment(ref _menuIdentifierCount);
			int id = GenericMath.Convert<TMenuCodeType, int>(Id);

			return new MenuListing(new MenuItemIdentifier((uint) id, (uint) itemId), 0, name);
		}

		/// <summary>
		/// Client sends <see cref="MenuItemIdentifier"/> data in selection packets to interact with menus.
		/// Calling this will translate the data into a call to the actual backing-script.
		/// </summary>
		/// <param name="selectionData"></param>
		/// <param name="token">Cancel token.</param>
		public async Task NetworkSelectionAsync(MenuItemIdentifier selectionData, CancellationToken token = default)
		{
			if (selectionData == null) throw new ArgumentNullException(nameof(selectionData));

			//TODO: Verify menu
			await InternalMenuEntries[selectionData.MenuId]
				.Script
				.OnSelectionAsync(token);
		}
	}
}
