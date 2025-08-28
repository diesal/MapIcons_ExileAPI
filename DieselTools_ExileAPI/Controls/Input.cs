using ImGuiNET;
using SVector2 = System.Numerics.Vector2;

namespace DieselTools_ExileAPI;




public static class Input {
    public class Options {
        public SVector2 PositionOffset { get; set; } = new SVector2(0, 0);
        public int? Width { get; set; }
        public int? Height { get; set; }
        public uint BackgroundColor { get; set; } = Colors.Input;
        public uint BorderColor { get; set; } = Colors.Black;
        public uint OuterBorderColor { get; set; } = Colors.ControlOuterGlow;
        public ImGuiInputTextFlags InputTextFlags { get; set; } = ImGuiInputTextFlags.None;
        public Tooltip.Options? Tooltip { get; set; } = null;
    }

    public static bool Draw(string uniqueID, ref string value, Options options) {
        if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
        if (options == null) options = new Options(); // safety?
        // position and size
        var pos = ImGui.GetCursorScreenPos() + options.PositionOffset;
        var width = options.Width ?? ImGui.GetContentRegionAvail().X;
        var height = options.Height ?? ImGui.GetFrameHeight();
        // custom theme 
        var drawList = ImGui.GetWindowDrawList();
        drawList.AddRectFilled(pos, pos + new SVector2(width, height), options.BackgroundColor);
        drawList.AddRect(pos, pos + new SVector2(width, height), options.BorderColor);
        drawList.AddRect(pos - new SVector2(1, 1), pos + new SVector2(width + 1, height + 1), options.OuterBorderColor);

        bool changed = false;
        ImGui.SetCursorScreenPos(pos + new SVector2(3, 1));
        ImGui.PushItemWidth(width - 8);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, 0x00000000);
        ImGui.PushStyleColor(ImGuiCol.Border, 0x00000000);
        changed = ImGui.InputText($"##{uniqueID}input", ref value, 128, options.InputTextFlags);
        ImGui.PopStyleColor(2);
        ImGui.PopItemWidth();

        if (ImGui.IsItemHovered() && options.Tooltip != null)
            Tooltip.Draw(options.Tooltip);

        return changed;
    }

}

