using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using GladNet;
using JetBrains.Annotations;
using Pipelines.Sockets.Unofficial;

namespace Booma
{
	/// <summary>
	/// Cryptographic block cipher supported <see cref="INetworkMessageInterface{TPayloadReadType,TPayloadWriteType}"/> for
	/// connection messaging.
	/// </summary>
	/// <typeparam name="TMessageReadType"></typeparam>
	/// <typeparam name="TMessageWriteType"></typeparam>
	public sealed class EncryptedNetworkMessageInterface<TMessageReadType, TMessageWriteType> 
		: SocketConnectionNetworkMessageInterface<TMessageReadType, TMessageWriteType>, ISingleDisposable
		where TMessageReadType : class 
		where TMessageWriteType : class
	{
		/// <summary>
		/// The crypto-block size.
		/// </summary>
		private int BlockSize { get; } = 8;

		/// <summary>
		/// The crypto service.
		/// </summary>
		private INetworkCryptoService CryptoService { get; }

		/// <summary>
		/// Indicates if we've been disposed.
		/// </summary>
		public bool isDisposed { get; private set; } = false;

		/// <summary>
		/// Buffer used for decrypted/read packets.
		/// </summary>
		private byte[] IncomingDecryptedPacketBuffer { get; }

		/// <summary>
		/// Read Syncronization object.
		/// Exists to prevent disposing in the middle of an operation.
		/// </summary>
		private readonly object ReadSyncObj = new object();

		//A total hack, but easy to do.
		/// <summary>
		/// Indicates if we should encrypt outgoing messages.
		/// By default we shouldn't.
		/// </summary>
		private bool ShouldEncryptOutgoing { get; set; } = false;
			
		public EncryptedNetworkMessageInterface(NetworkConnectionOptions networkOptions, 
			SocketConnection connection, 
			SessionMessageBuildingServiceContext<TMessageReadType, TMessageWriteType> messageServices,
			INetworkCryptoService cryptoService) 
			: base(networkOptions, connection, messageServices)
		{
			CryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));

