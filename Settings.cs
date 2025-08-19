
using ExileCore;
using ExileCore.Shared.Nodes;
using ExileCore.Shared.Interfaces;
using System.Numerics;
using SixLabors.ImageSharp.Processing;
using SharpDX;

namespace MapIcons;

public class MapIconSettings {

    public bool Draw = true;
    public bool DrawText = false;
    public int Size = 32;
    public int Index = 0;
    public Color Tint = new Color(255, 255, 255, 255);         // White, fully opaque
    public Color HiddenTint = new Color(128, 128, 128, 255);   // Gray, fully opaque

    //existing minimap icons
    public IconDrawStates DrawState = IconDrawStates.Off;

    //custom path icon settings
    public string Path = "";
    public bool Check_IsAlive = false;
    public bool Check_IsOpened = false;
}
public sealed class Settings : ISettings {
    public ToggleNode Enable { get; set; } = new(true);

    public int IconListUpdatePeriod = 100; // milliseconds
    public int RunEveryXTicks = 5;

    public bool DrawOnMinimap = true;
    public bool PixelPerfectIcons = true;
    public bool DrawCachedEntities = true;
    public bool DrawOverLargePanels = false;
    public bool DrawOverFullscreenPanels = false;

    public int IgnoredEntitiesHeight = 100;
    public bool DrawSettingsOpen = true;
    public bool IgnoredEntitesOpen = false;
    public bool CustomIconsOpen = true;

    //debug
    public bool Debug = false;
    public bool DebugMiscIcon = false;
    public bool DebugMinimapIcon = false;
    public bool DebugChestIcon = false;
    public bool DebugMonsterIcon = false;
    public bool DebugFriendlyIcon = false;
    public bool DebugTrapIcon = false;
    public bool DebugCustomIcon = false;
    public bool DebugUser = false;

    // category 
    public Dictionary<string, bool> CategoryHeadersOpen { get; set; } = new Dictionary<string, bool>();
    public bool GetCategoryHeaderOpen(string category, bool defaultValue = true) {
        if (!CategoryHeadersOpen.ContainsKey(category)) {
            CategoryHeadersOpen[category] = defaultValue;
        }
        return CategoryHeadersOpen[category];
    }  
    public void SetCategoryHeader(string category, bool value) {
        if (CategoryHeadersOpen.ContainsKey(category)) {
            CategoryHeadersOpen[category] = value;
        } else {
            CategoryHeadersOpen.Add(category, value);
        }
    }
    
    // icon settings
    public Dictionary<MapIconTypes, MapIconSettings> IconSettingsByType { get; set; } = new();
    public MapIconSettings GetIconSettings(MapIconTypes type) {
        return IconSettingsByType.TryGetValue(type, out var settings) ? settings : null;
    }
    public MapIconSettings SetDefaultIconSettings(MapIconTypes type, MapIconSettings defaultSettings) {
        if (!IconSettingsByType.ContainsKey(type)) {
            IconSettingsByType[type] = defaultSettings;
        }
        return IconSettingsByType[type];
    }

    // custom path icons
    public List<MapIconSettings> CustomPathIcons { get; set; } = new List<MapIconSettings> { };
    public void NewCustomPathIcon() {
        var customIcon = new MapIconSettings {
            Path = "Metadata/CustomPath/ReplaceMe",
        };
        CustomPathIcons.Add(customIcon);
    }
    public void RemoveCustomPathIcon(int index) {
        if (index >= 0 && index < CustomPathIcons.Count) CustomPathIcons.RemoveAt(index);
    }

    //ignored entities
    public string ignoredPaths = @"# Random Ignores
Metadata/Monsters/InvisibleFire/InvisibleSandstorm_
Metadata/Monsters/InvisibleFire/InvisibleFrostnado
Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionDemonColdDegen
Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionDemonColdDegenUnique
Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionCorpseDegen
Metadata/Monsters/InvisibleFire/InvisibleFireEyrieHurricane
Metadata/Monsters/InvisibleFire/InvisibleIonCannonFrost
Metadata/Monsters/InvisibleFire/AfflictionBossFinalDeathZone
Metadata/Monsters/InvisibleFire/InvisibleFireDoedreSewers
Metadata/Monsters/InvisibleFire/InvisibleFireDelveFlameTornadoSpiked
Metadata/Monsters/InvisibleFire/InvisibleHolyCannon
Metadata/Monsters/InvisibleCurse/InvisibleFrostbiteStationary
Metadata/Monsters/InvisibleCurse/InvisibleConductivityStationary
Metadata/Monsters/InvisibleCurse/InvisibleEnfeeble
Metadata/Monsters/InvisibleAura/InvisibleWrathStationary

# Metadata/Monsters/Labyrinth/GoddessOfJustice
# Metadata/Monsters/Labyrinth/GoddessOfJusticeMapBoss
Metadata/Monsters/Frog/FrogGod/SilverOrb
Metadata/Monsters/Frog/FrogGod/SilverPool
Metadata/Monsters/LunarisSolaris/SolarisCelestialFormAmbushUniqueMap
Metadata/Monsters/Invisible/MaligaroSoulInvisibleBladeVortex
Metadata/Monsters/Daemon
Metadata/Monsters/Daemon/MaligaroBladeVortexDaemon
Metadata/Monsters/Daemon/SilverPoolChillDaemon
Metadata/Monsters/AvariusCasticus/AvariusCasticusStatue
Metadata/Monsters/Maligaro/MaligaroDesecrate

# Ritual
Metadata/Monsters/LeagueRitual/FireMeteorDaemon
Metadata/Monsters/LeagueRitual/GenericSpeedDaemon
Metadata/Monsters/LeagueRitual/ColdRotatingBeamDaemon
Metadata/Monsters/LeagueRitual/ColdRotatingBeamDaemonUber
Metadata/Monsters/LeagueRitual/GenericEnergyShieldDaemon
Metadata/Monsters/LeagueRitual/GenericMassiveDaemon
Metadata/Monsters/LeagueRitual/ChaosGreenVinesDaemon_
Metadata/Monsters/LeagueRitual/ChaosSoulrendPortalDaemon
Metadata/Monsters/LeagueRitual/VaalAtziriDaemon
Metadata/Monsters/LeagueRitual/LightningPylonDaemon";

}