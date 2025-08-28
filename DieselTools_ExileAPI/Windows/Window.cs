using ImGuiNET;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;

namespace DieselTools_ExileAPI;

public static class Window {
    private static Dictionary<string, SVector2> _lastPositions = new();


    public class Options
    {
        public string? Title;
        public int TitleBarHeight { get; set; } = 20; // Height of the title bar
        public bool ShowCloseButton { get; set; } = true; // Whether to show a close button
        public bool ShowMinimizeButton { get; set; } = true; // Whether to show a minimize button
        public uint BorderColor { get; set; } = Colors.Border; // Default border color
        public uint BackgroundColor { get; set; } = Colors.Black; // Default background
        public uint GripColor { get; set; } = Colors.Button; // Color of the resize grip
        public uint ScrollbarColor { get; set; } = ColorTools.RGBA2Uint(0,0,0,20); // Color of the scrollbar
        public uint ScrollbarGripColor { get; set; } = Colors.Button; // Color of the scrollbar grab
        public uint TitleTextColor { get; set; } = Colors.ControlText; // Default text color

        public uint CloseButtonColor { get; set; } = ColorTools.RGBA2Uint(70, 12, 12); // Color of the close button
        public uint CloseButtonHoverColor { get; set; } = ColorTools.RGBA2Uint(130, 23, 23); // Color of the close button on hover
        public uint MinimizeButtonColor { get; set; } = ColorTools.RGBA2Uint(69, 32, 13); // (69,32,13) (87,41,15) (104,49,18) ? (130, 61, 23)
        public uint MinimizeButtonMinimisedColor { get; set; } = ColorTools.RGBA2Uint(173, 81, 31); // Color of the minimize button
        public uint MinimizeButtonHoverColor { get; set; } = ColorTools.RGBA2Uint(173, 81, 31); // Color of the minimize button on hover

        public bool Movable { get; set; } = true; // Whether the window can be moved
        public bool Resizable { get; set; } = false; // Whether the window can be resized

        public SVector2? ResetPosition { get; set; }
        public SVector2? ResetSize { get; set; }

        public float? LockWidth { get; set; }   // If set, window width is locked
        public float? LockHeight { get; set; }  // If set, window height is locked
        public float MinWidth { get; set; } = 100;
        public float MaxWidth { get; set; } = 5000;
        public float MinHeight { get; set; } = 50;
        public float MaxHeight { get; set; } = 5000;
    }

    private static void PushStrippedStyles(Options options) {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new SVector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new SVector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new SVector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, 0.0f);