			IncomingDecryptedPacketBuffer = ArrayPool<byte>.Shared.Rent(networkOptions.MaximumPacketSize);
		}

		/// <inheritdoc />
		protected override IPacketHeader DeserializePacketHeader(ReadOnlySequence<byte> buffer, int exactHeaderByteCount)
		{
			//TODO: This exactHeaderByteCount is wrong and won't work. WE NEED TO FIX RIGHT AWAY!!
			//This is just like the base implementation except we decrypt the 8 bytes sometimes and store the left over bytes
			//from the buffer.
			IPacketHeader header;
			using (var context = new PacketHeaderCreationContext(buffer, BlockSize))
			{
				//Decrypt if we need to.
				Span<byte> headerBuffer = context.GetSpan();
				CryptoService.DecryptionProvider.Crypt(headerBuffer, 0, BlockSize);
				header = MessageServices.PacketHeaderFactory.Create(context);

				//Once we've made the header let's copy the actual payload bytes into a temp buffer.
				lock(ReadSyncObj)
				{
					if (isDisposed)
						throw new ObjectDisposedException(nameof(IncomingDecryptedPacketBuffer));

					//Copy the remaining 6 bytes of the header buffer to the decrypted packet buffer.
					//these bytes will be needed for for the payload.
					headerBuffer
						.Slice(2, BlockSize - 2)
						.CopyTo(IncomingDecryptedPacketBuffer);
				}
			}

			return header;
		}

		/// <inheritdoc />
		protected override bool IsPayloadReadable(ReadResult result, IPacketHeader header)
		{
			//We already read a BlockSize. Therefore the whole packet read would be:
			// Header (Block Size) + Buffer >= PacketSize with block adjustment
			return result.Buffer.Length + BlockSize >= ConvertToBlocksizeCount(header.PacketSize);
		}

		/// <inheritdoc />
		protected override TMessageReadType ReadIncomingPacketPayload(in ReadOnlySequence<byte> result, IPacketHeader header)
		{
			lock (ReadSyncObj)
			{
				if (isDisposed)
					throw new ObjectDisposedException(nameof(IncomingDecryptedPacketBuffer));

				//We have a special case where the payload is empty and so no more bytes will exist.
				//So only the current buffer is required and it's already decrypted and everything
				if (result.IsEmpty)
				{
					int emptyOffset = 0;
					return MessageServices.MessageDeserializer.Deserialize(new Span<byte>(IncomingDecryptedPacketBuffer, 0, BlockSize - 2), ref emptyOffset);
				}

				Span<byte> buffer = new Span<byte>(IncomingDecryptedPacketBuffer);
				int remainingChunkSize = ConvertToBlocksizeCount(header.PacketSize) - BlockSize;

				//This copy is BAD but it really avoids a lot of API headaches
				//PacketSize + padding - BlockSize is the remaining buffer
				//We skip 6 bytes since blocksize - lengthsize is 6. 6 bytes left over from header read operation.
				result.Slice(0, remainingChunkSize).CopyTo(buffer.Slice(6));

				//WARNING: DON'T SKIP BY BLOCKSIZE! ONLY 6 BYTES, SINCE 2 BYTES WERE DROPPED OFF.
				//Decrypt ONLY the packet AFTER the header block (8 bytes blocksize - 2) skipped since header is decrypted).
				CryptoService.DecryptionProvider.Crypt(buffer.Slice(6, remainingChunkSize), 0, remainingChunkSize);

				//It's important we only allow the serializer to see the REAL payload size.
				//Don't show it the garbage bytes, so we slice at PayloadSize and not blockAdjustedSize.
				int offset = 0;
				return MessageServices.MessageDeserializer.Deserialize(buffer.Slice(0, header.PayloadSize), ref offset);
			}
		}

		/// <inheritdoc />
		protected override int ComputeIncomingPayloadBytesRead(IPacketHeader header)
		{
			if (header == null) throw new ArgumentNullException(nameof(header));

			//Idea here is we've read the entire packet when reading this payload EXCEPT for the first 8 byte
			//block size since that was read as the header and stored away.
			return ConvertToBlocksizeCount(header.PacketSize) - BlockSize;
		}

		protected override int OnBeforePacketBufferSend(in Span<byte> buffer, int length)
		{
			//Always send the correct size in blocks.
			length = ConvertToBlocksizeCount(length);

			if (buffer.Length < length)
				throw new InvalidOperationException($"Outgoing packet buffer was too small to support block size correction. Length: {buffer.Length} Requested: {length}");
			
			//It's not actually threadsafe to encrypt since many threads are allows to send a packet at one time technically
			//But we should assume this is never called on multiple threads, since caller should worry about Pipelines threadsafety
			//Now we must encrypt entire payload, including the padding of the block.
			if (ShouldEncryptOutgoing)
				CryptoService.EncryptionProvider.Crypt(buffer, 0, length);
			else
			{
				//After the first message is sent we should enable outgoing crypto.
				//It's a total hack to do it this way but it makes it so initialization
				//doesn't depend on order.
				ShouldEncryptOutgoing = true;
			}

			return length;
		}

		//From old GladNet: https://github.com/HelloKitty/GladNet3/blob/0e6d5b24dbb5af5bab0768c054d8f5f2fc456604/src/GladNet3.Client.API/Decorators/NetworkClientFixedBlockSizeCryptoDecorator.cs#L303
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int ConvertToBlocksizeCount(int count)
		{
			int remainder = count % BlockSize;

			//Important to check if it's already perfectly size
			//otherwise below code will return count + blocksize
			if(remainder == 0)
				return count;

			return count + (BlockSize - (count % BlockSize));
		}

		public void Dispose()
		{
			lock (ReadSyncObj)
			{
				if(isDisposed)
					return;

				try
				{
					ArrayPool<byte>.Shared.Return(IncomingDecryptedPacketBuffer);
				}
				finally
				{
					isDisposed = true;
				}
			}
		}
	}
}
