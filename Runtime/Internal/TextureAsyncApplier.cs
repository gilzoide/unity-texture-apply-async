#if UNITY_2021_1_OR_NEWER
    #define HAVE_RENDER_PIPELINE_MANAGER_BEGIN_CONTEXT_RENDERING
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gilzoide.TextureApplyAsync.Internal
{
    public static class TextureAsyncApplier
    {
        private static List<TextureApplyAsyncHandle> _applyHandlesEveryFrame = new List<TextureApplyAsyncHandle>();
        private static List<TextureApplyAsyncHandle> _applyHandlesThisFrame = new List<TextureApplyAsyncHandle>();
        private static CommandBuffer _commandBuffer;
        private static Camera _registeredCamera;
        private static int _lastProcessedFrame;
        private static bool _isCommandBufferDirty;
        private static bool _isOnPreRenderRegistered;

        private static int HandlesCount => _applyHandlesEveryFrame.Count + _applyHandlesThisFrame.Count;
#if UNITY_2019_3_OR_NEWER
        private static bool IsUsingScriptableRenderPipeline => GraphicsSettings.currentRenderPipeline != null;
#else
        private static bool IsUsingScriptableRenderPipeline => GraphicsSettings.renderPipelineAsset != null;
#endif

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
#if HAVE_RENDER_PIPELINE_MANAGER_BEGIN_CONTEXT_RENDERING
                RenderPipelineManager.beginContextRendering += CachedOnBeginContextRendering;
#else
                RenderPipelineManager.beginFrameRendering += CachedOnBeginFrameRendering;
#endif
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
#if HAVE_RENDER_PIPELINE_MANAGER_BEGIN_CONTEXT_RENDERING
                RenderPipelineManager.beginContextRendering -= CachedOnBeginContextRendering;
#else
                RenderPipelineManager.beginFrameRendering -= CachedOnBeginFrameRendering;
#endif
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
#pragma warning disable CS0618 // Type or member is obsolete
            switch (camera.actualRenderingPath)
            {
                case RenderingPath.DeferredLighting:
                case RenderingPath.DeferredShading:
                    return CameraEvent.BeforeGBuffer;

                default:
                    return CameraEvent.BeforeForwardOpaque;
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        #endregion // Builtin Render Pipeline

        #region Scriptable Render Pipeline

#if HAVE_RENDER_PIPELINE_MANAGER_BEGIN_CONTEXT_RENDERING
        private static readonly Action<ScriptableRenderContext, List<Camera>> CachedOnBeginContextRendering = OnBeginContextRendering;
        private static void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
#else
        private static readonly Action<ScriptableRenderContext, Camera[]> CachedOnBeginFrameRendering = OnBeginFrameRendering;
        private static void OnBeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
#endif
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
