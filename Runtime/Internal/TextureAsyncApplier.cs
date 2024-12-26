using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gilzoide.TextureApplyAsync.Internal
{
    public static class TextureAsyncApplier
    {
        private static readonly List<TextureApplyAsyncHandle> _applyHandlesEveryFrame = new();
        private static readonly List<TextureApplyAsyncHandle> _applyHandlesThisFrame = new();
        private static CommandBuffer _commandBuffer;
        private static Camera _registeredCamera;
        private static int _lastProcessedFrame;
        private static bool _isCommandBufferDirty;
        private static bool _isOnPreRenderRegistered;

        private static int HandlesCount => _applyHandlesEveryFrame.Count + _applyHandlesThisFrame.Count;
        private static bool IsUsingScriptableRenderPipeline => GraphicsSettings.currentRenderPipeline != null;

        public static void ScheduleUpdateEveryFrame(TextureApplyAsyncHandle handle)
        {
            Register(handle, true);
        }

        public static void ScheduleUpdateOnce(TextureApplyAsyncHandle handle)
        {
            Register(handle, false);
        }

        private static void Register(TextureApplyAsyncHandle handle, bool updateEveryFrame)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }
            if (!handle.IsValid
                || _applyHandlesEveryFrame.Contains(handle)
                || (!updateEveryFrame && _applyHandlesThisFrame.Contains(handle)))
            {
                return;
            }

            if (updateEveryFrame)
            {
                _applyHandlesThisFrame.Remove(handle);
                _applyHandlesEveryFrame.Add(handle);
            }
            else
            {
                _applyHandlesThisFrame.Add(handle);
            }
            _isCommandBufferDirty = true;
            if (!_isOnPreRenderRegistered)
            {
                RegisterOnPreRender();
            }
        }

        public static void Unregister(TextureApplyAsyncHandle handle)
        {
            bool removedAnyHandles = _applyHandlesEveryFrame.Remove(handle) || _applyHandlesThisFrame.Remove(handle);
            if (removedAnyHandles)
            {
                _isCommandBufferDirty = true;
                if (_isOnPreRenderRegistered && HandlesCount == 0)
                {
                    UnregisterOnPreRender();
                }
            }
        }

        public static bool IsRegistered(TextureApplyAsyncHandle handle)
        {
            return _applyHandlesEveryFrame.Contains(handle) || _applyHandlesThisFrame.Contains(handle);
        }

        internal static void MarkDirty(TextureApplyAsyncHandle handle)
        {
            if (!_isCommandBufferDirty)
            {
                _isCommandBufferDirty = IsRegistered(handle);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            _commandBuffer = new CommandBuffer
            {
                name = nameof(TextureAsyncApplier),
            };
            Application.quitting += Dispose;
        }

        private static void Dispose()
        {
            _applyHandlesEveryFrame.Clear();
            _applyHandlesThisFrame.Clear();

            UnregisterOnPreRender();

            _commandBuffer?.Dispose();
            _commandBuffer = null;
        }

        private static void RegisterOnPreRender()
        {
            if (IsUsingScriptableRenderPipeline)
            {
                RenderPipelineManager.beginContextRendering += CachedOnBeginContextRendering;
            }
            else
            {
                Camera.onPreRender += CachedOnPreRender;
            }
            _isOnPreRenderRegistered = true;
        }

        private static void UnregisterOnPreRender()
        {
            if (IsUsingScriptableRenderPipeline)
            {
                RenderPipelineManager.beginContextRendering -= CachedOnBeginContextRendering;
            }
            else
            {
                Camera.onPreRender -= CachedOnPreRender;
            }
            if (_registeredCamera && _commandBuffer != null)
            {
                _registeredCamera.RemoveCommandBuffer(_registeredCamera.GetFirstCameraEvent(), _commandBuffer);
            }
            _isOnPreRenderRegistered = false;
        }

        private static void RebuildCommandBuffer()
        {
            _commandBuffer.Clear();
            foreach (TextureApplyAsyncHandle handle in _applyHandlesEveryFrame)
            {
                handle.FillCommandBuffer(_commandBuffer);
            }
            foreach (TextureApplyAsyncHandle handle in _applyHandlesThisFrame)
            {
                handle.FillCommandBuffer(_commandBuffer);
            }
            _isCommandBufferDirty = false;
        }

        #region Builtin Render Pipeline

        private static readonly Camera.CameraCallback CachedOnPreRender = OnPreRender;
        private static void OnPreRender(Camera camera)
        {
            int currentFrame = Time.frameCount;
            if (currentFrame != _lastProcessedFrame)
            {
                _lastProcessedFrame = currentFrame;
                OnPreRenderFirstCamera(camera);
            }
        }

        private static void OnPreRenderFirstCamera(Camera camera)
        {
            if (HandlesCount == 0)
            {
                UnregisterOnPreRender();
                return;
            }

            if (!_isCommandBufferDirty && camera == _registeredCamera)
            {
                return;
            }

            if (_isCommandBufferDirty)
            {
                RebuildCommandBuffer();
            }
            if (_applyHandlesThisFrame.Count > 0)
            {
                _applyHandlesThisFrame.Clear();
                _isCommandBufferDirty = true;
            }
            if (_registeredCamera)
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
#pragma warning disable CS0618 // Type or member is obsolete
                case RenderingPath.DeferredLighting:
#pragma warning restore CS0618 // Type or member is obsolete
                case RenderingPath.DeferredShading:
                    return CameraEvent.BeforeGBuffer;

                default:
                    return CameraEvent.BeforeForwardOpaque;
            }
        }

        #endregion // Builtin Render Pipeline

        #region Scriptable Render Pipeline

        private static readonly Action<ScriptableRenderContext, List<Camera>> CachedOnBeginContextRendering = OnBeginContextRendering;
        private static void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            if (HandlesCount == 0)
            {
                UnregisterOnPreRender();
                return;
            }

            if (_isCommandBufferDirty)
            {
                RebuildCommandBuffer();
            }
            if (_applyHandlesThisFrame.Count > 0)
            {
                _applyHandlesThisFrame.Clear();
                _isCommandBufferDirty = true;
            }
            context.ExecuteCommandBuffer(_commandBuffer);
        }

        #endregion // Scriptable Render Pipeline
    }
}
