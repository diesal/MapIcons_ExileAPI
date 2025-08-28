
using ExileCore.PoEMemory.Components;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;



namespace DieselTools_ExileAPI
{
    public static class CollapsingPanel {

        public class Options
        {
            public string? Label { get; set; }
            /// <summary> X=Top, Y=Right, Z=Bottom, W=Left, Default:(4, 4, 4, 4) right padding has to be manually implemented by you </summary>
            public SVector4 Padding { get; set; } = new SVector4(4, 4, 4, 4); 
            public int HeaderHeight { get; set; } = 22;
            /// <summary> &gt;0 = fixed pixel width, any other value will result in the panel handling it </summary>
            public int? HeaderWidth { get; set; }
            public int HeaderSpacing { get; set; } = 1;

            /// <summary> null = auto-size to content, &lt;0 = fill space - value, &gt;0 = fixed pixel width </summary>
            public int? Width { get; set; }
            /// <summary> null = auto-size to content, &lt;0 = fill space - value, &gt;0 = fixed pixel width </summary>
            public int? Height { get; set; }

            public uint Color { get; set; } = Colors.Panel;
            public uint HeaderColor { get; set; } = Colors.Button;
            public uint InnerGlowColor { get; set; } = Colors.PanelInnerGlow;


            public bool Debug { get; set; } = false;
            public SVector2 CalculatedSize { get; set; }
        }

        private class PanelState
        {
            public Panel.Options PanelOptions;
            public Options Options;
        }
        private static readonly Dictionary<string, PanelState> panelStates = new();

        public static bool Begin(string uniqueID,ref bool collapsed, Options options) {
            if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
            if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");

            PanelState panelState;
            if (!panelStates.TryGetValue(uniqueID, out panelState)) {
                panelState = new();
                panelStates[uniqueID] = panelState;
            }
            panelState.Options = options;

            var panelOtions = new Panel.Options {
                Padding = options.Padding,
                Width = options.Width,
                Height = options.Height,
                Debug = options.Debug,
                Color = options.Color,
                InnerGlowColor = options.InnerGlowColor,
            };
            //panelState.PanelOptions = panelOtions;

            // layout 
            var drawList = ImGui.GetWindowDrawList();
            var startingPos = ImGui.GetCursorScreenPos();
            var availWidth = ImGui.GetContentRegionAvail().X;
            // button layout
            var buttonPos = startingPos;
            var buttonWidth = ImGui.CalcTextSize(options.Label).X + 8;
            if (options.HeaderWidth != null && options.HeaderWidth > 0) {
                buttonWidth = options.HeaderWidth.Value;
            }
            else {
                if (options.Width != null) {
                    if (options.Width <= 0) buttonWidth = availWidth + options.Width.Value; // fill space - value
                    else buttonWidth = options.Width.Value; // fixed pixel width
                }
            }

            var buttonHeight = options.HeaderHeight;
            // draw header
            drawList.AddRectFilled(buttonPos, buttonPos + new SVector2(buttonWidth, buttonHeight), options.HeaderColor);
            drawList.AddRect(buttonPos , buttonPos + new SVector2(buttonWidth , buttonHeight ), options.InnerGlowColor);
            if (!string.IsNullOrEmpty(options.Label)) {
                var textSize = ImGui.CalcTextSize(options.Label);
                var textPos = buttonPos + new SVector2( 4, (float)Math.Ceiling((buttonHeight - textSize.Y) / 2) - 2 );
                drawList.AddText(textPos, Colors.ControlText, options.Label);
            }
            // button
            ImGui.SetCursorScreenPos(buttonPos);
            bool toggled = ImGui.InvisibleButton($"{uniqueID}_headerbutton", new SVector2(buttonWidth, buttonHeight));
            if (toggled) collapsed = !collapsed;

            if (collapsed) {
                return false;
            }

            // position panel below header
            var panelPos = ImGui.GetCursorScreenPos();
            panelPos.Y += options.HeaderSpacing;

            ImGui.SetCursorScreenPos(panelPos);
            Panel.Begin(uniqueID, panelOtions);
            // Draw header at the top, inside the margin space
            ImGui.TableSetColumnIndex(1); // Content column, first row
            ImGui.SetCursorPosY(ImGui.GetCursorPosY()); // Already at correct Y after margin dummy

            // Optionally, draw a separator or custom background here
            return true;
        }


        public static void End(string uniqueID) {
            Panel.End(uniqueID);

        }
    }
}
