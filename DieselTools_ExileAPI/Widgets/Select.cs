using ImGuiNET;
using SVector2 = System.Numerics.Vector2;


namespace DieselTools_ExileAPI;

public static class Select {


    public class Options
    {
        public SVector2 PositionOffset { get; set; } = new SVector2(0, 0);
        public int? Width { get; set; }
        public int? Height { get; set; }


    }


    public static int DrawCustomDropdown( string uniqueId, IReadOnlyList<string> items, int selected, Func<int, string>? getIcon = null, ColorPicker.Options? options = null) {
        if (options == null) options = new ColorPicker.Options();

        var drawList = ImGui.GetWindowDrawList();
        SVector2 pos = ImGui.GetCursorScreenPos();
        float itemHeight = 24;
        float iconSize = 20;
        float width = options.Size.X;
        float height = itemHeight;

        // Draw dropdown "button"
        drawList.AddRectFilled(pos, pos + new SVector2(width, height), Colors.Panel);
        drawList.AddRect(pos, pos + new SVector2(width, height), Colors.PanelBorder);

        // Draw selected item (icon + text)
        if (getIcon != null) {
            drawList.AddText(pos + new SVector2(2, 2), Colors.ControlText, getIcon(selected));
            ImGui.SetCursorScreenPos(pos + new SVector2(iconSize + 6, 2));
        }
        else {
            ImGui.SetCursorScreenPos(pos + new SVector2(6, 2));
        }
        ImGui.Text(items[selected]);

        // Dropdown arrow (custom, not ImGui)
        drawList.AddText(pos + new SVector2(width - 18, 4), Colors.ControlText, "\u25BC"); // ▼

        // Open popup on click
        ImGui.SetCursorScreenPos(pos);
        if (ImGui.InvisibleButton($"##{uniqueId}_dropdown_btn", new SVector2(width, height)))
            ImGui.OpenPopup($"##{uniqueId}_dropdown_popup");

        int newSelected = selected;
        if (ImGui.BeginPopup($"##{uniqueId}_dropdown_popup")) {
            SVector2 popupPos = ImGui.GetCursorScreenPos();
            for (int i = 0; i < items.Count; i++) {
                SVector2 itemPos = popupPos + new SVector2(0, i * itemHeight);
                // Draw item background
                uint bgColor = (i == selected) ? Colors.ControlInput : Colors.Panel;
                drawList.AddRectFilled(itemPos, itemPos + new SVector2(width, itemHeight), bgColor);
                drawList.AddRect(itemPos, itemPos + new SVector2(width, itemHeight), Colors.PanelBorder);

                // Draw icon if present
                if (getIcon != null) {
                    drawList.AddText(itemPos + new SVector2(2, 2), Colors.ControlText, getIcon(i));
                    ImGui.SetCursorScreenPos(itemPos + new SVector2(iconSize + 6, 2));
                }
                else {
                    ImGui.SetCursorScreenPos(itemPos + new SVector2(6, 2));
                }
                ImGui.Text(items[i]);

                // Handle selection
                ImGui.SetCursorScreenPos(itemPos);
                if (ImGui.InvisibleButton($"##{uniqueId}_item_{i}", new SVector2(width, itemHeight))) {
                    newSelected = i;
                    ImGui.CloseCurrentPopup();
                }
            }
            ImGui.EndPopup();
        }
        return newSelected;
    }


}
