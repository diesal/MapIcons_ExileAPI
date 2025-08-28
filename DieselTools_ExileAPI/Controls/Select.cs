using ImGuiNET;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;


namespace DieselTools_ExileAPI;

public static class Select {


    public class Options
    {
        public Tooltip.Options? Tooltip { get; set; }
        public List<string>? Items { get; set; }
        public SVector2 PositionOffset { get; set; } = new SVector2(0, 0);
        public int? Width { get; set; }
        public int? Height { get; set; }
        public uint SelectBorderColor { get; set; } = Colors.Black;
        public uint SelectOuterGlow { get; set; } = Colors.ControlOuterGlow;
        public uint SelectInnerGlow { get; set; } = Colors.ControlInnerGlow;
        public uint SelectColor { get; set; } = Colors.Button;
        public uint HighlightColor { get; set; } = Colors.Highlight;


        public SVector4 DropdownPadding { get; set; } = new SVector4(2, 2, 2, 2); 
        public int DropdownItemSpacing { get; set; } = 3; 
        public uint DropdownBackgroundColor { get; set; } = Colors.Black;
        public uint DropdownBorderColor { get; set; } = Colors.Black;
        public uint DropdownSelectedItemColor { get; set; } = Colors.ButtonChecked;

    }


    public static bool Draw( string uniqueId, ref int selected, Options? options = null) {
        if (string.IsNullOrEmpty(uniqueId)) throw new ArgumentException("uniqueId cannot be null or empty", nameof(uniqueId));
        if (options == null) throw new ArgumentNullException(nameof(options), "Options cannot be null");
        if (options.Items == null || options.Items.Count == 0) throw new ArgumentException("Items cannot be null or empty", nameof(options.Items));
        bool newSelected = false;
        // position and size
        var pos = ImGui.GetCursorScreenPos() + options.PositionOffset;
        var width = options.Width ?? ImGui.GetContentRegionAvail().X;
        var height = options.Height ?? ImGui.GetFrameHeight();
        // Draw Select  
        var drawList = ImGui.GetWindowDrawList();
        drawList.AddRectFilled(pos, pos + new SVector2(width, height), options.SelectColor);
        drawList.AddRect(pos + new SVector2(1, 1),pos + new SVector2(width - 1, height - 1), options.SelectInnerGlow);
        drawList.AddRect(pos, pos + new SVector2(width, height), options.SelectBorderColor);
        drawList.AddRect(pos - new SVector2(1, 1), pos + new SVector2(width + 1, height + 1), options.SelectOuterGlow);

        // Draw text
        ImGui.SetCursorScreenPos(pos + new SVector2(4, 1));
        ImGui.Text(options.Items[selected]);
        // Dropdown arrow (custom, not ImGui)
        // Dropdown arrow (drawn triangle)
        int arrowWidth = 6;
        int arrowHeight = 6;
        SVector2 arrowPos = pos + new SVector2(width - arrowWidth - 5, (int)((height - arrowHeight) / 2) +1);
        // Points for a downward triangle
        SVector2 p1 = arrowPos;
        SVector2 p2 = arrowPos + new SVector2(arrowWidth, 0);
        SVector2 p3 = arrowPos + new SVector2(arrowWidth / 2f, arrowHeight);
        drawList.AddTriangleFilled(p1, p2, p3, Colors.ControlText);

        // Open popup on click
        // Set cursor to just below the input for the popup trigger
        ImGui.SetCursorScreenPos(pos);
        if (ImGui.InvisibleButton($"##{uniqueId}_dropdown_btn", new SVector2(width, height)))
            ImGui.OpenPopup($"##{uniqueId}_dropdown_popup");
        if (ImGui.IsItemHovered()) {
            drawList.AddRectFilled(pos, pos + new SVector2(width, height), options.HighlightColor);
            if (options.Tooltip != null)
                Tooltip.Draw(options.Tooltip);
        }

        // Calculate popup size
        float itemHeight = ImGui.GetFontSize();
        int itemSpacing = options.DropdownItemSpacing;
        int itemCount = options.Items.Count;
        var padding = options.DropdownPadding;
        float leftPad = padding.X;
        float topPad = padding.Y;
        float rightPad = padding.Z;
        float bottomPad = padding.W;
        float contentHeight = itemCount * itemHeight + ((itemCount > 1) ? (itemCount - 1) * itemSpacing : 0);
        float totalHeight = topPad + contentHeight + bottomPad;

        // Position popup directly below input
        ImGui.SetNextWindowPos(pos + new SVector2(0, height));
        ImGui.SetNextWindowSize(new SVector2(width, totalHeight));

        if (ImGui.BeginPopup($"##{uniqueId}_dropdown_popup")) {
            var popupDrawList = ImGui.GetWindowDrawList();
            var bgStart = ImGui.GetCursorScreenPos();
            var bgEnd = bgStart + new SVector2(width, totalHeight);

            // Draw full dropdown background and border
            popupDrawList.AddRectFilled(bgStart, bgEnd, options.DropdownBackgroundColor);
            popupDrawList.AddRect(bgStart, bgEnd, options.DropdownBorderColor);

            // Draw items inside the padded area
            for (int i = 0; i < itemCount; i++) {
                float y = topPad + i * itemHeight + (i > 0 ? i * itemSpacing : 0);

                ImGui.SetCursorPos(new SVector2(leftPad, y));
                var itemPos = ImGui.GetCursorScreenPos();
                var itemSize = new SVector2(width - leftPad - rightPad, itemHeight);

                // Draw background ONLY for selected item
                if (i == selected) {
                    popupDrawList.AddRectFilled(itemPos, itemPos + itemSize, options.DropdownSelectedItemColor);
                }

                ImGui.SetCursorPos(new SVector2(leftPad + 6, y-2));
                ImGui.Text(options.Items[i]);

                ImGui.SetCursorPos(new SVector2(leftPad, y));
                if (ImGui.InvisibleButton($"##{uniqueId}_item_{i}", itemSize)) {
                    selected = i;
                    newSelected = true;
                    ImGui.CloseCurrentPopup();
                }
                if (ImGui.IsItemHovered()) {
                    // Highlight the hovered item
                    popupDrawList.AddRectFilled(itemPos, itemPos + itemSize, options.HighlightColor);
                }
            }
            ImGui.EndPopup();
        }




        return newSelected;
    }


}
