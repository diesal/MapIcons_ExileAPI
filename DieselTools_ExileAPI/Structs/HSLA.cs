using SVector4 = System.Numerics.Vector4;

namespace DieselTools_ExileAPI;



public struct HSLA
{

    private float _h, _s, _l, _a;

    public float H {
        get => _h;
        set => _h = Math.Clamp(value, 0f, 360f);
    }
    public float S {
        get => _s;
        set => _s = Math.Clamp(value, 0f, 1f);
    }
    public float L {
        get => _l;
        set => _l = Math.Clamp(value, 0f, 1f);
    }
    public float A {
        get => _a;
        set => _a = Math.Clamp(value, 0f, 1f);
    }

    public float SPercent {
        get => _s * 100f;
        set => S = value / 100f;
    }
    public float LPercent {
        get => _l * 100f;
        set => L = value / 100f;
    }
    public float APercent {
        get => _a * 100f;
        set => A = value / 100f;
    }
    
    public HSLA(float h, float s, float l, float a = 1f) {
        _h = ((h % 360f) + 360f) % 360f;
        _s = Math.Clamp(s, 0f, 1f);
        _l = Math.Clamp(l, 0f, 1f);
        _a = Math.Clamp(a, 0f, 1f);
    }

    // equality 
    private const float Tolerance = 1f / 255f; // 8-bit color precision
    public static bool operator ==(HSLA left, HSLA right) {
        return Math.Abs(left.H - right.H) < Tolerance &&
               Math.Abs(left.S - right.S) < Tolerance &&
               Math.Abs(left.L - right.L) < Tolerance &&
               Math.Abs(left.A - right.A) < Tolerance;
    }
    public static bool operator !=(HSLA left, HSLA right) {
        return !(left == right);
    }
    public readonly override bool Equals(object obj) {
        return obj is HSLA other && this == other;
    }
    public override int GetHashCode() {
        // use HashCode.Combine for simplicity
        return HashCode.Combine(H, S, L, A);
    }


    /// <summary>
    /// Creates an <see cref="HSLA"/> color from percent[0-100] values for saturation, lightness, and alpha.
    /// </summary>
    /// <param name="h">Hue in degrees (0-360).</param>
    /// <param name="sPercent">Saturation as a percent (0-100).</param>
    /// <param name="lPercent">Lightness as a percent (0-100).</param>
    /// <param name="aPercent">Alpha as a percent (0-100, default is 100).</param>
    /// <returns>HSLA color</returns>
    public static HSLA FromPercent(float h, float sPercent, float lPercent, float aPercent = 100f) {
        return new HSLA(h, sPercent / 100f, lPercent / 100f, aPercent / 100f);
    }

    /// <summary>
    /// Converts this color to a normalized RGBA <see cref="System.Numerics.Vector4"/>.
    /// </summary>
    public readonly SVector4 ToNormalisedRGBA() {
        float h = H;
        float s = S;
        float l = L;
        float a = A;

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
    /// Converts a normalized RGBA <see cref="System.Numerics.Vector4"/> (values in [0,1])
    /// to an <see cref="HSLA"/> color.
    /// </summary>
    /// <param name="rgba">Normalized RGBA vector (X=R, Y=G, Z=B, W=A).</param>
    /// <returns>HSLA color</returns>
    public static HSLA FromNormalisedRGBA(SVector4 rgba) {
        float r = rgba.X;
        float g = rgba.Y;
        float b = rgba.Z;
        float a = rgba.W;

        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        float l = (max + min) / 2f;
        float h, s;

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

            h *= 60f;
        }

        return new HSLA(h, s, l, a);
    }

    /// <summary>
    /// Converts this HSLA color to a packed 32-bit RGBA uint for ImGui (0xRRGGBBAA).
    /// </summary>
    /// <returns>Packed RGBA color as uint.</returns>
    public readonly uint ToImGuiColor() {
        var rgba = ToNormalisedRGBA(); // SVector4 with [0,1] floats
        byte r = (byte)(Math.Clamp(rgba.X, 0f, 1f) * 255f);
        byte g = (byte)(Math.Clamp(rgba.Y, 0f, 1f) * 255f);
        byte b = (byte)(Math.Clamp(rgba.Z, 0f, 1f) * 255f);
        byte a = (byte)(Math.Clamp(rgba.W, 0f, 1f) * 255f);
        return ((uint)r) | ((uint)g << 8) | ((uint)b << 16) | ((uint)a << 24);
    }
}