        ImGui.PushStyleColor(ImGuiCol.ScrollbarBg,options.ScrollbarColor);
        ImGui.PushStyleColor(ImGuiCol.ScrollbarGrab, options.ScrollbarGripColor);
        ImGui.PushStyleColor(ImGuiCol.WindowBg, ColorTools.RGBA2Uint(0, 0, 0, 0));
        ImGui.PushStyleColor(ImGuiCol.FrameBg, ColorTools.RGBA2Uint(0, 0, 0, 0));
        ImGui.PushStyleColor(ImGuiCol.Border, new SVector4(0, 0, 0, 0));
    }
    private static void PopStrippedStyles() {
        ImGui.PopStyleVar(6);
        ImGui.PopStyleColor(5); 
    }


    private static Dictionary<string, bool> _minimizedStates = new();


    private class DrawResult
    {
        public bool DrawContent;
        public bool IsClosed;
    }

    public static bool Begin(string uniqueID, Options options) {
        var drawResult = InternalBegin(uniqueID, options);

        return drawResult.DrawContent;
    }

    public static bool Begin(string uniqueID, ref bool isOpen, Options options) {
        if (!isOpen) return false;
        var drawResult = InternalBegin(uniqueID, options);
        if (drawResult.IsClosed) isOpen = false;
        return drawResult.DrawContent;
    }

    private static DrawResult InternalBegin(string uniqueID, Options options) {
        if (string.IsNullOrEmpty(uniqueID)) throw new ArgumentException("uniqueID cannot be null or empty", nameof(uniqueID));
        if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");

        PushStrippedStyles(options);

        // minimise flag
        bool isMinimized;
        if (!_minimizedStates.TryGetValue(uniqueID, out isMinimized)) {
            isMinimized = false;
            _minimizedStates[uniqueID] = false;
        }

        // Manage window flags
        var flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDocking;
        if (!options.Movable) flags |= ImGuiWindowFlags.NoMove;
        if (!options.Resizable || isMinimized) flags |= ImGuiWindowFlags.NoResize;

        // Handle reset position/size
        if (options.ResetPosition.HasValue) {
            ImGui.SetNextWindowPos(options.ResetPosition.Value, ImGuiCond.Always);
            options.ResetPosition = null; // Clear after use
        }
        if (options.ResetSize.HasValue) {
            ImGui.SetNextWindowSize(options.ResetSize.Value, ImGuiCond.Always);
            options.ResetSize = null; // Clear after use
        }

        // Set window size constraints(lock axis if specified, else use min/ max)
        float minW = options.MinWidth;
        float maxW = options.MaxWidth;
        float minH = options.MinHeight;
        float maxH = options.MaxHeight;
        if (options.LockWidth.HasValue)
            minW = maxW = options.LockWidth.Value;
        if (options.LockHeight.HasValue)
            minH = maxH = options.LockHeight.Value;

        ImGui.SetNextWindowSizeConstraints(
            new SVector2(minW, minH),
            new SVector2(maxW, maxH)
        );


        // Begin window
        bool opened = ImGui.Begin($"##{uniqueID}Window", flags);
        if (!opened) {
            PopStrippedStyles();
            return new DrawResult { DrawContent = false, IsClosed = true}; 
         }

        var drawList = ImGui.GetWindowDrawList();
        SVector2 winPos = ImGui.GetWindowPos();
        SVector2 winSize = ImGui.GetWindowSize();
        SVector2 winPosEnd = isMinimized ? winPos + new SVector2(winSize.X, options.TitleBarHeight) : winPos + winSize;
        // Draw background and border
        drawList.AddRectFilled(winPos, winPosEnd, options.BackgroundColor);
        drawList.AddRect(winPos, winPosEnd, options.BorderColor);
        // Title text
        if (!string.IsNullOrEmpty(options.Title)) {
            drawList.AddText(winPos + new SVector2(4, 1), options.TitleTextColor, options.Title);
        }
        if (options.Resizable && !isMinimized) DrawGrip(winPos, winSize, options.GripColor);


        SVector2 btnSize = new(options.TitleBarHeight - 4, options.TitleBarHeight - 4);
        SVector2 btnPos = winPos + new SVector2(winSize.X - btnSize.X - 2, 2);
        // Draw custom close button if requested
        if (options.ShowCloseButton) {
            ImGui.SetCursorScreenPos(btnPos);
            if (Button.Draw($"##{uniqueID}_close", new Button.Options { Width = (int)btnSize.X, Height = (int)btnSize.Y, Color= options.CloseButtonColor, HoveredColor = options.CloseButtonHoverColor, Tooltip = Tooltip.BasicOptions("Close Window")  })) {
                ImGui.End();
                PopStrippedStyles();
                return new DrawResult { DrawContent = false, IsClosed = true };
            }
        }
        // Draw minimise button
        if (options.ShowMinimizeButton) {
            btnPos.X -= (btnSize.X + 2);
            ImGui.SetCursorScreenPos(btnPos);
            if (Button.Draw($"##{uniqueID}_minimize", new Button.Options { Width = (int)btnSize.X, Height = (int)btnSize.Y, Color = isMinimized ? options.MinimizeButtonMinimisedColor : options.MinimizeButtonColor, HoveredColor = options.MinimizeButtonMinimisedColor, Tooltip = Tooltip.BasicOptions(isMinimized ? "Restore Window" : "Minimize Window") })) {
                _minimizedStates[uniqueID] = !_minimizedStates[uniqueID];
            }
            if (_minimizedStates[uniqueID]) {
                ImGui.End();
                PopStrippedStyles();
                return new DrawResult { DrawContent = false, IsClosed = false };
            }
        }


        ImGui.SetCursorScreenPos(new(winPos.X, winPos.Y + options.TitleBarHeight));

        return new DrawResult { DrawContent = true, IsClosed = false };
    }
    public static void End() {
        ImGui.End();
        PopStrippedStyles();
    }


    private static void DrawGrip(SVector2 winPos, SVector2 winSize, uint gripColor) {
        var drawList = ImGui.GetWindowDrawList();
        float gripSize = 16f;
        SVector2 gripMin = winPos + new SVector2(winSize.X - gripSize, winSize.Y - gripSize);
        SVector2 gripMax = winPos + new SVector2(winSize.X, winSize.Y);
        SVector2 gripTriangle1 = gripMin + new SVector2(gripSize, 0);
        SVector2 gripTriangle2 = gripMin + new SVector2(0, gripSize);
        SVector2 gripTriangle3 = gripMax;
        drawList.AddTriangleFilled(gripTriangle1, gripTriangle2, gripTriangle3, gripColor);
        var mousePos = ImGui.GetIO().MousePos;
        if (mousePos.X >= gripMin.X && mousePos.X <= gripMax.X && mousePos.Y >= gripMin.Y && mousePos.Y <= gripMax.Y) {
            drawList.AddTriangleFilled(gripTriangle1, gripTriangle2, gripTriangle3, 0x1AFFFFFF);
        }
    }







}
