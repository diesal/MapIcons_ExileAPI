using ExileCore;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ImGuiNET;
using SharpDX;
using System.Diagnostics;

namespace DieselTools_ExileAPI
{

    public static class Exile
    {
        public static GameController GameController { get; private set; }
        public static IngameState IngameState { get; private set; }
        public static Vector2 WindowOffset { get; private set; }
        public static bool InventoryPanelVisible => GameController.IngameState.IngameUi.InventoryPanel.IsVisible;

        public static bool IsGameFocused {
            get {
                // Check if the game window is focused or if an ImGui button is being interacted with
                return GameController.Window.IsForeground() || ImGui.IsAnyItemActive() || ImGui.IsAnyItemHovered() || ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow);
            }
        }

        public static void Init(GameController gameController) {
            if (GameController != null) return; // Already initialized, do nothing

            GameController = gameController;
            IngameState = GameController.Game.IngameState;
            WindowOffset = GameController.Window.GetWindowRectangleTimeCache.TopLeft;
            // init other tools
        }
        public static string RunApplication(string applicationPath, string applicationName) {
            try {
                // Check if the application is already running
                if (Process.GetProcessesByName(applicationName).Any()) {
                    return null; // No error, just don't start again
                }

                // Start the application
                Process.Start(applicationPath);
                return null; // Success, no error
            }
            catch (Exception ex) {
                return $"Failed to start application: {ex.Message}";
            }
        }

        /// <summary>
        /// Retrieves the list of items in the player's inventory.
        /// </summary>
        /// <returns>A list of <see cref="PlayerItem"/> representing the items in the player's inventory.</returns>
        public static List<PlayerItem> GetPlayerInventory() {
            if (!IngameState.IngameUi.InventoryPanel.IsVisible) { return []; }

            var inventoryItems = Exile.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems;
            var playerItems = new List<PlayerItem>();

            foreach (var item in inventoryItems) playerItems.Add(new PlayerItem(item));
            return playerItems;
        }

        public static PlayerItem GetHoveredInventoryItem() {
            if (!IngameState.IngameUi.InventoryPanel.IsVisible) { return null; }

            try {
                var uiHover = Exile.IngameState.UIHover;

                if (uiHover.AsObject<HoverItemIcon>().ToolTipType != ToolTipType.ItemInChat) {
                    var inventoryItemIcon = uiHover.AsObject<NormalInventoryItem>();
                    var tooltip = inventoryItemIcon.Tooltip;
                    var poeEntity = inventoryItemIcon.Item;
                    if (tooltip != null && poeEntity.Address != 0 && poeEntity.IsValid) {
                        var item = inventoryItemIcon.Item;
                        var baseItemType = Exile.GameController.Files.BaseItemTypes.Translate(item.Path);
                        if (baseItemType != null) {
                            return new PlayerItem(inventoryItemIcon);
                        }
                    }
                }
            }
            catch {
                return null;
            }
            return null;
        }



    }

}
