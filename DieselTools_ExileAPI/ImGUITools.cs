using ImGuiNET;
using SVector4 = System.Numerics.Vector4;
using SVector2 = System.Numerics.Vector2;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DieselTools_ExileAPI;

public static class ImGUITools {

    


    private static SVector2 _popupOffset = new(10, 10);
    public static void ColorSwatch(string desc_id, ref SVector4 color) {
        if (ImGui.ColorButton(desc_id, color)) {
            ImGui.SetNextWindowPos(ImGui.GetMousePos() + _popupOffset, ImGuiCond.Always);
            ImGui.OpenPopup(desc_id);
        }
        if (ImGui.BeginPopup(desc_id)) {
            ImGui.ColorPicker4(desc_id, ref color);
            ImGui.EndPopup();
        }
    }


    public static string StripID(string label) {
        int idx = label.IndexOf("##");
        return idx >= 0 ? label.Substring(0, idx) : label;
    }

    /// <summary>
    /// Renders a checkbox with an optional tooltip.
    /// </summary>
    /// <param name="label">The label for the checkbox.</param>
    /// <param name="tooltip">\Ttooltip text to display when the checkbox is hovered.</param>
    /// <param name="value">The boolean value of the checkbox.</param>
    public static void Checkbox(string label, string tooltip, ref bool value) {
        ImGui.Checkbox(label, ref value);
        if (ImGui.IsItemHovered()) {
            ImGui.BeginTooltip();
            ImGui.Text(tooltip);
            ImGui.EndTooltip();
        }
    }

    public static bool CollapsingHeader(string label, ref bool open) {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
        if (open) flags |= ImGuiTreeNodeFlags.DefaultOpen;

        open = ImGui.CollapsingHeader(label, flags);

        return open;
    }

    public static SharpDX.Color Vector4ToDxColor(SVector4 vector) {
        // Ensure the vector components are in the range [0, 1]
        vector = SVector4.Clamp(vector, SVector4.Zero, SVector4.One);

        int alpha = (int)(vector.W * 255);
        int red = (int)(vector.X * 255);
        int green = (int)(vector.Y * 255);
        int blue = (int)(vector.Z * 255);

        return new SharpDX.Color(red, green, blue, alpha);
    }


    public static void DrawCheckerboard(SVector2 pos, float width, float height, int cellSize = 4, uint col1 = 0xFFCCCCCC, uint col2 = 0xFF888888) {
        var drawList = ImGui.GetWindowDrawList();
        int cols = (int)Math.Ceiling(width / cellSize);
        int rows = (int)Math.Ceiling(height / cellSize);
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < cols; x++) {
                uint col = (x + y) % 2 == 0 ? col1 : col2;
                SVector2 p0 = pos + new SVector2(x * cellSize, y * cellSize);
                SVector2 p1 = pos + new SVector2(
                    Math.Min((x + 1) * cellSize, width),
                    Math.Min((y + 1) * cellSize, height)
                );
                drawList.AddRectFilled(p0, p1, col);
            }
        }
    }





}
