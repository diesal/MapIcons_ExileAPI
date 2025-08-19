using ExileCore;
using ExileCore.Shared.Cache;
using ImGuiNET;
using System.Drawing;
using System.Numerics;

namespace MapIcons;

public class Plugin : BaseSettingsPlugin<Settings>
{
    //--| Classes |--------------------------------------------------------------------------------------------------
    private IconAtlas _iconAtlas;
    public IconAtlas IconAtlas => _iconAtlas ??= new IconAtlas(Graphics, "MapIcons", Path.Combine(Path.GetDirectoryName(typeof(MapIcon).Assembly.Location), "media", "MapIcons.png"), new Vector2(32, 32));
    //--| Modules |--------------------------------------------------------------------------------------------------
    private IconBuilder _iconBuilder;       
    public IconBuilder IconBuilder => _iconBuilder ??= new IconBuilder(this);

    private IconRenderer _iconRenderer;
    private IconRenderer IconRenderer => _iconRenderer ??= new IconRenderer(this);

    private UserInterface _userInterface;
    private UserInterface UserInterface => _userInterface ??= new UserInterface(this);

    //--| fields |--------------------------------------------------------------------------------------------------
    public CachedValue<List<MapIcon>> IconListCache;
    private TimeCache<List<MapIcon>> CreateIconListCache() {
        return new TimeCache<List<MapIcon>>(() => {
            var entitySource = Settings.DrawCachedEntities
                ? GameController?.EntityListWrapper.Entities
                : GameController?.EntityListWrapper?.OnlyValidEntities;
            var baseIcons = entitySource?.Select(x => x.GetHudComponent<MapIcon>())
                .Where(icon => icon != null)
                .OrderBy(x => x.Priority)
                .ToList();
            return baseIcons ?? [];
        }, Settings.IconListUpdatePeriod);
    }




    //--| Initialise |--------------------------------------------------------------------------------------------------
    public override bool Initialise() {
        CanUseMultiThreading = true;

        Log.SetCustomHeaderControls(CustomHeaderControls);

        IconBuilder.Initialise();
        Graphics.InitImage("Icons.png");

        IconListCache = CreateIconListCache();

        return base.Initialise();
    }
    //--| Draw Settings |-----------------------------------------------------------------------------------------------
    public override void DrawSettings() {

        UserInterface.Draw();

    }

    //--| Tick |-------------------------------------------------------------------------------------------------------
    public override Job Tick() {
        IconBuilder.Tick();

        return null;
    }
    //--| Render |-----------------------------------------------------------------------------------------------------
    public override void Render() {
        Log.Render(ref Settings.Debug);

        IconRenderer.Render();      
    }
    //--| Misc |-------------------------------------------------------------------------------------------------------
    private void CustomHeaderControls() {
        ImGui.SameLine();
        ImGui.Checkbox("Friendly", ref Settings.DebugFriendlyIcon); ImGui.SameLine();
        ImGui.Checkbox("Monster", ref Settings.DebugMonsterIcon); ImGui.SameLine();
        ImGui.Checkbox("Chest", ref Settings.DebugChestIcon); ImGui.SameLine();
        ImGui.Checkbox("Ingame", ref Settings.DebugMinimapIcon); ImGui.SameLine();
        ImGui.Checkbox("Misc", ref Settings.DebugMiscIcon); ImGui.SameLine();
        ImGui.Checkbox("User", ref Settings.DebugUser); ImGui.SameLine();
        if (ImGui.Button("Rebuild Icons")) IconBuilder.RebuildIcons();
    }

}