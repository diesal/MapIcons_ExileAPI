
using ImGuiNET;
using SVector2 = System.Numerics.Vector2;
using SVector3 = System.Numerics.Vector3;
using SVector4 = System.Numerics.Vector4;
using Rectangle = System.Drawing.Rectangle;

//+-----------------------------------------------------------+---------+----------------------------------------------------------------+
//| (LeftMargin + leftPadding , TopMargin + TopPadding)       |         |  (righttMargin + rightPadding , TopMargin + TopPadding)        |
//+-----------------------------------------------------------+---------+----------------------------------------------------------------+
//|                                                           | Content |                                                                |
//+-----------------------------------------------------------+---------+----------------------------------------------------------------+
//| (LeftMargin + leftPadding , BottomMargin + BottomPadding) |         |  (righttMargin + rightPadding , BottomMargin + BottomPadding)  |
//+-----------------------------------------------------------+---------+----------------------------------------------------------------+


namespace DieselTools_ExileAPI
{
    public static class Panel {

        public class Options
        {
            /// <summary> X=Top, Y=Right, Z=Bottom, W=Left, right padding has to be manually implemented by you </summary>
            public SVector4 Padding { get; set; } = new SVector4(4, 4, 4, 4); // top, right, bottom, left
            /// <summary> null = auto-size to content, &lt;0 = fill space - value, &gt;0 = fixed pixel width </summary>
            public int? Width { get; set; }
            /// <summary> null = auto-size to content, &lt;0 = fill space - value, &gt;0 = fixed pixel width </summary>
            public int? Height { get; set; }
            public uint Color { get; set; } = Colors.Panel;
            public uint InnerGlowColor { get; set; } = Colors.PanelInnerGlow;


            public bool Debug { get; set; } = false;
            /// <summary> calculated size of the entire panel including margins and padding after End() is called </summary>
        }

        // Debugging
        private static uint red = Palettes.GetMaterialColor("Red A700",50);
        private static uint yellow = Palettes.GetMaterialColor("Yellow A700", 50);
        private static uint orange = Palettes.GetMaterialColor("Orange A700", 50);

        private static uint green = Palettes.GetMaterialColor("Green A700", 50);
        private static uint blue = Palettes.GetMaterialColor("Light Blue A700", 50);
        private static uint purple = Palettes.GetMaterialColor("Purple A700", 50);
        private static void DummyCell(SVector2 cell, SVector2 size, bool debug) {
            if (debug) {
                var pos = ImGui.GetCursorScreenPos();
                var drawList = ImGui.GetWindowDrawList();
                if (cell.X == 1) {
                    if (cell.Y == 0) {// 1, 0 
                        drawList.AddRectFilled(pos, new SVector2(pos.X + size.X, pos.Y + size.Y), red, 0f);
                    }
                    else if (cell.Y == 1) { // 1, 1
                        drawList.AddRectFilled(pos, new SVector2(pos.X + size.X, pos.Y + size.Y), orange, 0f);
                    }
                    else {// 1, 2
                        drawList.AddRectFilled(pos, new SVector2(pos.X + size.X, pos.Y + size.Y), yellow, 0f);
                    }
                }
                else {
                    if (cell.Y == 0) {// !1,0
                        drawList.AddRectFilled(pos, new SVector2(pos.X + size.X, pos.Y + size.Y), blue, 0f);
                    }
                    else if (cell.Y == 1) {// !1, 1
                        drawList.AddRectFilled(pos, new SVector2(pos.X + size.X, pos.Y + size.Y), green, 0f);
                    }
                    else { // !1,2 
                        drawList.AddRectFilled(pos, new SVector2(pos.X + size.X, pos.Y + size.Y), purple, 0f);
                    }
                }

            }

            ImGui.Dummy(size);
        }


        private class PanelState
        {
            public Rectangle PanelSize;
            public SVector2 PanelStart;
            public Options Options;
            public SVector2? CellEnd;
        }
        private static readonly Dictionary<string, PanelState> panelStates = new();

