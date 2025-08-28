using ImGuiNET;
using System.Text.RegularExpressions;
using System.Xml;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;


namespace DieselTools_ExileAPI;

public static class IconSelect
{

    public class Options {
        public SVector2? IconPickerWindowOffset;
        public SVector2 PositionOffset { get; set; } = new SVector2(0, 0);
        public int? Width { get; set; }
        public int? Height { get; set; }
        public uint IconColor { get; set; } = Colors.White;
        public uint Color { get; set; } = Colors.Button;
        public uint? BorderColor { get; set; } = Colors.Black;
        public uint? OuterGlowColor { get; set; } = Colors.ControlOuterGlow;
        public uint? InnerGlowColor { get; set; } = Colors.ControlInnerGlow;
        public uint? HoveredColor { get; set; } = Colors.ButtonHovered;
    }

    private static void InternalDraw(string uniqueID, string label, ref int selectedIndex, IconAtlas iconAtlas, Options options) {
        if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueId cannot be null or empty", nameof(uniqueID));
        if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");

        // calc 
        var pos = ImGui.GetCursorScreenPos() + options.PositionOffset;
        var width = options.Width ?? ImGui.GetFrameHeight();
        var height = options.Height ?? ImGui.GetFrameHeight();

        // draw style
        var drawList = ImGui.GetWindowDrawList();
        drawList.AddRectFilled(pos, pos + new SVector2(width, height), options.Color);
        if (options.OuterGlowColor != null) drawList.AddRect(pos, pos + new SVector2(width, height), options.OuterGlowColor.Value);

        // imgui button
        ImGui.SetCursorScreenPos(pos);
        var clicked = ImGui.InvisibleButton($"##{uniqueID}", new SVector2(width, height));
        if (ImGui.IsItemHovered()) {
            if (options.HoveredColor != null) drawList.AddRectFilled(pos + new SVector2(1, 1), pos + new SVector2(width - 1, height - 1), options.HoveredColor.Value);

            Tooltip.Draw(new Tooltip.Options {
                Lines = new List<Tooltip.Line> {
                new Tooltip.Title { Text = label },
                new Tooltip.Separator(),
                new Tooltip.DoubleLine { LeftText = "Index:", RightText = $"{selectedIndex}" },
            }});
        }

        var (uv0, uv1) = iconAtlas.GetIconUVs(selectedIndex);
        drawList.AddImage(iconAtlas.TextureId, pos, pos + new SVector2(width, height), uv0, uv1, options.IconColor);

        if (options.BorderColor != null) drawList.AddRect(pos, pos + new SVector2(width, height), options.BorderColor.Value);
        if (options.InnerGlowColor != null) drawList.AddRect(pos + new SVector2(1, 1), pos + new SVector2(width - 1, height - 1), options.InnerGlowColor.Value);

        if (clicked) IconPicker.Open(uniqueID, iconAtlas, options.IconPickerWindowOffset);
        IconPicker.Draw(uniqueID, iconAtlas, ref selectedIndex, new IconPicker.Options { Title = label, IconColor = options.IconColor });
    }

    /// <summary>
    /// Draws an icon for the specified atlast index.
    /// When the icon is clicked, an icon picker popup appears. 
    /// </summary>
    public static void Draw(string uniqueID, string label, ref int selectedIndex, IconAtlas iconAtlas, Options? options = null) {
        options ??= new Options();
        InternalDraw(uniqueID, label, ref selectedIndex, iconAtlas, options);
    }

}
