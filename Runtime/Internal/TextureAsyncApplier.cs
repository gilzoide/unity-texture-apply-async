using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gilzoide.TextureAsyncApply.Internal
{
    public static class TextureAsyncApplier
    {
        private static List<TextureAsyncApplyHandle> _textureApplyHandles = new List<TextureAsyncApplyHandle>();
        private static CommandBuffer _commandBuffer = new CommandBuffer
        {
            name = nameof(TextureAsyncApplier),
        };
        private static Camera _registeredCamera;
        private static int _lastProcessedFrame;

        public static void Register(TextureAsyncApplyHandle handle)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }
            if (!handle.IsValid || _textureApplyHandles.Contains(handle))
            {
                return;
            }

            _textureApplyHandles.Add(handle);
            RebuildCommandBuffer();
            if (_textureApplyHandles.Count == 1)
            {
                RegisterOnPreRender();
            }
        }

        public static void Unregister(TextureAsyncApplyHandle handle)
        {
            if (_textureApplyHandles.Remove(handle) && _textureApplyHandles.Count == 0)
            {
                UnregisterOnPreRender();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            Application.quitting += Dispose;
        }

        private static void Dispose()
        {
            _textureApplyHandles.Clear();

            UnregisterOnPreRender();

            _commandBuffer?.Dispose();
            _commandBuffer = null;
        }

        private static void RegisterOnPreRender()
        {
            Camera.onPreRender += OnPreRender;
        }

        private static void UnregisterOnPreRender()
        {
            if (_registeredCamera && _commandBuffer != null)
            {
                _registeredCamera.RemoveCommandBuffer(_registeredCamera.GetFirstCameraEvent(), _commandBuffer);
            }
            Camera.onPreRender -= OnPreRender;
        }

        private static void RebuildCommandBuffer()
        {
            _commandBuffer.Clear();
            foreach (TextureAsyncApplyHandle handle in _textureApplyHandles)
            {
                handle.FillCommandBuffer(_commandBuffer);
            }
        }

        private static void OnPreRender(Camera camera)
        {
            int currentFrame = Time.frameCount;
            if (currentFrame == _lastProcessedFrame)
            {
                return;
            }
            _lastProcessedFrame = currentFrame;

            if (camera == _registeredCamera)
            {
                return;
            }
            else if (_registeredCamera)
            {
                _registeredCamera.RemoveCommandBuffer(_registeredCamera.GetFirstCameraEvent(), _commandBuffer);
            }

            camera.AddCommandBuffer(camera.GetFirstCameraEvent(), _commandBuffer);
            _registeredCamera = camera;
        }

        private static CameraEvent GetFirstCameraEvent(this Camera camera)
        {
            switch (camera.actualRenderingPath)
            {
                case RenderingPath.DeferredShading:
                    return CameraEvent.BeforeGBuffer;

                default:
                    return CameraEvent.BeforeForwardOpaque;
            }
        }
    }
}
