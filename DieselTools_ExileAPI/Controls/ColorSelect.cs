using ImGuiNET;
using System.Text.RegularExpressions;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;


namespace DieselTools_ExileAPI;

public static class ColorSelect
{

    public class Options {
        public SVector2? ColorPickerWindowOffset;
        public SVector2? Size; 
        public uint BorderColor = ColorTools.RGBA2Uint(0,0,0);
    }

    private static void InternalDraw(string uniqueID, string label, ref SVector4 rgbaNormalized, Options options = null) {
        if (options == null) options = new Options();

        var size = options.Size ?? new SVector2(ImGui.GetFrameHeight(), ImGui.GetFrameHeight());
        var pos = ImGui.GetCursorScreenPos();
        var buttonClicked = ImGui.InvisibleButton($"##{uniqueID}InvisibleButton", size);
        var drawList = ImGui.GetWindowDrawList();
        ImGUITools.DrawCheckerboard(pos,size.X, size.Y);
        drawList.AddRectFilled(pos, pos + size, ImGui.ColorConvertFloat4ToU32(rgbaNormalized));
        drawList.AddRect(pos, pos + size, options.BorderColor);
        // tooltip 
        if (ImGui.IsItemHovered()) {
            Tooltip.Draw(new Tooltip.Options { Lines = new List<Tooltip.Line> {
                new Tooltip.Title { Text = label },
                new Tooltip.Separator(),
                new Tooltip.DoubleLine { LeftText = "RGBA:", RightText = $"{Math.Round(rgbaNormalized.X * 255)},{Math.Round(rgbaNormalized.Y * 255)},{Math.Round(rgbaNormalized.Z * 255)},{Math.Round(rgbaNormalized.W * 255)}" },
                new Tooltip.DoubleLine { LeftText = "HEX:", RightText = $"#{(int)(rgbaNormalized.X * 255):X2}{(int)(rgbaNormalized.Y * 255):X2}{(int)(rgbaNormalized.Z * 255):X2}{(int)(rgbaNormalized.W * 255):X2}" },
            }});
        }
        if (buttonClicked)ColorPicker.Open(uniqueID, rgbaNormalized, options.ColorPickerWindowOffset);
        ColorPicker.Draw(uniqueID, ref rgbaNormalized, new ColorPicker.Options { Title = label });
    }

    /// <summary>
    /// Draws a color swatch widget for the specified color.
    /// When the swatch is clicked, a color picker popup appears. 
    /// </summary>
    public static void Draw(string uniqueID, string label, ref SharpDX.Color color, Options options = null) {
        var rgbaNormalized = new SVector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        InternalDraw(uniqueID, label, ref rgbaNormalized, options);
        color = new SharpDX.Color(
            (int)Math.Round(rgbaNormalized.X * 255),
            (int)Math.Round(rgbaNormalized.Y * 255),
            (int)Math.Round(rgbaNormalized.Z * 255),
            (int)Math.Round(rgbaNormalized.W * 255)
        );
    }

}
