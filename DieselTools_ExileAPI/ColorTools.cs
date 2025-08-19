using ImGuiNET;
using SVector4 = System.Numerics.Vector4;
using SVector2 = System.Numerics.Vector2;

namespace DieselTools_ExileAPI;

public static class ColorTools { 


    /// <summary>
    /// Converts RGBA byte values (0-255) to an ImGui-compatible uint color.
    /// </summary>
    /// <param name="r">Red component (0-255).</param>
    /// <param name="g">Green component (0-255).</param>
    /// <param name="b">Blue component (0-255).</param>
    /// <param name="a">Alpha component (0-255, default is 255).</param>
    /// <returns>rgba color as uint.</returns>
    public static uint RGBA2Uint(byte r, byte g, byte b, byte a=255) {
        return ImGui.ColorConvertFloat4ToU32( new SVector4(r / 255f, g / 255f, b / 255f, a / 255f) );
    }

    public static uint Hex2Uint(string hex) {
        hex = hex.Replace("#", "");
        if (hex.Length == 6) hex += "FF"; // Assume alpha = 255 if not provided
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);
        byte a = Convert.ToByte(hex.Substring(6, 2), 16);
        return RGBA2Uint(r, g, b, a);
    }

    /// <summary>
    /// Converts ( hue[0-360], saturation[1-100], lightness[1-100], alpha[1-100] ) to an ImGui-compatible packed uint color.
    /// </summary>
    /// <param name="h">Hue in degrees (0-360).</param>
    /// <param name="s">Saturation percentage (0-100).</param>
    /// <param name="l">Lightness percentage (0-100).</param>
    /// <param name="a">Alpha percentage (0-100, default is 100).</param>
    /// <returns>Packed RGBA color as uint for ImGui.</returns>
    public static uint HSLA2Uint(int h, float s, float l, float a = 100f) {
        // Accept hue as 0-360 directly
        float sat = s / 100f;
        float light = l / 100f;
        float alpha = a / 100f;

        SVector4 rgba = HSLA2RGBA(h, sat, light, alpha);
        return ImGui.ColorConvertFloat4ToU32(rgba);
    }

    /// <summary>
    /// Converts HSLA color values to RGBA color space (all channels in [0,1]).
    /// </summary>
    /// <param name="h">Hue in degrees (float, can be any value; wrapped to [0,360)).</param>
    /// <param name="s">Saturation (float, [0,1]).</param>
    /// <param name="l">Lightness (float, [0,1]).</param>
    /// <param name="a">Alpha (float, [0,1]).</param>
    /// <returns>Vector4 (R, G, B, A), each in [0,1].</returns>
    public static SVector4 HSLA2RGBA(float h, float s, float l, float a) {
        h = (h % 360f + 360f) % 360f; // Wrap hue to [0,360)
        s = Math.Clamp(s, 0f, 1f); // 0-1
        l = Math.Clamp(l, 0f, 1f); // 0-1
        a = Math.Clamp(a, 0f, 1f); // 0-1

        float c = (1f - Math.Abs(2f * l - 1f)) * s;
        float x = c * (1f - Math.Abs((h / 60f) % 2f - 1f));
        float m = l - c / 2f;
        float r = 0f, g = 0f, b = 0f;

        if (h < 60f) { r = c; g = x; b = 0f; }
        else if (h < 120f) { r = x; g = c; b = 0f; }
        else if (h < 180f) { r = 0f; g = c; b = x; }
        else if (h < 240f) { r = 0f; g = x; b = c; }
        else if (h < 300f) { r = x; g = 0f; b = c; }
        else { r = c; g = 0f; b = x; }

        return new SVector4(r + m, g + m, b + m, a);
    }

    /// <summary>
    /// Converts ( red[0-1], green[0-1], blue[0-1], alpha[0-1] ) to HSLA color space.
    /// Output: ( hue[0-360], saturation[0-1], lightness[0-1], alpha[0-1] )
    /// </summary>
    /// <param name="rgba">Input color as Vector4 (R, G, B, A), each in [0,1].</param>
    /// <param name="h">Output hue in degrees (0-360).</param>
    /// <param name="s">Output saturation (0-1).</param>
    /// <param name="l">Output lightness (0-1).</param>
    /// <param name="a">Output alpha (0-1, copied from input).</param>
    public static void RGBA2HSLA(SVector4 rgba, out float h, out float s, out float l, out float a) {
        float r = rgba.X;
        float g = rgba.Y;
        float b = rgba.Z;
        a = rgba.W;

        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        l = (max + min) / 2f;

        if (max == min) {
            h = 0f;
            s = 0f;
        }
        else {
            float d = max - min;
            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

            if (max == r)
                h = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g)
                h = (b - r) / d + 2f;
            else
                h = (r - g) / d + 4f;

            h *= 60f; // Convert to degrees
        }
    }



}
