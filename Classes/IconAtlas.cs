using ExileCore;
using ImGuiNET;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using RectangleF = SharpDX.RectangleF;

namespace MapIcons;

public class IconAtlas
{
    private readonly Graphics _graphics;

    public string Name { get; }
    public string FilePath { get; }
    public nint TextureId { get; }
    public Vector2 IconSize { get; }
    public Vector2 AtlasSize { get; }
    public int IconsPerRow { get; }
    public int TotalIcons { get; }
    public IconAtlas(Graphics graphics, string name, string filePath, Vector2 iconSize)
    {
        if (graphics == null) throw new ArgumentNullException(nameof(graphics));

        if (File.Exists(filePath))
        {

            using (Image<Rgba32> image = Image.Load<Rgba32>(filePath))
            {
                AtlasSize = new Vector2(image.Width, image.Height);
                Log.Write($"Image Size: {AtlasSize}");
            }

            if (AtlasSize.X != AtlasSize.Y || AtlasSize.X % 2 != 0)
            {
                throw new Exception("Invalid image size. The image dimensions must be the same and powers of two.");
            }

            if (graphics.InitImage(name, filePath))
            {
                TextureId = graphics.GetTextureId(name);
                Log.Write($"Icons Loaded from: {filePath}");
            }
            else
            {
                throw new Exception("Failed to initialize image.");
            }
        }
        else
        {
            throw new FileNotFoundException($"Texture file not found: {filePath}");
        }

        Name = name;
        FilePath = filePath;
        IconSize = iconSize;
        IconsPerRow = (int)(AtlasSize.X / iconSize.X);
        TotalIcons = (int)(AtlasSize.X / iconSize.X) * (int)(AtlasSize.Y / iconSize.Y);
    }
    public RectangleF GetIconUV(int iconIndex)
    {
        float x = iconIndex % IconsPerRow * IconSize.X / AtlasSize.X;
        float y = iconIndex / IconsPerRow * IconSize.Y / AtlasSize.Y;
        return new RectangleF(x, y, IconSize.X / AtlasSize.X, IconSize.Y / AtlasSize.Y);
    }

    public (Vector2 uv0, Vector2 uv1) GetIconUVs(int iconIndex)
    {
        int x = iconIndex % IconsPerRow;
        int y = iconIndex / IconsPerRow;

        float u0 = x * IconSize.X / AtlasSize.X;
        float v0 = y * IconSize.Y / AtlasSize.Y;
        float u1 = (x + 1) * IconSize.X / AtlasSize.X;
        float v1 = (y + 1) * IconSize.Y / AtlasSize.Y;

        Vector2 uv0 = new Vector2(u0, v0);
        Vector2 uv1 = new Vector2(u1, v1);
        return (uv0, uv1);
    }

    /// <summary>
    /// Shows an icon picker window for this atlas.
    /// </summary>
    /// <param name="name">The name of the window.</param>
    /// <param name="selectedIcon">The currently selected icon index.</param>
    /// <param name="tint">The optional tint to apply to the icons.</param>
    /// <returns>True if the window is open</returns>
    public bool ShowIconPickerWindow(string name, ref int selectedIcon, Vector4? tint = null)
    {
        if (TextureId == nint.Zero) return false;

        var show = true;
        int iconsPerRow = (int)(AtlasSize.X / IconSize.X);
        int iconsPerColumn = (int)(AtlasSize.Y / IconSize.Y);

        uint childBgColor = ImGui.GetColorU32(ImGuiCol.ChildBg);
        uint buttonBgColor = ImGui.GetColorU32(ImGuiCol.Button);

        Vector2 windowPadding = ImGui.GetStyle().WindowPadding;
        Vector2 originalFramePadding = ImGui.GetStyle().FramePadding;
        ImGui.GetStyle().FramePadding = Vector2.Zero;

        float titleBarHeight = ImGui.GetFrameHeight();

        Vector2 windowSize = new Vector2(
            AtlasSize.X + windowPadding.X * 2,
            AtlasSize.Y + windowPadding.Y * 2 + titleBarHeight
        );

        ImGui.Begin($"{name}###IconPickerWindow", ref show, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar);

        Vector2 cursorPos = ImGui.GetCursorPos();

        for (int y = 0; y < iconsPerColumn; y++)
        {
            for (int x = 0; x < iconsPerRow; x++)
            {
                int iconIndex = y * iconsPerRow + x;
                (Vector2 uv0, Vector2 uv1) = GetIconUVs(iconIndex);

                ImGui.PushStyleColor(ImGuiCol.Button, iconIndex == selectedIcon ? buttonBgColor : childBgColor);
                ImGui.SetCursorPos(new Vector2(cursorPos.X + x * IconSize.X, cursorPos.Y + y * IconSize.Y));
                if (ImGui.ImageButton($"##textureID_{y}_{x}", TextureId, IconSize, uv0, uv1, Vector4.Zero, tint ?? new Vector4(1f, 1f, 1f, 1f)))
                {
                    selectedIcon = iconIndex;
                    show = false;
                }
                ImGui.PopStyleColor();

                if (x < iconsPerRow - 1) ImGui.SameLine(0, 0);
            }
        }

        ImGui.GetStyle().FramePadding = originalFramePadding;
        ImGui.End();
        return show;
    }

}
