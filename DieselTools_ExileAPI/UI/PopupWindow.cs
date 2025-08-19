using SVector4 = System.Numerics.Vector4;
using SVector2 = System.Numerics.Vector2;
using ImGuiNET;

namespace DieselTools_ExileAPI;

public static class PopupWindow {

    public class Options
    {
        public string? Title;
        public SVector2 Offset { get; set; } = new SVector2(10, 10);
        public SVector2 Size { get; set; } = new SVector2(200, 100); // Default size if not specified
        public int TitleBarHeight { get; set; } = 18; // Height of the title bar
        /// <summary>
        /// top, right, bottom, left padding around the panel
        /// </summary>
        public SVector4 PanelPadding { get; set; } = new SVector4(0, 3, 3, 3); // Padding around the panel
        public uint BackgroundColor { get; set; } = Colors.Black; // Default background
        public uint PanelColor { get; set; } = Colors.Panel; // Default panel color
        public uint PanelBorderColor { get; set; } = Colors.PanelBorder; // Default panel border color
        public uint TextColor { get; set; } = Colors.ControlText; // Default text color
    }


    public static void Open(string unique_id, SVector2 offset) {
        ImGui.SetNextWindowPos(ImGui.GetMousePos() + offset, ImGuiCond.Always);
        ImGui.OpenPopup($"##{unique_id}POPUP");
    }
    public static void Draw(string unique_id, Options options, Action<SVector2> content) {
        if (ImGui.IsPopupOpen($"##{unique_id}POPUP")) {

            var popupSize = new SVector2(options.Size.X + 5, options.Size.Y + 5);
            ImGui.SetNextWindowSize(popupSize, ImGuiCond.Always);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new SVector2(0, 0)); // Remove window padding
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new SVector2(0, 0)); // Remove frame padding
            ImGui.PushStyleColor(ImGuiCol.PopupBg, new SVector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Border, new SVector4(0, 0, 0, 0));

            ImGui.BeginPopup($"##{unique_id}POPUP");
            // draw custom background and panel
            var drawList = ImGui.GetWindowDrawList();
            SVector2 winPos = ImGui.GetWindowPos() + new SVector2(1, 1);
            SVector2 panelPos = winPos + new SVector2(options.PanelPadding.W, options.TitleBarHeight + options.PanelPadding.X);
            SVector2 panelSize = new SVector2(
                options.Size.X - options.PanelPadding.Y - options.PanelPadding.W,
                options.Size.Y - options.TitleBarHeight - options.PanelPadding.X - options.PanelPadding.Z
            );
            drawList.AddRectFilled(winPos, winPos + options.Size, Colors.WindowBackground, 0);
            drawList.AddRectFilled(panelPos, panelPos + panelSize, options.PanelColor, 0.0f);
            drawList.AddRect(panelPos, panelPos + panelSize, options.PanelBorderColor, 0.0f, ImDrawFlags.None, 1.0f);
            if (!string.IsNullOrEmpty(options.Title)) {
                drawList.AddText(winPos + new SVector2(2, 0), options.TextColor, options.Title);
            }
            // draw content
            ImGui.SetCursorScreenPos(panelPos);
            content?.Invoke(panelPos);

            ImGui.EndPopup();
            ImGui.PopStyleColor(2); // Pop both colors
            ImGui.PopStyleVar(2); // Pop window padding
        }
    }






}
