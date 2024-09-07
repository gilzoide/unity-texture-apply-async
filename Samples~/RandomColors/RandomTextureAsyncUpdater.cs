using Gilzoide.TextureApplyAsync;
using UnityEngine;
using UnityEngine.UI;

public class RandomTextureAsyncUpdater : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;

    private Texture2D _texture;
    private TextureApplyAsyncHandle _textureApplyAsyncHandle;

    void Start()
    {
        _texture = new Texture2D(64, 64, TextureFormat.RGBA32, false, false);
        rawImage.texture = _texture;

        // 1. Create a `TextureApplyAsyncHandle` for your `Texture`
        _textureApplyAsyncHandle = new TextureApplyAsyncHandle(_texture);

        // 2. If you want to update your texture every frame,
        // schedule the apply handle to update every frame.
        _textureApplyAsyncHandle.ScheduleUpdateEveryFrame();
    }

    void Update()
    {
        // 3. Update the texture data normally
        for (int x = 0; x < _texture.width; x++)
        {
            for (int y = 0; y < _texture.height; y++)
            {
                _texture.SetPixel(x, y, Random.ColorHSV());
            }
        }

        // 4. If you want to update your texture only once,
        // schedule a one-shot update.
        _textureApplyAsyncHandle.ScheduleUpdateOnce();
    }

    void OnDestroy()
    {
        // 5. Dispose of the `TextureApplyAsyncHandle` when not needed anymore
        _textureApplyAsyncHandle.Dispose();
        Destroy(_texture);
    }
}
