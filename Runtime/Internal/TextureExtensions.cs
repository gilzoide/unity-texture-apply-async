using UnityEngine;

namespace Gilzoide.TextureAsyncApply
{
    public static class TextureExtensions
    {
        public static int GetSizeInBytes(this Texture2D texture)
        {
#if UNITY_2022_2_OR_NEWER
            return (int) UnityEngine.Experimental.Rendering.GraphicsFormatUtility.ComputeMipmapSize(texture.width, texture.height, texture.format);
#else
            int pixelCount = texture.width * texture.height;
            switch (texture.format)
            {
                //
                // Summary:
                //     Alpha-only texture format, 8 bit integer.
                case TextureFormat.Alpha8:
                    return pixelCount;
                //
                // Summary:
                //     A 16 bits/pixel texture format. Texture stores color with an alpha channel.
                case TextureFormat.ARGB4444:
                    return pixelCount * 2;
                //
                // Summary:
                //     Color texture format, 8-bits per channel.
                case TextureFormat.RGB24:
                    return pixelCount;
                //
                // Summary:
                //     Color with alpha texture format, 8-bits per channel.
                case TextureFormat.RGBA32:
                    return pixelCount * 4;
                //
                // Summary:
                //     Color with alpha texture format, 8-bits per channel.
                case TextureFormat.ARGB32:
                    return pixelCount * 4;
                //
                // Summary:
                //     A 16 bit color texture format.
                case TextureFormat.RGB565:
                    return pixelCount * 2;
                //
                // Summary:
                //     Single channel (R) texture format, 16 bit integer.
                case TextureFormat.R16:
                    return pixelCount * 2;
                //
                // Summary:
                //     Compressed color texture format.
                case TextureFormat.DXT1:
                    return pixelCount / 2;
                //
                // Summary:
                //     Compressed color with alpha channel texture format.
                case TextureFormat.DXT5:
                    return pixelCount;
                //
                // Summary:
                //     Color and alpha texture format, 4 bit per channel.
                case TextureFormat.RGBA4444:
                    return pixelCount * 2;
                //
                // Summary:
                //     Color with alpha texture format, 8-bits per channel.
                case TextureFormat.BGRA32:
                    return pixelCount * 4;
                //
                // Summary:
                //     Scalar (R) texture format, 16 bit floating point.
                case TextureFormat.RHalf:
                    return pixelCount * 2;
                //
                // Summary:
                //     Two color (RG) texture format, 16 bit floating point per channel.
                case TextureFormat.RGHalf:
                    return pixelCount * 4;
                //
                // Summary:
                //     RGB color and alpha texture format, 16 bit floating point per channel.
                case TextureFormat.RGBAHalf:
                    return pixelCount * 4;
                //
                // Summary:
                //     Scalar (R) texture format, 32 bit floating point.
                case TextureFormat.RFloat:
                    return pixelCount * 4;
                //
                // Summary:
                //     Two color (RG) texture format, 32 bit floating point per channel.
                case TextureFormat.RGFloat:
                    return pixelCount * 8;
                //
                // Summary:
                //     RGB color and alpha texture format, 32-bit floats per channel.
                case TextureFormat.RGBAFloat:
                    return pixelCount * 16;
                //
                // Summary:
                //     RGB HDR format, with 9 bit mantissa per channel and a 5 bit shared exponent.
                case TextureFormat.RGB9e5Float:
                    return pixelCount * 4;
                //
                // Summary:
                //     Compressed one channel (R) texture format.
                case TextureFormat.BC4:
                    return pixelCount / 2;
                //
                // Summary:
                //     Compressed two-channel (RG) texture format.
                case TextureFormat.BC5:
                    return pixelCount;
                //
                // Summary:
                //     HDR compressed color texture format.
                case TextureFormat.BC6H:
                    return pixelCount;
                //
                // Summary:
                //     High quality compressed color texture format.
                case TextureFormat.BC7:
                    return pixelCount;
                //
                // Summary:
                //     PowerVR (iOS) 2 bits/pixel compressed color texture format.
                case TextureFormat.PVRTC_RGB2:
                    return pixelCount / 4;
                //
                // Summary:
                //     PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format.
                case TextureFormat.PVRTC_RGBA2:
                    return pixelCount / 4;
                //
                // Summary:
                //     PowerVR (iOS) 4 bits/pixel compressed color texture format.
                case TextureFormat.PVRTC_RGB4:
                    return pixelCount / 2;
                //
                // Summary:
                //     PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format.
                case TextureFormat.PVRTC_RGBA4:
                    return pixelCount / 2;
                //
                // Summary:
                //     ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
                case TextureFormat.ETC_RGB4:
                    return pixelCount / 2;
                //
                // Summary:
                //     ETC2 EAC (GL ES 3.0) 4 bitspixel compressed unsigned single-channel texture format.
                case TextureFormat.EAC_R:
                    return pixelCount / 2;
                //
                // Summary:
                //     ETC2 EAC (GL ES 3.0) 4 bitspixel compressed signed single-channel texture format.
                case TextureFormat.EAC_R_SIGNED:
                    return pixelCount / 2;
                //
                // Summary:
                //     ETC2 EAC (GL ES 3.0) 8 bitspixel compressed unsigned dual-channel (RG) texture
                //     format.
                case TextureFormat.EAC_RG:
                    return pixelCount;
                //
                // Summary:
                //     ETC2 EAC (GL ES 3.0) 8 bitspixel compressed signed dual-channel (RG) texture
                //     format.
                case TextureFormat.EAC_RG_SIGNED:
                    return pixelCount;
                //
                // Summary:
                //     ETC2 (GL ES 3.0) 4 bits/pixel compressed RGB texture format.
                case TextureFormat.ETC2_RGB:
                    return pixelCount / 2;
                //
                // Summary:
                //     ETC2 (GL ES 3.0) 8 bits/pixel compressed RGBA texture format.
                case TextureFormat.ETC2_RGBA8:
                    return pixelCount;
                //
                // Summary:
                //     Two color (RG) texture format, 8-bits per channel.
                case TextureFormat.RG16:
                    return pixelCount * 2;
                //
                // Summary:
                //     Single channel (R) texture format, 8 bit integer.
                case TextureFormat.R8:
                    return pixelCount;
                //
                // Summary:
                //     Two channel (RG) texture format, 16 bit integer per channel.
                case TextureFormat.RG32:
                    return pixelCount * 4;
                //
                // Summary:
                //     Three channel (RGB) texture format, 16 bit integer per channel.
                case TextureFormat.RGB48:
                    return pixelCount * 6;
                //
                // Summary:
                //     Four channel (RGBA) texture format, 16 bit integer per channel.
                case TextureFormat.RGBA64:
                    return pixelCount * 8;

                default:
                    throw new System.ArgumentOutOfRangeException(nameof(texture), "Texture format is not supported yet.");
            }
#endif
        }
    }
}
