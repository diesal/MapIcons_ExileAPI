using ExileCore.PoEMemory.Components;
using ImGuiNET;
using System.Drawing.Design;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;

namespace DieselTools_ExileAPI;


public enum ToolbarOrientation
{
    Horizontal,
    Vertical
}

public static class FloatingToolbar {

    private static Dictionary<string, SVector2> _lastPositions = new();


    public abstract class Tool
    {
        public abstract SVector2 GetSize();
        public abstract void Draw(string uniqueID, SVector2 position, SVector2 forcedSize);
    }
    public class Button : Tool
    {
        public string Label { get; set; } = "";
        public int? Width { get; set; }
        public int? Height { get; set; }
        public Action? OnClick { get; set; }
        public uint? Color { get; set; }
        public Func<bool>? GetChecked { get; set; }
        public Action<bool>? SetChecked { get; set; }

        public override SVector2 GetSize() {
            var textSize = ImGui.CalcTextSize(Label);
            int width = Width ?? (int)Math.Ceiling(textSize.X + 10);
            int height = Height ?? (int)Math.Ceiling(textSize.Y + 4);
            return new SVector2(width, height);
        }

        public override void Draw(string uniqueID, SVector2 position, SVector2 forcedSize) {
            ImGui.SetCursorScreenPos(position);
            var buttonOptions = new DieselTools_ExileAPI.Button.Options {
                Label = Label,
                Width = (int)forcedSize.X,
                Height = (int)forcedSize.Y,
            };
            if (Color != null) buttonOptions.Color = Color.Value;

            if (GetChecked != null && SetChecked != null) {
                bool checkedState = GetChecked();
                if (DieselTools_ExileAPI.Button.Draw(uniqueID, ref checkedState, buttonOptions)) {
                    SetChecked(checkedState);
                    OnClick?.Invoke();
                }
            }
            else {
                if (DieselTools_ExileAPI.Button.Draw(uniqueID, buttonOptions)) {
                    OnClick?.Invoke();
                }
            }
        }
    }
    public class Label : Tool
    {
        public string Text { get; set; } = "";
        public uint BackgroundColor { get; set; } = Colors.Black;
        public uint TextColor { get; set; } = Colors.ControlText;
        public int? Width { get; set; }
        public int? Height { get; set; }

        public override SVector2 GetSize() {
            var textSize = ImGui.CalcTextSize(Text);
            int width = Width ?? (int)Math.Ceiling(textSize.X + 4); // padding
            int height = Height ?? (int)Math.Ceiling(textSize.Y + 4);
            return new SVector2(width, height);
        }

        public override void Draw(string uniqueID, SVector2 position, SVector2 forcedSize) {
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(position, position + forcedSize, BackgroundColor);
            var textSize = ImGui.CalcTextSize(Text);
            var Textpos = position + new SVector2(
                (float)Math.Round((forcedSize.X - textSize.X) / 2),
                (float)Math.Round((forcedSize.Y - textSize.Y) / 2) - 2
            );
            drawList.AddText(Textpos, TextColor, Text);
        }
    }

    public class Options {
        public uint BackgroundColor { get; set; } = Colors.Black;
        public uint BorderColor { get; set; } = Colors.Black;
        public uint LabelTextColor { get; set; } = Colors.ControlText;
        /// <summary> X=Top, Y=Right, Z=Bottom, W=Left </summary>
        public SVector4 Padding { get; set; } = new SVector4(2, 2, 2, 2);
        public int ButtonSpacing { get; set; } = 1;
        public bool Movable { get; set; } = true;
        public SVector2? ResetPosition { get; set; }
        public ToolbarOrientation Orientation { get; set; } = ToolbarOrientation.Horizontal;
        public List<Tool> Tools { get; set; } = new();
    }
    
