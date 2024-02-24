using System;
using Gilzoide.TextureAsyncApply.Internal;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gilzoide.TextureAsyncApply
{
    public class TextureAsyncApplyHandle : IDisposable
    {
        public uint Id { get; private set; }
        public Texture2D Texture { get; private set; }
        public NativeArray<byte> Buffer { get; private set; }

        public bool IsValid => Id != 0 && Texture;

        public TextureAsyncApplyHandle(Texture2D texture, Allocator allocator = Allocator.Persistent)
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
                Id = NativeBridge.CreateHandle((IntPtr) Buffer.GetUnsafePtr());
            }
        }

        public void RegisterInRenderLoop()
        {
            TextureAsyncApplier.Register(this);
        }

        public void UnregisterFromRenderLoop()
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
            UnregisterFromRenderLoop();

            NativeBridge.DisposeHandle(Id);
            Id = 0;

            Texture = null;

            if (Buffer.IsCreated)
            {
                Buffer.Dispose();
            }
        }
    }
}
