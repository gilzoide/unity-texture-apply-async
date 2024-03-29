using System;
using System.Runtime.InteropServices;

namespace Gilzoide.TextureApplyAsync.Internal
{
    internal class NativeBridge
    {
#if !UNITY_EDITOR && (UNITY_WEBGL || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS)
        internal const string LibraryName = "__Internal";
#else
        internal const string LibraryName = "texture_async_apply";
#endif

        [DllImport(LibraryName, EntryPoint = "TextureAsyncApply_event_handler")]
        internal static extern IntPtr GetEventHandler();
        internal static readonly IntPtr EventHandler_ptr = GetEventHandler();

        [DllImport(LibraryName, EntryPoint = "TextureAsyncApply_new")]
        internal static extern uint RegisterHandle(IntPtr buffer);

        [DllImport(LibraryName, EntryPoint = "TextureAsyncApply_dispose")]
        internal static extern void UnregisterHandle(uint handle);
    }
}
