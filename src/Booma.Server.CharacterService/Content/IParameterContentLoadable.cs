using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Booma;

namespace Booma
{
	/// <summary>
	/// Contract for a strategy for loading <see cref="DataParameterFileHeader"/>s and the respective binary content.
	/// </summary>
	public interface IParameterContentLoadable
	{
		/// <summary>
		/// Loads the <see cref="DataParameterFileHeader"/>s.
		/// </summary>
		/// <returns>The parameter heads.</returns>
		Task<DataParameterFileHeader[]> LoadHeadersAsync();

		/// <summary>
		/// Loads a data parameter buffer based on the provided <see cref="header"/>.
		/// </summary>
		/// <param name="header">The header for the parameter content/</param>
		/// <returns>The binary buffer representing the parameters.</returns>
		Task<byte[]> GetParameterDataAsync(DataParameterFileHeader header);

		/// <summary>
		/// Loads a binary buffer linked to the specified <see cref="chunkId"/>.
		/// </summary>
		/// <param name="chunkId">The chunk id.</param>
		/// <returns>The buffer linked to the id.</returns>
		Task<byte[]> GetParameterDataChunkAsync(int chunkId);
	}
}
