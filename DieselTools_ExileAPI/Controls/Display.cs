using ImGuiNET;
using SVector2 = System.Numerics.Vector2;

namespace DieselTools_ExileAPI;


public static class Display {
    public class Options {
        public SVector2 PositionOffset { get; set; } = new SVector2(0, 0);
        public int? Width { get; set; }
        public int? Height { get; set; }
        public uint? BackgroundColor { get; set; } = Colors.Input;
        public uint? BorderColor { get; set; } = Colors.Black;
        public uint? OuterGlowColor { get; set; } = Colors.ControlOuterGlow;
        public uint TextColor { get; set; } = Colors.ControlText; 
        public Tooltip.Options? Tooltip { get; set; } = null;
        public bool DrawBackground { get; set; } = true; // <--- Add this line
    }


    public static void Draw(string uniqueID, string value, Options options) {
        if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
        if (options == null) options = new Options(); // safety?
        // position and size
        var pos = ImGui.GetCursorScreenPos() + options.PositionOffset;
        var width = options.Width ?? ImGui.GetContentRegionAvail().X;
        var height = options.Height ?? ImGui.GetFrameHeight();
        // custom theme 
        var drawList = ImGui.GetWindowDrawList();
        if (options.DrawBackground) {
            if (options.BackgroundColor != null) drawList.AddRectFilled(pos, pos + new SVector2(width, height), options.BackgroundColor.Value);
            if (options.BorderColor != null) drawList.AddRect(pos, pos + new SVector2(width, height), options.BorderColor.Value);
            if (options.OuterGlowColor != null) drawList.AddRect(pos - new SVector2(1, 1), pos + new SVector2(width + 1, height + 1), options.OuterGlowColor.Value);
        }
        // Truncate text if needed
        string displayText = value;
        float maxTextWidth = width - 6; // 3px padding left/right
        if (ImGui.CalcTextSize(displayText).X > maxTextWidth) {
            // Truncate and add ellipsis
            int len = displayText.Length;
            while (len > 0 && ImGui.CalcTextSize(displayText.Substring(0, len) + "…").X > maxTextWidth)
                len--;
            displayText = (len > 0) ? displayText.Substring(0, len) + "…" : "";
        }

        ImGui.SetCursorScreenPos(pos + new SVector2(3, 1));
        ImGui.TextColored(ImGui.ColorConvertU32ToFloat4(options.TextColor), displayText);
        ImGui.SetCursorScreenPos(pos);
        ImGui.InvisibleButton($"##{uniqueID}display", new SVector2(width, height)); // Covers the full area

        if (ImGui.IsItemHovered() && options.Tooltip != null) Tooltip.Draw(options.Tooltip);

    }

}

