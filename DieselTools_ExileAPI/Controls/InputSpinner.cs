using ImGuiNET;
using SVector2 = System.Numerics.Vector2;

namespace DieselTools_ExileAPI;




public static class InputSpinner {
    public class Options {
        public SVector2 PositionOffset { get; set; } = new SVector2(0, 0);
        public int? Width { get; set; }
        public int? Height { get; set; }
        public float Min { get; set; } = 0f;
        public float Max { get; set; } = 100f;
        public float Step { get; set; } = 1f;
        public float? ShiftStep { get; set; } 
        public uint BackgroundColor { get; set; } = Colors.Input;
        public uint BorderColor { get; set; } = Colors.Black;
        public uint OuterBorderColor { get; set; } = Colors.ControlOuterGlow;
        public Tooltip.Options? Tooltip { get; set; } = null;
        public ImGuiInputTextFlags InputTextFlags { get; set; } = ImGuiInputTextFlags.None;

    }

    private static string FormatDisplayValue(float value) {
        string formatted = value.ToString("F1");
        if (formatted.EndsWith(".0")) formatted = formatted.Substring(0, formatted.Length - 2);

        return formatted;
    }


    private static bool InternalDraw(string uniqueID, ref float value,bool isInt, Options options) {
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
        string buf = isInt ? ((int)value).ToString() : FormatDisplayValue(value);
        // Draw input box
        ImGui.PushStyleColor(ImGuiCol.FrameBg, 0x00000000);
        ImGui.PushStyleColor(ImGuiCol.Border, 0x00000000);
        ImGui.SetCursorScreenPos(pos + new SVector2(3, 1));
        ImGui.PushItemWidth(width - 8);
        if (ImGui.InputText($"##{uniqueID}input", ref buf, 4, ImGuiInputTextFlags.CharsDecimal | options.InputTextFlags))
            changed = true;
        // Handle mouse wheel input for changing value
        if (ImGui.IsItemHovered()) {
            float wheel = ImGui.GetIO().MouseWheel;
            if (wheel != 0) {
                if (isInt) {
                    int newValue;
                    if (!int.TryParse(buf, out newValue)) newValue = (int)value;
                    newValue += (int)(Math.Sign(wheel) * (ImGui.GetIO().KeyShift && options.ShiftStep.HasValue ? options.ShiftStep.Value : options.Step));
                    newValue = (int)Math.Clamp(newValue, options.Min, options.Max);
                    buf = newValue.ToString();
                    changed = true;
                }
                else {
                    float newValue;
                    if (!float.TryParse(buf, out newValue)) newValue = value;
                    newValue += Math.Sign(wheel) * (ImGui.GetIO().KeyShift && options.ShiftStep.HasValue ? options.ShiftStep.Value : options.Step);
                    newValue = Math.Clamp(newValue, options.Min, options.Max);
                    buf = newValue.ToString();
                    changed = true;
                }
            }
            if (options.Tooltip != null) Tooltip.Draw(options.Tooltip);
        }
        // 
        if (changed) {
            if (isInt) {
                // Remove any decimal point and everything after it
                int dotIndex = buf.IndexOf('.');
                if (dotIndex >= 0) buf = buf.Substring(0, dotIndex);
                if (int.TryParse(buf, out int newVal))
                    value = Math.Clamp(newVal, options.Min, options.Max);
            }
            else {
                if (float.TryParse(buf, out float newVal))
                    value = Math.Clamp(newVal, options.Min, options.Max);
            }
        }

        ImGui.PopItemWidth();
        ImGui.PopStyleColor(2);
        return changed;
    }

    public static bool Draw(string uniqueID, ref float value, Options options) {
        return InternalDraw(uniqueID, ref value, false, options);
    }
    public static bool Draw(string uniqueID, ref int value, Options options) {
        float float_value = value; // Convert int to float for processing
        var changed = InternalDraw(uniqueID, ref float_value, true, options);
        if (changed) {
            value = (int)float_value; // Direct cast, no rounding needed
        }
        return changed;
    }


}

