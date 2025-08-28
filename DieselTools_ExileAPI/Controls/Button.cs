using ImGuiNET;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;


namespace DieselTools_ExileAPI;

public static class Button {

    private enum ClickType
    {
        None,
        Left,
        Other
    }

    public class Options
    {
        public Tooltip.Options? Tooltip { get; set; }
        public SVector2 PositionOffset { get; set; } = new SVector2(0, 0);
        public int? Width { get; set; }
        public int? Height { get; set; }
        public uint? BorderColor { get; set; } = Colors.Black;
        public uint? OuterGlowColor { get; set; } = Colors.ControlOuterGlow;
        public uint? InnerGlowColor { get; set; } = Colors.ControlInnerGlow;
        public uint Color { get; set; } = Colors.Button;
        public uint CheckedColor { get; set; } = Colors.ButtonChecked;
        public uint HoveredColor { get; set; } = Colors.ButtonHovered;


        public string? Label { get; set; }

    }
    // Normal button
    public static bool Draw(string uniqueId, Options? options = null) {
        var clickType = InternalDraw(uniqueId, options, null);
        return clickType != ClickType.None;
    }

    // Toggleable button
    public static bool Draw(string uniqueId, ref bool checkedState, Options? options = null) {
        var clickType = InternalDraw(uniqueId, options, checkedState);
        if (clickType == ClickType.None) return false;
        if (clickType == ClickType.Left) {
            checkedState = !checkedState;
        }
        return true;
    }

    // Internal draw method
    private static ClickType InternalDraw(string uniqueId, Options? options, bool? checkedState) {
        if (string.IsNullOrEmpty(uniqueId)) throw new ArgumentException("uniqueId cannot be null or empty", nameof(uniqueId));
        if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");
        
        // calc 
        var pos = ImGui.GetCursorScreenPos() + options.PositionOffset;
        var width = options.Width ?? ImGui.GetContentRegionAvail().X;
        var height = options.Height ?? ImGui.GetFrameHeight();
        var textSize = new SVector2(0, 0);
        var textPos = new SVector2(0, 0);
        if (!string.IsNullOrEmpty(options.Label)) {
            textSize = ImGui.CalcTextSize(options.Label);
            textPos = pos + new SVector2(
                (float)Math.Round((width - textSize.X) / 2),
                (float)Math.Ceiling((height - textSize.Y) / 2) - 2
            );
        }
        uint fillColor = (checkedState.HasValue && checkedState.Value) ? options.CheckedColor : options.Color;

        // draw
        var drawList = ImGui.GetWindowDrawList();
        drawList.AddRectFilled(pos, pos + new SVector2(width, height), fillColor);
        if (options.BorderColor != null) drawList.AddRect(pos, pos + new SVector2(width, height), options.BorderColor.Value);
        if (options.OuterGlowColor != null) drawList.AddRect(pos, pos + new SVector2(width, height), options.OuterGlowColor.Value);

        ImGui.SetCursorScreenPos(pos);
        ImGui.InvisibleButton($"##{uniqueId}", new SVector2(width, height));
        if (ImGui.IsItemHovered()) {
            drawList.AddRectFilled(pos + new SVector2(1, 1), pos + new SVector2(width - 1, height - 1), options.HoveredColor);
            if (textSize.X > 0) drawList.AddText(textPos, Colors.White, options.Label);
            if (options.Tooltip != null)
                Tooltip.Draw(options.Tooltip);
        }
        else if (textSize.X > 0) {
            drawList.AddText(textPos, Colors.ControlText, options.Label);
        }

        if (options.InnerGlowColor != null) drawList.AddRect(pos + new SVector2(1, 1), pos + new SVector2(width - 1, height - 1), options.InnerGlowColor.Value);

        if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) {
            return ClickType.Other; // Right click
        }
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left)) {
            return ClickType.Left; // Left click
        }
        return ClickType.None;
    }


}