        private static bool FUCKIMGUI_Begin(string uniqueID, Options options) {
           // PanelState panelState;
           // if (!panelStates.TryGetValue(uniqueID, out panelState)) {
           //     panelState = new();
           //     panelStates[uniqueID] = panelState;
           // }
           // panelState.Options = options;
           //
           // var avail = ImGui.GetContentRegionAvail();
           // float contentWidth = options.Width == null
           //     ? avail.X - (options.Margin.W + options.Padding.W) - (options.Margin.Y + options.Padding.Y)
           //     : (options.Width == -1 ? 0: options.Width.Value);
           //
           // float contentHeight = options.Height == null
           //     ? avail.Y - options.Margin.X - options.Padding.X - options.Margin.Z - options.Padding.Z
           //     : (options.Height == -1 ? 0 : options.Height.Value);
           //
           //
           // var childFlags = ImGuiChildFlags.None;
           // SVector2 childSize = new(0, 0);
           // if (options.Width.HasValue) {
           //     if (options.Width.Value < 0) {
           //         childFlags |= ImGuiChildFlags.AutoResizeX;
           //         childSize.X = 0;
           //     }
           //     else childSize.X = options.Width.Value - options.Padding.W - options.Padding.Y;
           // }
           // else childSize.X = avail.X;
           // if (options.Height.HasValue) {
           //     if (options.Height.Value < 0) {
           //         childFlags |= ImGuiChildFlags.AutoResizeY;
           //         childSize.Y = 0;
           //     }
           //     else childSize.Y = options.Height.Value - options.Padding.X - options.Padding.Z;
           // }
           // else childSize.Y = avail.Y;
           //
           // var startPos = ImGui.GetCursorScreenPos();
           // ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new SVector2(0, 0));
           // // Table: 3 columns (left, content, right)
           // ImGui.BeginTable(uniqueID, 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.NoPadOuterX | ImGuiTableFlags.NoPadInnerX);
           // var drawList = ImGui.GetWindowDrawList();
           //
           // panelState.PanelStart = ImGui.GetCursorScreenPos();
           // // Top row: margin + padding
           // ImGui.TableNextRow();
           // ImGui.TableSetColumnIndex(0);
           // DummyCell(new(0, 0), new(options.Margin.W + options.Padding.W, options.Margin.X + options.Padding.X), options.Debug );
           //
           // ImGui.TableSetColumnIndex(1);
           // DummyCell(new(0, 1), new(contentWidth, options.Margin.X + options.Padding.X), options.Debug);
           //
           // ImGui.TableSetColumnIndex(2);
           // DummyCell(new(0, 2), new(options.Margin.Y + options.Padding.Y, options.Margin.X + options.Padding.X), options.Debug);
           //
           // // Content row
           // ImGui.TableNextRow();
           // ImGui.TableSetColumnIndex(0);
           // DummyCell(new(1, 0), new(options.Margin.W + options.Padding.W, contentHeight), options.Debug);
           //
           // // content cell
           // ImGui.TableSetColumnIndex(1);
           //
           // //// Draw custom background behind content cell
           // //var cellStart = startPos + new SVector2(options.Margin.W , options.Margin.X );
           // ////var cellStart = ImGui.GetCursorScreenPos();
           // //panelState.CellEnd = panelState.CellEnd ?? cellStart + new SVector2(contentWidth, contentHeight);
           // //var drawList = ImGui.GetWindowDrawList();
           // ////var bgMin = new SVector2(cellStart.X - options.Padding.W, cellStart.Y - options.Padding.X);
           // //var bgMin = cellStart;
           // //var bgMax = new SVector2(
           // //    panelState.CellEnd.Value.X + options.Padding.Y,
           // //    panelState.CellEnd.Value.Y + options.Padding.Z
           // //);
           // //drawList.AddRectFilled(bgMin, bgMax, options.Color);
           // //drawList.AddRect(bgMin, bgMax, options.InnerGlowColor);
           //
           //
           // ImGui.PushStyleColor(ImGuiCol.ChildBg, 0);
           // ImGui.BeginChild(uniqueID, childSize, childFlags, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
           //
           // // Draw custom background
           // var windowPos = ImGui.GetWindowPos();
           // var windowEndPos = windowPos + ImGui.GetWindowSize();
           // var bgMin = new SVector2(windowPos.X - options.Padding.W, windowPos.Y - options.Padding.X);// windowPos;
           // var bgMax = new SVector2(windowEndPos.X + options.Padding.Y, windowEndPos.Y + options.Padding.Z);
           //
           // //var bgMin = new SVector2(windowPos.X, windowPos.Y );// windowPos;
           // //var bgMax = new SVector2(windowEndPos.X , windowEndPos.Y );
           //
           // drawList.AddRectFilled(bgMin, bgMax, options.Color);
           // drawList.AddRect(bgMin, bgMax, options.InnerGlowColor);
           // Rectangle p = new((int)bgMin.X, (int)bgMin.Y, (int)(bgMax.X - bgMin.X), (int)(bgMax.Y - bgMin.Y));
           //
           return true;
        }
        private static void FUCKIMGUI_End(string uniqueID) {
            // var panelState = panelStates[uniqueID];
            // var options = panelState.Options;
            // 
            // ImGui.EndChild();
            // ImGui.PopStyleColor();
            // 
            // // Content row: right margin + right padding
            // ImGui.TableSetColumnIndex(2);
            // DummyCell(new(1, 2), new SVector2(options.Margin.Y + options.Padding.Y, 0), options.Debug);
            // 
            // // Bottom row: margin + padding
            // ImGui.TableNextRow();
            // ImGui.TableSetColumnIndex(0);
            // DummyCell(new(2, 0), new SVector2(options.Margin.W + options.Padding.W, options.Margin.Z + options.Padding.Z), options.Debug);
            // 
            // ImGui.TableSetColumnIndex(1);
            // DummyCell(new(2, 1), new SVector2(0, options.Margin.Z + options.Padding.Z), options.Debug);
            // 
            // ImGui.TableSetColumnIndex(2);
            // panelState.CellEnd = ImGui.GetCursorScreenPos();
            // DummyCell(new(2, 2), new SVector2(options.Margin.Y + options.Padding.Y, options.Margin.Z + options.Padding.Z), options.Debug);
            // 
            // var panelSize = ImGui.GetCursorScreenPos() - panelState.PanelStart;
            // panelSize.X += options.Margin.Y + options.Padding.Y;
            // // DBugger.LogMessage($"GetCursorScreenPos: {ImGui.GetCursorScreenPos()} PanelStart:{panelState.PanelStart} panelSize {panelSize} )");    
            // ImGui.EndTable();
            // ImGui.PopStyleVar();
            // 
            // //options.CalculatedSize = panelSize;
        }

