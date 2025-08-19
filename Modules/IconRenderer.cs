using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using SVector2 = System.Numerics.Vector2;

namespace MapIcons {

    public sealed class IconRenderer : PluginModule
    {

        public IconRenderer(Plugin plugin) : base(plugin) { }

        private const float CameraAngle = 38.7f * MathF.PI / 180;
        private static readonly float CameraAngleCos = MathF.Cos(CameraAngle);
        private static readonly float CameraAngleSin = MathF.Sin(CameraAngle);



        public void Initialise() { }
        public void Tick() { }

        public void Render() {
            var ingameUi = GameController.IngameState.IngameUi;
            if (ingameUi == null || !ingameUi.IsVisibleLocal) return;

            var mapCenter = new SVector2();
            var mapScale = 1.0f;
            bool? largeMapOpen = null;

            // Render icons on the map based on the current map state
            var smallMiniMap = ingameUi.Map.SmallMiniMap;
            var largeMapWindow = ingameUi.Map.LargeMap;

            if (smallMiniMap.IsValid && smallMiniMap.IsVisibleLocal) {
                var mapRect = smallMiniMap.GetClientRectCache;
                mapCenter = mapRect.Center.ToVector2Num();
                largeMapOpen = false;
                mapScale = smallMiniMap.MapScale;
            }
            else if (ingameUi.Map.LargeMap.IsVisibleLocal) {
                mapCenter = largeMapWindow.MapCenter;
                largeMapOpen = true;
                mapScale = largeMapWindow.MapScale;
            }
            // check game states 
            if (largeMapOpen == null || !GameController.InGame || !Settings.DrawOnMinimap && largeMapOpen != true) return;
            if (!Settings.DrawOverFullscreenPanels && ingameUi.FullscreenPanels.Any(x => x.IsVisible) || Settings.DrawOverLargePanels && ingameUi.LargePanels.Any(x => x.IsVisible)) return;
            if (largeMapWindow == null) return;

            // Get player position and height
            var playerRender = GameController?.Player?.GetComponent<Render>();
            if (playerRender == null) return;
            var playerPos = playerRender.PosNum.WorldToGrid();
            var playerHeight = -playerRender.UnclampedHeight;
            // Get the cached icons list
            var _iconListCache = Plugin.IconListCache;
            if (_iconListCache == null) return;
            var mapIcons = _iconListCache.Value;
            if (mapIcons == null) return;

            // Render each icon on the map
            foreach (var icon in mapIcons) {

                if (icon?.Entity == null) continue;
                if (!icon.Show()) continue;
                if (icon.Settings == null) {
                    icon.Settings = Settings.GetIconSettings(icon.Type);
                    if (icon.Settings == null) continue;
                }

                if (icon.Settings.Check_IsAlive && !icon.Entity.IsAlive) continue; 
                if (icon.Settings.Check_IsOpened && icon.Entity.IsOpened) continue;

                var iconFileName = Plugin.IconAtlas.Name;
                var iconSize = 0;
                var iconColor = SharpDX.Color.White;
                var iconUV = new SharpDX.RectangleF(); // Default UV coordinates

                // get icon position
                var iconGridPos = icon.GridPosition();
                var iconDelta = iconGridPos - playerPos;
                var iconDeltaZ = (playerHeight + GameController.IngameState.Data.GetTerrainHeightAt(iconGridPos)) * PoeMapExtension.WorldToGridConversion;
                var iconPosition = mapCenter + (mapScale * SVector2.Multiply(new SVector2(iconDelta.X - iconDelta.Y, iconDeltaZ - (iconDelta.X + iconDelta.Y)), new SVector2(CameraAngleCos, CameraAngleSin)));

                // icon rendering
                switch (icon.Renderer) {    
                    case MapIconRenderers.Default:
                        if (!icon.Settings.Draw) continue;
                        iconSize = icon.Settings.Size;
                        iconColor = icon.Hidden() ? icon.Settings.HiddenTint : icon.Settings.Tint;
                        iconUV = Plugin.IconAtlas.GetIconUV(icon.Settings.Index);
                        icon.DrawRect = GetIconPositionRect(iconSize, iconPosition, ingameUi);
                        if (icon.DrawRect == null) continue;

                        Graphics.DrawImage(iconFileName, icon.DrawRect.Value, iconUV, iconColor);
                        if (icon.Settings.DrawText) Graphics.DrawText(icon.Text, iconPosition.Translate(0, 0), FontAlign.Center);

                        break;
                    case MapIconRenderers.Monster:
                        if (!icon.Settings.Draw) continue;
                        iconSize = icon.Settings.Size;
                        iconColor = icon.Hidden() ? icon.Settings.HiddenTint : icon.Settings.Tint;
                        iconUV = Plugin.IconAtlas.GetIconUV(icon.Settings.Index);
                        icon.DrawRect = GetIconPositionRect(iconSize, iconPosition, ingameUi);
                        if (icon.DrawRect == null) continue;

                        Graphics.DrawImage(iconFileName, icon.DrawRect.Value, iconUV, iconColor);
                        if (icon.Settings.DrawText) Graphics.DrawText(icon.Text, iconPosition.Translate(0, 0), FontAlign.Center);

                        break;
                    case MapIconRenderers.IngameIcon:
                        if (icon.Settings.DrawState == IconDrawStates.Off || (icon.Settings.DrawState == IconDrawStates.Ranged && icon.Entity.IsValid)) continue;

                        iconFileName = icon.InGameTexture.FileName;
                        iconSize = (int)icon.InGameTexture.Size;
                        iconUV = icon.InGameTexture.UV;
                        icon.DrawRect = GetIconPositionRect(iconSize, iconPosition, ingameUi);
                        if (icon.DrawRect == null) continue;
                        Graphics.DrawImage(iconFileName, icon.DrawRect.Value, iconUV, iconColor);
                        if (icon.Settings.DrawText) Graphics.DrawText(icon.Text, iconPosition.Translate(0, 0), FontAlign.Center);

                        break;
                    case MapIconRenderers.Chest:
                        if (!icon.Settings.Draw) continue;
                        iconSize = icon.Settings.Size;
                        iconColor = icon.Settings.Tint;
                        iconUV = Plugin.IconAtlas.GetIconUV(icon.Settings.Index);
                        icon.DrawRect = GetIconPositionRect(iconSize, iconPosition, ingameUi);
                        if (icon.DrawRect == null) continue;

                        Graphics.DrawImage(iconFileName, icon.DrawRect.Value, iconUV, iconColor);
                        break;
                    case MapIconRenderers.Player:
                        break;
                    default:
                        break;
                }

            }
        }

        public SharpDX.RectangleF? GetIconPositionRect(int iconSize, SVector2 iconPosition, IngameUIElements inGameUI) {
            float halfSize = iconSize / 2;
            float iconX = iconPosition.X - halfSize;
            float iconY = iconPosition.Y - halfSize;
            if (Settings.PixelPerfectIcons) {
                iconX = MathF.Round(iconX);
                iconY = MathF.Round(iconY);
            }

            var rect = new SharpDX.RectangleF(iconX, iconY, iconSize, iconSize);

            if (inGameUI.Map.LargeMap.IsVisibleLocal == false && !inGameUI.Map.SmallMiniMap.GetClientRectCache.Contains(rect)) return null; 

            return rect;
        }







    }
}
