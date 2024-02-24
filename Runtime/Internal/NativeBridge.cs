using System;
using System.Runtime.InteropServices;

namespace Gilzoide.TextureAsyncApply.Internal
{
    public class NativeBridge
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        public const string LibraryName = "__Internal";
#else
        public const string LibraryName = "texture_async_apply";
#endif

        [DllImport(LibraryName, EntryPoint = "TextureAsyncApply_event_handler")]
        public static extern IntPtr GetEventHandler();
        public static readonly IntPtr EventHandler_ptr = GetEventHandler();

        [DllImport(LibraryName, EntryPoint = "TextureAsyncApply_new")]
        public static extern uint RegisterHandle(IntPtr buffer);

        [DllImport(LibraryName, EntryPoint = "TextureAsyncApply_dispose")]
        public static extern void UnregisterHandle(uint handle);
    }
}
