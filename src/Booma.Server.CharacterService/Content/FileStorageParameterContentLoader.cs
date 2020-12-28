using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booma.Proxy;
using Force.Crc32;
using Nito.AsyncEx;

namespace Booma.Content
{
	/* The list of parameter files. These should be in blueburst/param
	const static char* param_files[NUM_PARAM_FILES] = {
		"ItemMagEdit.prs",
		"ItemPMT.prs",
		"BattleParamEntry.dat",
		"BattleParamEntry_on.dat",
		"BattleParamEntry_lab.dat",
		"BattleParamEntry_lab_on.dat",
		"BattleParamEntry_ep4.dat",
		"BattleParamEntry_ep4_on.dat",
		"PlyLevelTbl.prs"
	};*/

	/// <summary>
	/// File-storage based implementation of <see cref="IParameterContentLoadable"/>.
	/// </summary>
	public sealed class FileStorageParameterContentLoader : IParameterContentLoadable
	{
		/// <summary>
		/// Async syncronization object.
		/// </summary>
		private AsyncReaderWriterLock SyncObj { get; } = new AsyncReaderWriterLock();

		/// <summary>
		/// The cached parameter file headers.
		/// </summary>
		private DataParameterFileHeader[] CachedHeaders { get; set; }

		/// <summary>
		/// Buffer storage map.
		/// </summary>
		private Dictionary<string, byte[]> FileBufferMap { get; } = new Dictionary<string, byte[]>(9);

		/// <summary>
		/// Represents a list of chunk buffers. Pre-calculated parameter files as chunks.
		/// </summary>
		private List<byte[]> ChunkList { get; } = new List<byte[]>(500);

		/// <inheritdoc />
		public async Task<DataParameterFileHeader[]> LoadHeadersAsync()
		{
			if (CachedHeaders != null)
				return CachedHeaders;

			using (await SyncObj.WriterLockAsync())
			{
				//Double check locking.
				if(CachedHeaders != null)
					return CachedHeaders;

				//NEVER EVER USE THE CANCEL TOKEN FROM A SESSION HERE EVER!!
				//WE MUST FINISH LOADING NO MATTER WHAT
				CachedHeaders = await LoadParameterDataAsync();
				return CachedHeaders;
			}
		}

		/// <summary>
		/// Builds the headers and fills the <see cref="FileBufferMap"/>.
		/// </summary>
		/// <returns></returns>
		private async Task<DataParameterFileHeader[]> LoadParameterDataAsync()
		{
			List<DataParameterFileHeader> parameterFiles = new List<DataParameterFileHeader>(9);
			List<byte[]> buffers = new List<byte[]>(9);
			FileBufferMap.Clear();

			foreach (string fileName in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Data")))
			{
				byte[] bytes = await File.ReadAllBytesAsync(fileName);
				uint checksum = Crc32Algorithm.Compute(bytes);

				//Build the parameter header and store the bytes for streaming.
				//The offset is basically, how deep into the binary blob this would be.
				uint offset = (uint) FileBufferMap.Values.Aggregate(0, (i, storedBuffer) => i + storedBuffer.Length);

				string simpleName = Path.GetFileName(fileName);
				parameterFiles.Add(new DataParameterFileHeader((uint) bytes.Length, checksum, offset, simpleName));
				FileBufferMap[simpleName] = bytes;
				buffers.Add(bytes);
			}

			//TODO: Make 0x6800 a constant define somewhere.
			//Break all the buffers into chunks of 0x6800 max.
			await using (MemoryStream stream = new MemoryStream())
			{
				//Writes all the buffers into the memory stream
				foreach (var buffer in buffers)
					await stream.WriteAsync(buffer);

				stream.Position = 0;

				while (stream.Position < stream.Length)
				{
					//TODO: Make this 0x6800 chunk size a constant.
					Memory<byte> chunkBuffer = new Memory<byte>(new byte[0x6800]);

					//This allocations more than it needs to but it's fine I guess.
					int count = await stream.ReadAsync(chunkBuffer);
					ChunkList.Add(chunkBuffer.Slice(0, count).ToArray());
				}
			}

			return parameterFiles.ToArray();
		}

		/// <inheritdoc />
		public async Task<byte[]> GetParameterDataAsync([NotNull] DataParameterFileHeader header)
		{
			if (header == null) throw new ArgumentNullException(nameof(header));

			using (await SyncObj.ReaderLockAsync())
			{
				if (FileBufferMap.ContainsKey(header.FileName))
					return FileBufferMap[header.FileName];
				else
					return Array.Empty<byte>();
			}
		}

		/// <inheritdoc />
		public async Task<byte[]> GetParameterDataChunkAsync(int chunkId)
		{
			if (chunkId < 0) throw new ArgumentOutOfRangeException(nameof(chunkId));

			using (await SyncObj.ReaderLockAsync())
			{
				return ChunkList[chunkId];
			}
		}
	}
}
