using System;
using Gilzoide.TextureApplyAsync.Internal;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gilzoide.TextureApplyAsync
{
    public class TextureApplyAsyncHandle : IDisposable
    {
        public uint Id { get; private set; }
        public Texture2D Texture { get; private set; }
        public NativeArray<byte> Buffer { get; private set; }

        public bool IsValid => Id != 0 && Texture;

        public TextureApplyAsyncHandle(Texture2D texture, Allocator allocator = Allocator.Persistent)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            Texture = texture;
            if (texture.isReadable)
            {
                Buffer = new NativeArray<byte>(texture.GetPixelData<byte>(0), allocator);
            }
            else
            {
                Buffer = new NativeArray<byte>(texture.GetSizeInBytes(), allocator);
            }
            unsafe
            {
                Id = NativeBridge.RegisterHandle((IntPtr) Buffer.GetUnsafePtr());
            }
        }

        ~TextureApplyAsyncHandle()
        {
            Dispose();
        }

        public NativeArray<TPixel> GetPixelData<TPixel>() where TPixel : struct
        {
            if (UnsafeUtility.SizeOf<TPixel>() != Buffer.Length / (Texture.width * Texture.height))
            {
                throw new ArgumentException("Pixel type does not match texture pixel size.", nameof(TPixel));
            }
            return Buffer.Reinterpret<TPixel>(sizeof(byte));
        }

        public void ScheduleUpdateEveryFrame()
        {
            TextureAsyncApplier.ScheduleUpdateEveryFrame(this);
        }

        public void ScheduleUpdateOnce()
        {
            TextureAsyncApplier.ScheduleUpdateOnce(this);
        }

        public void CancelUpdates()
        {
            TextureAsyncApplier.Unregister(this);
        }

        public void FillCommandBuffer(CommandBuffer commandBuffer)
        {
            if (IsValid)
            {
                commandBuffer.IssuePluginCustomTextureUpdateV2(NativeBridge.EventHandler_ptr, Texture, Id);
            }
        }

        public void Dispose()
        {
            CancelUpdates();

            NativeBridge.UnregisterHandle(Id);
            Id = 0;

            Texture = null;

            if (Buffer.IsCreated)
            {
                Buffer.Dispose();
            }
        }
    }
}