        public static bool Begin(string uniqueID, Options options) {
            if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
            if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");

            PanelState panelState;
            if (!panelStates.TryGetValue(uniqueID, out panelState)) {
                panelState = new();
                panelStates[uniqueID] = panelState;
            }
            panelState.Options = options;

            var avail = ImGui.GetContentRegionAvail();
            var flags = ImGuiChildFlags.None;

            // get panel size
            SVector2 panelSize = new(0, 0);
            if (options.Width.HasValue) {
                if (options.Width.Value <= 0) { // fill space - value
                    panelSize.X = avail.X + options.Width.Value; 
                }
                else panelSize.X = options.Width.Value;
            }
            else flags |= ImGuiChildFlags.AutoResizeX; // auto-size

            if (options.Height.HasValue) {
                if (options.Height.Value <= 0) { // fill space - value 
                    panelSize.Y = avail.Y + options.Height.Value;                    
                }
                else panelSize.Y = options.Height.Value;
            }
            else flags |= ImGuiChildFlags.AutoResizeY; // auto-size

            // draw child
            ImGui.PushStyleColor(ImGuiCol.ChildBg, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,10);
            ImGui.BeginChild(uniqueID, panelSize, flags, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            // Draw custom background
            var childPos = ImGui.GetWindowPos();
            var childSize = ImGui.GetWindowSize();
            var bgMin = childPos;
            var bgMax = new SVector2(childPos.X + childSize.X, childPos.Y + childSize.Y);

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(bgMin, bgMax, options.Color);
            drawList.AddRect(bgMin, bgMax, options.InnerGlowColor);

            // Padding top
            if (options.Padding.X > 0) ImGui.Dummy(new SVector2(0, options.Padding.X));
            // Indent for left padding
            if (options.Padding.W > 0) ImGui.Indent(options.Padding.W);

            return true;
        }
        public static void End(string uniqueID) {
            var panelState = panelStates[uniqueID];
            var options = panelState.Options;

            // Padding bottom
            if (options.Padding.Z > 0) ImGui.Dummy(new SVector2(0, options.Padding.Z));
            // Unindent left padding
            if (options.Padding.W > 0) ImGui.Unindent(options.Padding.W);
            ImGui.EndChild();
            ImGui.PopStyleColor();
        }
    }
}
