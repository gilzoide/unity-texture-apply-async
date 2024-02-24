using Gilzoide.TextureAsyncApply;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

public class PlasmaTextureAsyncUpdater : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField, Min(1)] private int width = 128;
    [SerializeField, Min(1)] private int height = 128;

    private Texture2D _texture;
    private TextureAsyncApplyHandle _asyncApplyHandle;
    private JobHandle _jobHandle;

    void OnEnable()
    {
        if (_texture == null)
        {
            _texture = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
        }
        _texture.Apply(false, true);
        rawImage.texture = _texture;
        _asyncApplyHandle = new TextureAsyncApplyHandle(_texture);
        _asyncApplyHandle.RegisterInRenderLoop();
    }

    void OnDisable()
    {
        _asyncApplyHandle.UnregisterFromRenderLoop();
    }

    void OnDestroy()
    {
        _asyncApplyHandle?.Dispose();
        Destroy(_texture);
    }

    void Update()
    {
        NativeArray<Color32> pixels = _asyncApplyHandle.Buffer.Reinterpret<Color32>(sizeof(byte));
        _jobHandle = new PlasmaColorJob
        {
            Time = Time.time,
            Width = _texture.width,
            Height = _texture.height,
            Pixels = _asyncApplyHandle.Buffer.Reinterpret<Color32>(sizeof(byte)),
        }.Schedule(pixels.Length, 64);
    }

    void LateUpdate()
    {
        _jobHandle.Complete();
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
