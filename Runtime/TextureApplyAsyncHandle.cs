using System;
using Gilzoide.TextureApplyAsync.Internal;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gilzoide.TextureApplyAsync
{
    public class TextureApplyAsyncHandle : IDisposable
    {
        public uint Id { get; private set; }
        public Texture2D Texture { get; private set; }

        public bool IsValid => Id != 0 && Texture;

        public TextureApplyAsyncHandle(Texture2D texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            Texture = texture;
            unsafe
            {
                Id = NativeBridge.RegisterHandle((IntPtr) Texture.GetRawTextureData<byte>().GetUnsafeReadOnlyPtr());
            }
        }

        ~TextureApplyAsyncHandle()
        {
            Dispose();
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
        }
    }
}