    public static void Draw(string uniqueID, Options options) {
        InternalDraw(uniqueID, options);
    }
    public static void Draw(string uniqueID, List<Tool> tools) {
        var options = new Options {
            Tools = tools,
        };
        InternalDraw(uniqueID, options);
    }
    private static void InternalDraw(string uniqueID, Options options) {
        if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
        if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");

        // Calculate button sizes
        List<SVector2> toolSizes = new();
        float ToolsTotalWidth = 0;
        float ToolsTotalHeight = 0;
        float maxToolWidth = 0;
        float maxToolHeight = 0;
        for (int i = 0; i < options.Tools.Count; i++) {
            var tool = options.Tools[i];
            var size = tool.GetSize();
            int width = (int)size.X;
            int height = (int)size.Y;

            toolSizes.Add(new SVector2(width, height));
            if (options.Orientation == ToolbarOrientation.Horizontal) {
                ToolsTotalWidth += width;
                if (i < options.Tools.Count - 1) ToolsTotalWidth += options.ButtonSpacing;
                maxToolHeight = Math.Max(maxToolHeight, height);
            }
            else {
                ToolsTotalHeight += height;
                if (i < options.Tools.Count - 1) ToolsTotalHeight += options.ButtonSpacing;
                maxToolWidth = Math.Max(maxToolWidth, width);
            }
        }

        // Calculate toolbar size
        float totalWidth = options.Padding.W + options.Padding.Y;
        float totalHeight = options.Padding.X + options.Padding.Z;
        if (options.Orientation == ToolbarOrientation.Horizontal) {
            totalWidth += ToolsTotalWidth;
            totalHeight += maxToolHeight;
        }
        else {
            totalWidth += maxToolWidth;
            totalHeight += ToolsTotalHeight;
        }

        if (options.ResetPosition.HasValue) {
            ImGui.SetNextWindowPos(options.ResetPosition.Value, ImGuiCond.Always);
            options.ResetPosition = null;
        }
        ImGui.SetNextWindowSize(new SVector2(totalWidth, totalHeight), ImGuiCond.Always);

        var flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize;
        if (!options.Movable) flags |= ImGuiWindowFlags.NoMove;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new SVector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new SVector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleColor(ImGuiCol.Border, options.BorderColor);
        ImGui.PushStyleColor(ImGuiCol.WindowBg, ColorTools.RGBA2Uint(0,0,0,0));
        ImGui.Begin($"##{uniqueID}Toolbar", flags);

        var drawList = ImGui.GetWindowDrawList();
        SVector2 winPos = ImGui.GetWindowPos();
        SVector2 winSize = ImGui.GetWindowSize();

        // Draw background and border
        drawList.AddRectFilled(winPos, winPos + winSize, options.BackgroundColor);
        drawList.AddRect(winPos, winPos + winSize, options.BorderColor);

        // Draw tools
        SVector2 tool_pos = winPos + new SVector2(options.Padding.W, options.Padding.X);
        for (int i = 0; i < options.Tools.Count; i++) {

            var tool = options.Tools[i];
            var tool_size = tool.GetSize();

            // Force uniform size
            if (options.Orientation == ToolbarOrientation.Horizontal)
                tool_size.Y = maxToolHeight;
            else
                tool_size.X = maxToolWidth;

            tool.Draw($"##{uniqueID}_item{i}", tool_pos, tool_size);

            if (options.Orientation == ToolbarOrientation.Horizontal)
                tool_pos.X += tool_size.X + options.ButtonSpacing;
            else
                tool_pos.Y += tool_size.Y + options.ButtonSpacing;
        }

        ImGui.End();
        ImGui.PopStyleColor(2); // Pop WindowBg, Border
        ImGui.PopStyleVar(4);   // Pop WindowRounding, WindowBorderSize, FramePadding, WindowPadding

        //ImGui.Begin("DBG", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDecoration);
        //string dbg = $"maxH:{maxButtonHeight}\ntotH:{totalHeight}\nwinY:{winSize.Y}\npadT:{options.Padding.X}\npadB:{options.Padding.Z}";
        //ImGui.InputTextMultiline("##dbgcopy", ref dbg, 256, new SVector2(300, 60), ImGuiInputTextFlags.ReadOnly);
        //ImGui.End();

    }
}

