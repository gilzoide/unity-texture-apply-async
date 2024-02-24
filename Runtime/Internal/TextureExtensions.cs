using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Gilzoide.TextureApplyAsync
{
    public static class TextureExtensions
    {
        public static int GetSizeInBytes(this Texture2D texture)
        {
            return GetSizeInBytes(texture.width, texture.height, texture.graphicsFormat);
        }

        public static int GetSizeInBytes(int width, int height, GraphicsFormat format)
        {
#if UNITY_2022_2_OR_NEWER
            return (int) GraphicsFormatUtility.ComputeMipmapSize(width, height, format);
#else
            int pixelCount = width * height;
            switch (format)
            {
                case GraphicsFormat.R8_SRGB:
                case GraphicsFormat.R8_UNorm:
                case GraphicsFormat.R8_SNorm:
                case GraphicsFormat.R8_UInt:
                case GraphicsFormat.R8_SInt:
                case GraphicsFormat.S8_UInt:
                    return pixelCount * 1;

                case GraphicsFormat.R8G8_SRGB:
                case GraphicsFormat.R8G8_UNorm:
                case GraphicsFormat.R8G8_SNorm:
                case GraphicsFormat.R8G8_UInt:
                case GraphicsFormat.R8G8_SInt:
                    return pixelCount * 2;

                case GraphicsFormat.R8G8B8_SRGB:
                case GraphicsFormat.R8G8B8_UNorm:
                case GraphicsFormat.R8G8B8_SNorm:
                case GraphicsFormat.R8G8B8_UInt:
                case GraphicsFormat.R8G8B8_SInt:
                case GraphicsFormat.B8G8R8_SRGB:
                case GraphicsFormat.B8G8R8_UNorm:
                case GraphicsFormat.B8G8R8_SNorm:
                case GraphicsFormat.B8G8R8_UInt:
                case GraphicsFormat.B8G8R8_SInt:
                    return pixelCount * 3;

                case GraphicsFormat.R8G8B8A8_SRGB:
                case GraphicsFormat.R8G8B8A8_UNorm:
                case GraphicsFormat.R8G8B8A8_SNorm:
                case GraphicsFormat.R8G8B8A8_UInt:
                case GraphicsFormat.R8G8B8A8_SInt:
                case GraphicsFormat.B8G8R8A8_SRGB:
                case GraphicsFormat.B8G8R8A8_UNorm:
                case GraphicsFormat.B8G8R8A8_SNorm:
                case GraphicsFormat.B8G8R8A8_UInt:
                case GraphicsFormat.B8G8R8A8_SInt:
                    return pixelCount * 4;

                case GraphicsFormat.R16_UNorm:
                case GraphicsFormat.R16_SNorm:
                case GraphicsFormat.R16_UInt:
                case GraphicsFormat.R16_SInt:
                case GraphicsFormat.R16_SFloat:
                case GraphicsFormat.D16_UNorm:
                    return pixelCount * 2;

                case GraphicsFormat.R16G16_UNorm:
                case GraphicsFormat.R16G16_SNorm:
                case GraphicsFormat.R16G16_UInt:
                case GraphicsFormat.R16G16_SInt:
                case GraphicsFormat.R16G16_SFloat:
                    return pixelCount * 4;

                case GraphicsFormat.R16G16B16_UNorm:
                case GraphicsFormat.R16G16B16_SNorm:
                case GraphicsFormat.R16G16B16_UInt:
                case GraphicsFormat.R16G16B16_SInt:
                case GraphicsFormat.R16G16B16_SFloat:
                    return pixelCount * 6;

                case GraphicsFormat.R16G16B16A16_UNorm:
                case GraphicsFormat.R16G16B16A16_SNorm:
                case GraphicsFormat.R16G16B16A16_UInt:
                case GraphicsFormat.R16G16B16A16_SInt:
                case GraphicsFormat.R16G16B16A16_SFloat:
                    return pixelCount * 8;

                case GraphicsFormat.R32_UInt:
                case GraphicsFormat.R32_SInt:
                case GraphicsFormat.R32_SFloat:
                    return pixelCount * 4;

                case GraphicsFormat.R32G32_UInt:
                case GraphicsFormat.R32G32_SInt:
                case GraphicsFormat.R32G32_SFloat:
                    return pixelCount * 8;

                case GraphicsFormat.R32G32B32_UInt:
                case GraphicsFormat.R32G32B32_SInt:
                case GraphicsFormat.R32G32B32_SFloat:
                    return pixelCount * 12;

                case GraphicsFormat.R32G32B32A32_UInt:
                case GraphicsFormat.R32G32B32A32_SInt:
                case GraphicsFormat.R32G32B32A32_SFloat:
                    return pixelCount * 16;

                case GraphicsFormat.R4G4B4A4_UNormPack16:
                case GraphicsFormat.B4G4R4A4_UNormPack16:
                case GraphicsFormat.R5G6B5_UNormPack16:
                case GraphicsFormat.B5G6R5_UNormPack16:
                case GraphicsFormat.R5G5B5A1_UNormPack16:
                case GraphicsFormat.B5G5R5A1_UNormPack16:
                case GraphicsFormat.A1R5G5B5_UNormPack16:
                    return pixelCount * 2;

                case GraphicsFormat.E5B9G9R9_UFloatPack32:
                case GraphicsFormat.B10G11R11_UFloatPack32:
                case GraphicsFormat.A2B10G10R10_UNormPack32:
                case GraphicsFormat.A2B10G10R10_UIntPack32:
                case GraphicsFormat.A2B10G10R10_SIntPack32:
                case GraphicsFormat.A2R10G10B10_UNormPack32:
                case GraphicsFormat.A2R10G10B10_UIntPack32:
                case GraphicsFormat.A2R10G10B10_SIntPack32:
                case GraphicsFormat.A2R10G10B10_XRSRGBPack32:
                case GraphicsFormat.A2R10G10B10_XRUNormPack32:
                case GraphicsFormat.R10G10B10_XRSRGBPack32:
                case GraphicsFormat.R10G10B10_XRUNormPack32:
                case GraphicsFormat.A10R10G10B10_XRSRGBPack32:
                case GraphicsFormat.A10R10G10B10_XRUNormPack32:
                    return pixelCount * 4;

                case GraphicsFormat.D24_UNorm:
                case GraphicsFormat.D24_UNorm_S8_UInt:
                case GraphicsFormat.D32_SFloat:
                    return pixelCount * 4;

                case GraphicsFormat.D32_SFloat_S8_UInt:
                    return pixelCount * 8;

                case GraphicsFormat.D16_UNorm_S8_UInt:
                    return pixelCount * 3;

                default:
                    throw new System.ArgumentOutOfRangeException(nameof(format), "Texture format is not supported yet.");
            }
#endif
        }
    }
}
