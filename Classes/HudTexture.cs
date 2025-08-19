namespace MapIcons;

public class HudTexture
{
    public HudTexture() {
    }

    public HudTexture(string fileName) {
        FileName = fileName;
    }

    public string FileName { get; set; }
    public SharpDX.RectangleF UV { get; set; } = new SharpDX.RectangleF(0, 0, 1, 1);
    public float Size { get; set; } = 13;
    public System.Drawing.Color Color { get; set; } = System.Drawing.Color.White;
}
