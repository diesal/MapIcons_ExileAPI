using ImGuiNET;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;

namespace DieselTools_ExileAPI;

public static class IconPicker {

    private static int TitlebarHeight = 20;
    private static readonly SVector4 PanelPadding = new(0, 3, 3, 3);
    private static readonly SVector2 DefaultWindowOffset = new(10, 10);

    // working variables
    private static SVector2 WindowSize = new SVector2(0, 0);



    public class Options
    {
        public string Title = "Icon Picker";
        public SVector2 WindowOffset = DefaultWindowOffset;

        public uint HoveredIconColor = Colors.ButtonHovered; 
        public uint SelectedIconColor = Colors.ButtonChecked;
        public uint IconColor = Colors.White;

    }

    public static void Open(string uniqueID, IconAtlas iconAtlas, SVector2? windowOffset) {
        WindowSize = new(
            PanelPadding.W + iconAtlas.AtlasSize.X + PanelPadding.X,
            TitlebarHeight + PanelPadding.X + iconAtlas.AtlasSize.Y + PanelPadding.Z
        );
        PopupWindow.Open(uniqueID, windowOffset ?? DefaultWindowOffset);
    }

    public static void Draw(string uniqueID, IconAtlas iconAtlas, ref int selectedIconIndex, Options options = null) {

        options ??= new Options();

        if (PopupWindow.Begin(uniqueID, new PopupWindow.Options { Size = WindowSize, Title = options.Title, PanelPadding = PanelPadding, TitleBarHeight = TitlebarHeight })) {
            var contentPos = ImGui.GetCursorScreenPos();
            var drawList = ImGui.GetWindowDrawList();

            for (int y = 0; y < iconAtlas.IconsPerColumn; y++) {
                for (int x = 0; x < iconAtlas.IconsPerRow; x++) {
                    int iconIndex = y * iconAtlas.IconsPerRow + x;
                    (SVector2 uv0, SVector2 uv1) = iconAtlas.GetIconUVs(iconIndex);
                    var buttonPos = contentPos + new SVector2(x * iconAtlas.IconSize.X, y * iconAtlas.IconSize.Y);

                    if (iconIndex == selectedIconIndex) drawList.AddRectFilled(buttonPos, buttonPos + iconAtlas.IconSize, options.SelectedIconColor);

                    ImGui.SetCursorScreenPos(buttonPos);
                    if (ImGui.InvisibleButton($"{uniqueID}textureID_{y}_{x}", iconAtlas.IconSize)) {
                        selectedIconIndex = iconIndex;
                        ImGui.CloseCurrentPopup();
                    }
                    if (ImGui.IsItemHovered()) drawList.AddRectFilled(buttonPos, buttonPos + iconAtlas.IconSize, options.HoveredIconColor);

                    drawList.AddImage(iconAtlas.TextureId, buttonPos, buttonPos + iconAtlas.IconSize, uv0, uv1, options.IconColor );
                }
            }

            PopupWindow.End();
        }
    }



}


