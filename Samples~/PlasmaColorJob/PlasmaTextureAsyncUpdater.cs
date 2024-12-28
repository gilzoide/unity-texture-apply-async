using Gilzoide.TextureApplyAsync;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

public class PlasmaTextureAsyncUpdater : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField, Min(1)] private int width = 128;
    [SerializeField, Min(1)] private int height = 128;
    [SerializeField] bool _updateAsync = true;

    public bool UpdateAsync
    {
        get => _updateAsync;
        set => _updateAsync = value;
    }

    private Texture2D _texture;
    private TextureApplyAsyncHandle _textureApplyAsyncHandle;
    private JobHandle _jobHandle;

    void OnEnable()
    {
        if (_texture == null)
        {
            _texture = new Texture2D(width, height, TextureFormat.RGBA32, false, false)
            {
                name = name
            };
            rawImage.texture = _texture;
        }
        else
        {
            _texture.Reinitialize(width, height, TextureFormat.RGBA32, false);
            _texture.Apply();
        }
        _textureApplyAsyncHandle = new TextureApplyAsyncHandle(_texture);
    }

    void OnDisable()
    {
        _textureApplyAsyncHandle.Dispose();
    }

    void OnDestroy()
    {
        Destroy(_texture);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (isActiveAndEnabled && _texture)
        {
            _texture.Reinitialize(width, height, TextureFormat.RGBA32, false);
            _texture.Apply();
            _textureApplyAsyncHandle.Reinitialize();
        }
    }
#endif

    void Update()
    {
        NativeArray<Color32> pixels = _texture.GetPixelData<Color32>(0);
        _jobHandle = new PlasmaColorJob
        {
            Time = Time.time,
            Width = _texture.width,
            Height = _texture.height,
            Pixels = pixels,
        }.Schedule(pixels.Length, 64);
    }

    void LateUpdate()
    {
        _jobHandle.Complete();
        if (UpdateAsync)
        {
            _textureApplyAsyncHandle.ScheduleUpdateOnce();
        }
        else
        {
            _texture.Apply();
        }
    }

#if HAVE_BURST
    [Unity.Burst.BurstCompile]
#endif
    private struct PlasmaColorJob : IJobParallelFor
    {
        public float Time;
        public int Height;
        public int Width;
        [WriteOnly] public NativeArray<Color32> Pixels;

        // Old school plasma effect
        // Reference code: https://github.com/keijiro/TextureUpdateExample/blob/master/Plugin/Plasma.c
        static Color32 Plasma(int x, int y, int width, int height, float time)
        {
            float px = (float) x / width;
            float py = (float) y / height;

            float l = Mathf.Sin(px * Mathf.Sin(time * 1.3f) + Mathf.Sin(py * 4 + time) * Mathf.Sin(time));

            byte r = (byte) (Mathf.Sin(l *  6) * 127 + 127);
            byte g = (byte) (Mathf.Sin(l *  7) * 127 + 127);
            byte b = (byte) (Mathf.Sin(l * 10) * 127 + 127);

            return new Color32(r, g, b, 255);
        }

        public void Execute(int i)
        {
            Pixels[i] = Plasma(i % Width, i / Width, Width, Height, Time);
        }
    }
}
