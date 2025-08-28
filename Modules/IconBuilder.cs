using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using SharpDX;
using System.Drawing;
using DBugger = DieselTools_ExileAPI.DBugger;

namespace MapIcons;




public sealed class IconBuilder : PluginModule {
    public IconBuilder(Plugin plugin) : base(plugin) { }


    public static readonly HashSet<string> RogueExilesByName = new HashSet<string>{
        "Torr Olgosso",
        "Armios Bell",
        "Zacharie Desmarais",
        "Jonah Unchained",
        "Damoi Tui",
        "Xandro Blooddrinker",
        "Vickas Giantbone",
        "Orra Greengate",
        "Thena Moga",
        "Antalie Napora",
        "Augustina Solaria",
        "Lael Furia",
        "Vanth Agiel",
        "Ion Darkshroud",
        "Ash Lessard",
        "Wilorin Demontamer",
        "Eoin Greyfur",
        "Tinevin Highdove",
        "Magnus Stonethorn",
        "Minara Anemina",
        "Igna Phoenix",
        "Dena Lorenni",
        "Ailentia Rac",
        "Oyra Ona",
        "Bolt Brownfur",
        "Ulysses Morvant",
        "Aurelio Voidsinger",
        "Antonio Bravadi",
        "Haki Karukaru",
        "Silva Fearsting",
        "Jade",
        "Jarek Irontrap",
        "Doven Falsetongue",
        "Aria Vindicia",
        "Dimachaeri Cassius",
        "Rudiarius Felix",
        "Mevia",
        "Shade of a Duelist",
        "Shade of a Marauder",
        "Shade of a Ranger",
        "Shade of a Scion",
        "Shade of a Shadow",
        "Shade of a Templar",
        "Shade of a Witch",
        "Ultima Thule",
        "Ohne Trix",
        "Kirmes Olli",
        "Baracus Phraxisanct",
        "Thom Imperial",
        "Ainsley Varrich",
        "Sevet Tetherein"
    };
    private List<string> UserSkippedEntityPaths { get; set; }

    private int RunCounter { get; set; }
    private int IconVersion;

    public void RebuildIcons() => IconVersion++;
    public void Initialise() => UpdateUserSkippedEntities();
    public void Tick()
    {
        RunCounter++;
        if (RunCounter % Settings.RunEveryXTicks != 0) return;

        foreach (var entity in GameController.Entities)
        {
            if (entity.GetHudComponent<MapIcon>() is { Version: var version, } && version >= IconVersion) continue;
            MapIcon icon = CreateIcon(entity);
            if (icon == null) continue;
            icon.Version = IconVersion;
            entity.SetHudComponent(icon);
        }
    }

    public void UpdateUserSkippedEntities() {
        // Split the input by lines and add to the list, ignoring lines starting with #
        UserSkippedEntityPaths = new List<string>();
        var lines = Settings.ignoredPaths.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines) {
            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#")) {
                UserSkippedEntityPaths.Add(line.Trim());
            }
        }
        RebuildIcons(); // Forced update on all icons
    }

    private void DebugIcon(MapIcon icon) {
        if (!Settings.DBuggerSettings.ShowLog) return;
        DBugger.Log($"--| Type: {icon.Type} | Renderer: {icon.Renderer} | Entity.Type: {icon.Entity.Type} | RenderName: {icon.Entity.RenderName} | Path: {icon.Entity.Path}");
    }
    private void DebugIngameIcon(MapIcon icon) {
        if (Settings.DebugMinimapIcon) DebugIcon(icon);
    }
    private void DebugMonsterIcon(MapIcon icon) {
        if (Settings.DebugMonsterIcon) DebugIcon(icon);
    }
    private void DebugFriendlyIcon(MapIcon icon) {
        if (Settings.DebugFriendlyIcon) DebugIcon(icon);
    }
    private void DebugTrapIcon(MapIcon icon) {
        if (Settings.DebugTrapIcon) DebugIcon(icon);
    }
    private void DebugMiscIcon(MapIcon icon) {
        if (Settings.DebugMiscIcon) DebugIcon(icon);
    }
    private void DebugCustomIcon(MapIcon icon) {
        if (Settings.DebugCustomIcon) DebugIcon(icon);
    }
    private void DebugChestIcon(MapIcon icon) {
        if (Settings.DebugChestIcon) DebugIcon(icon);
    }

    private void CreateIcon_Strongbox(Entity entity) {
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/ArmourerStrongbox"):
        //    IconType = IconTypes.ArmourerStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/MartialStrongbox"):
        //    IconType = IconTypes.BlacksmithStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Artisan"):
        //    IconType = IconTypes.ArtisanStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Cartographer"):
        //    IconType = IconTypes.CartographerStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Chemist"):
        //    IconType = IconTypes.ChemistStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Gemcutter"):
        //    IconType = IconTypes.GemcutterStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Jeweller"):
        //    IconType = IconTypes.JewellerStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Large"):
        //    IconType = IconTypes.LargeStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Ornate"):
        //    IconType = IconTypes.OrnateStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/StrongboxDivination"):
        //    IconType = IconTypes.DivinerStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Operative"):
        //    IconType = IconTypes.OperativeStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Arcane"):
        //    IconType = IconTypes.ArcaneStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes/Researcher"):
        //    IconType = IconTypes.ResearcherStrongbox;
        //    break;
        //case string path when path.StartsWith("Metadata/Chests/StrongBoxes"):
        //    IconType = IconTypes.UnknownStrongbox;
        //    break;
    }
    private MapIcon CreateIcon_Chest(Entity entity) {
        var icon = new MapIcon(entity);
        icon.Renderer = MapIconRenderers.Chest;
        icon.Name = icon.RenderName;
        icon.Show = () => !entity.IsOpened;

        // Breakables
        var chest = entity.GetComponent<Chest>();
        if (chest?.Stompable == true || chest?.OpenOnDamage == true) {
            icon.Type = MapIconTypes.BreakableObject;
            DebugChestIcon(icon);
            return icon;
        }
        switch (entity.Path) {
            // Breach Chests
            case string path when path.Contains("BreachChest"):
                if (entity.Path.Contains("Large")) {
                    icon.Type = MapIconTypes.BreachChestLarge;
                }
                else {
                    icon.Type = MapIconTypes.BreachChestNormal;
                }
                break;
            // Expedition Chests
            case string path when path.StartsWith("Metadata/Chests/LeaguesExpedition/", StringComparison.Ordinal):
                if (entity.Rarity == MonsterRarity.White) {
                    icon.Type = MapIconTypes.ExpeditionChestWhite;
                }
                else if (entity.Rarity == MonsterRarity.Magic) {
                    icon.Type = MapIconTypes.ExpeditionChestMagic;
                }
                else if (entity.Rarity == MonsterRarity.Rare) {
                    icon.Type = MapIconTypes.ExpeditionChestRare;
                }
                else return null;
                break;
            // Sanctum Chests
            case string path when path.StartsWith("Metadata/Chests/LeagueSanctum/", StringComparison.Ordinal):
                icon.Type = MapIconTypes.SanctumChest;
                break;
            // pirate chest 
            case string path when path.StartsWith("Metadata/Chests/GraveyardBooty", StringComparison.Ordinal):
                icon.Type = MapIconTypes.PirateChest;
                break;
            // Abyss Horde Chests
            case string path when path.StartsWith("Metadata/Chests/AbyssChest", StringComparison.Ordinal):
                icon.Type = MapIconTypes.AbyssChest;
                break;
            // Strongboxes
            //case string path when path.StartsWith("Metadata/Chests/StrongBoxes", StringComparison.Ordinal):
            //    CreateIcon_Strongbox(entity);
            //    break;
            // Default
            default:
                if (entity.Rarity == MonsterRarity.White) {
                    icon.Type = MapIconTypes.ChestWhite;
                }
                else if (entity.Rarity == MonsterRarity.Magic) {
                    icon.Type = MapIconTypes.ChestMagic;
                }
                else if (entity.Rarity == MonsterRarity.Rare) {
                    icon.Type = MapIconTypes.ChestRare;
                }
                else if (entity.Rarity == MonsterRarity.Unique) {
                    icon.Type = MapIconTypes.ChestUnique;
                }
                else return null;
                break;
        }
        DebugChestIcon(icon);
        return icon;
    }
    private MapIcon CreateIcon_Delirium(Entity entity) {
        var icon = new MapIcon(entity);
        icon.Renderer = MapIconRenderers.Monster;
        icon.Name = icon.RenderName;
        icon.Show = () => entity.IsAlive;

        var pathstring = "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemon";

        if (entity.Path.StartsWith(pathstring + "BloodBag", StringComparison.Ordinal)) {
            icon.Type = MapIconTypes.BloodBag;
        }
        else if (entity.Path.StartsWith(pathstring + "EggFodder", StringComparison.Ordinal)) {
            icon.Type = MapIconTypes.EggFodder;
        }
        else if (entity.Path.StartsWith(pathstring + "GlobSpawn", StringComparison.Ordinal)) {
            icon.Type = MapIconTypes.GlobSpawn;
        }
        else return null;

        icon.Hidden = () => false;

        DebugMonsterIcon(icon);
        return icon;
    }
    private MapIcon CreateIcon_LabyrinthTrap(Entity entity) {
        var icon = new MapIcon(entity);
        icon.Name = icon.RenderName;
        icon.Renderer = MapIconRenderers.Monster;
        // Roomba
        if (entity.Path.Contains("LabyrinthRoomba") || entity.Path.Contains("LabyrinthFlyingRoomba")) {
            icon.Show = () => entity.IsValid;
            icon.Type = MapIconTypes.LabyrinthRoomba;
        }
        // Spike Trap 
        else if (entity.Path.Contains("LabyrinthCascadeSpikeTrap")) {
            icon.Hidden = () => !entity.IsTransitioned;
            icon.Type = MapIconTypes.LabyrinthCascadeSpikeTrap;
        }
        // saw blade 
        else if (entity.Path.Contains("LabyrinthSawblade")) {
            icon.Show = () => entity.IsValid;
            icon.Type = MapIconTypes.LabyrinthSawblade;
        }
        // spinner
        else if (entity.Path.Contains("LabyrinthSpinner")) {
            icon.Show = () => entity.IsValid;
            icon.Type = MapIconTypes.LabyrinthSpinner;
        }
        // roller 
        else if (entity.Path.Contains("LabyrinthRoller")) {
            icon.Hidden = () => !entity.IsTransitioned;
            icon.Type = MapIconTypes.LabyrinthRoller;
        }
        else return null;

        DebugTrapIcon(icon);
        return icon;
    }
    private MapIcon CreateIcon_Friendly(Entity entity) {
        if (!entity.IsAlive) return null;
        //Converts on death
        if (entity.HasComponent<ObjectMagicProperties>()) {
            var objectMagicProperties = entity.GetComponent<ObjectMagicProperties>();
            var mods = objectMagicProperties.Mods;
            if (mods != null) {
                if (mods.Contains("MonsterConvertsOnDeath_")) return null;
            }
        }

        var icon = new MapIcon(entity);
        icon.Renderer = MapIconRenderers.Friendly;
        icon.Name = icon.RenderName;
        icon.Show = () => entity.IsAlive;

        // NPC
        if (entity.Type == EntityType.Npc) {
            if (!entity.HasComponent<Render>()) return null;
            var component = entity.GetComponent<Render>();
            icon.Type = MapIconTypes.NPC;
            icon.Name = component?.Name.Split(',')[0];
            icon.Show = () => entity.IsValid;
        }
        // Totem
        else if (entity.Path.Contains("Metadata/Monsters/Totems/TauntTotem")) {
            icon.Priority = IconPriority.High;
            icon.Type = MapIconTypes.DecoyTotem;
        }
        // Minion
        else {
            icon.Priority = IconPriority.Low;
            icon.Type = MapIconTypes.Minion;
        }
        DebugFriendlyIcon(icon);
        return icon;
    }
    private MapIcon CreateIcon_Monster(Entity entity) {
        if (!entity.IsAlive) return null;

        var icon = new MapIcon(entity);
        icon.Renderer = MapIconRenderers.Monster;
        icon.Name = icon.RenderName;
        icon.Show = () => entity.IsAlive;

        // Volatile Core
        if (entity.Path.Contains("SentinelVolatile") || entity.Path.Contains("Metadata/Monsters/LeagueAffliction/Volatile") || entity.Path.Contains("ToxicVolatile") || entity.Path.Contains("VolatileCore") ) {
            icon.Priority = IconPriority.Critical;
            icon.Type = MapIconTypes.VolatileCore;
            icon.Hidden = () => false;
        }
        // Lightning Clone
        else if (entity.Path.Contains("LightningClone")) {
            icon.Priority = IconPriority.Critical;
            icon.Type = MapIconTypes.LightningClone;
            icon.Hidden = () => false;
        }
        // consume phantasm
        else if (entity.Path.Contains("ConsumePhantasm")) {
            icon.Priority = IconPriority.Critical;
            icon.Type = MapIconTypes.ConsumingPhantasm;
            icon.Hidden = () => false;
        }
        // Drowning Orb
        else if (entity.Path.Contains("Metadata/Monsters/AtlasInvaders/ConsumeMonsters/ConsumeBossStalkerOrbUberMaps")) {
            icon.Priority = IconPriority.Critical;
            icon.Type = MapIconTypes.DrowningOrb;
        }
        // Spirit
        else if (icon.Rarity == MonsterRarity.Unique && entity.Path.Contains("Metadata/Monsters/Spirit/")) {
            icon.Type = MapIconTypes.Spirit;
        }
        // Monster
        else {
            icon.Hidden = () => {
                if (entity.IsHidden) return true;
                if (entity.Stats != null && entity.Stats.TryGetValue(GameStat.CannotBeDamaged, out var value) && value > 0) return true;
                return false;
            };
            // legion monsters
            if (entity.Path.Contains("Legion")) {
                var statDictionary = entity.Stats;
                if (statDictionary == null) {
                    icon.Show = () => entity.GetComponent<Life>().HPPercentage > 0.02;
                }
                else if (statDictionary.Count == 0) {
                    statDictionary = entity.GetComponentFromMemory<Stats>()?.StatDictionary ?? statDictionary;
                    if (statDictionary.Count == 0)
                        icon.Name = "Error";
                }
                else
                    icon.Show = () => !icon.Hidden() && entity.GetComponent<Life>()?.HPPercentage > 0.02;
            }
            // White Monster
            if (icon.Rarity == MonsterRarity.White) {
                icon.Type = MapIconTypes.WhiteMonster;
            }
            // Magic
            else if (icon.Rarity == MonsterRarity.Magic) {
                icon.Type = MapIconTypes.MagicMonster;
            }
            // Rare
            else if (icon.Rarity == MonsterRarity.Rare) {
                // Beasts
                switch (entity.RenderName) {
                    case "Vivid Vulture":
                        icon.Type = MapIconTypes.VividVulture;
                        break;
                    case "Black Mórrigan":
                        icon.Type = MapIconTypes.BlackMorrigan;
                        break;
                    case "Craicic Chimeral":
                        icon.Type = MapIconTypes.CraicicChimeral;
                        break;
                    case "Wild Bristle Matron":
                        icon.Type = MapIconTypes.WildBristleMatron;
                        break;
                    case "Wild Hellion Alpha":
                        icon.Type = MapIconTypes.WildHellionAlpha;
                        break;
                    case "Fenumal Plagued Arachnid":
                        icon.Type = MapIconTypes.FenumalPlaguedArachnid;
                        break;
                    case "Fenumus, First of the Night":
                        icon.Type = MapIconTypes.FenumusFirstOfTheNight;
                        break;
                    default:
                        // Rare Monster
                        icon.Type = MapIconTypes.RareMonster;
                        break;
                }
            }
            // Unique Monster
            else if (icon.Rarity == MonsterRarity.Unique) {
                // Rogue Exiles
                if (RogueExilesByName.Contains(entity.RenderName)) {
                    // Giant Rogue Exile 
                    if (entity.GetComponent<Positioned>().Scale >= 1.8) {                        
                        icon.Type = MapIconTypes.GiantRogueExile;
                    }
                    else {
                        icon.Type = MapIconTypes.RogueExile;
                    }
                }
                else { // Unique Monster
                    icon.Type = MapIconTypes.UniqueMonster;
                }
            }
            //Converts on death
            if (entity.HasComponent<ObjectMagicProperties>()) {
                var objectMagicProperties = entity.GetComponent<ObjectMagicProperties>();
                var mods = objectMagicProperties.Mods;
                if (mods != null) {
                    if (mods.Contains("MonsterConvertsOnDeath_"))
                        icon.Show = () => entity.IsAlive && entity.IsHostile;
                }
            }
        }

        DebugMonsterIcon(icon);
        return icon;
    }
    private MapIcon CreateIcon_Ingame(Entity entity) {
        var icon = new MapIcon(entity);
        icon.Renderer = MapIconRenderers.IngameIcon;
        icon.Name = icon.RenderName;

        var minimapIconComponent = entity.GetComponent<MinimapIcon>();
        if (minimapIconComponent == null || minimapIconComponent.IsHide) return null;

        var minimapIconName = minimapIconComponent.Name;
        if (string.IsNullOrEmpty(minimapIconName)) return null;

        var iconIndexByName = Extensions.IconIndexByName(minimapIconName);
        if (iconIndexByName == MapIconsIndex.MyPlayer) return null;

        icon.InGameTexture = new HudTexture("Icons.png") { UV = SpriteHelper.GetUV(iconIndexByName), Size = 16 };
        // revisit this later and recheck logic
        var isHidden = false;
        var transitionableFlag1 = 1;
        var shrineIsAvailable = true;
        var isOpened = false;

        T Update<T>(ref T store, Func<T> update) { return entity.IsValid ? store = update() : store; }

        icon.Show = () => !Update(ref isHidden, () => entity.GetComponent<MinimapIcon>()?.IsHide ?? isHidden) &&
                           Update(ref transitionableFlag1, () => entity.GetComponent<Transitionable>()?.Flag1 ?? 1) == 1 &&
                           Update(ref shrineIsAvailable, () => entity.GetComponent<Shrine>()?.IsAvailable ?? shrineIsAvailable) &&
                          !Update(ref isOpened, () => entity.GetComponent<Chest>()?.IsOpened ?? isOpened);

        var iconSizeMultiplier = RemoteMemoryObject.pTheGame.Files.MinimapIcons.EntriesList.ElementAtOrDefault((int)iconIndexByName)?.LargeMinimapSize ?? 1;
        icon.InGameTexture.Size = RemoteMemoryObject.pTheGame.IngameState.IngameUi.Map.LargeMapZoom * RemoteMemoryObject.pTheGame.IngameState.Camera.Height * iconSizeMultiplier / 64;

        // Shrine
        if (entity.HasComponent<Shrine>()) {
            icon.Type = MapIconTypes.Shrine;
        }
        // NPC
        else if (entity.Type == EntityType.Npc) {
            icon.Type = MapIconTypes.IngameNPC;
        }
        // QuestObject
        else if (minimapIconName == "QuestObject") {
            icon.Type = MapIconTypes.QuestObject;
        }
        else {
            switch (entity.Path) {
                case string path when path.StartsWith("Metadata/MiscellaneousObjects/Waypoint"):
                    icon.Type = MapIconTypes.Waypoint;
                    break;
                case string path when path.StartsWith("Metadata/MiscellaneousObjects/Checkpoint"):
                    icon.Type = MapIconTypes.Checkpoint;
                    break;
                case string path when path.StartsWith("Metadata/MiscellaneousObjects/AreaTransition"):
                    icon.Type = MapIconTypes.AreaTransition;
                    break;
                case string path when path.StartsWith("Metadata/Terrain/Leagues/Ritual/RitualRuneInteractable"):
                    icon.Type = MapIconTypes.Ritual;
                    break;
                case string path when path.StartsWith("Metadata/MiscellaneousObjects/Breach/BreachObject"):
                    icon.Type = MapIconTypes.Breach;
                    break;
                default:
                    icon.Type = MapIconTypes.IngameUncategorized;
                    break;
            }
        }
        DebugIngameIcon(icon);
        return icon;
    }

    private MapIcon CreateIcon(Entity entity)
    {
        if (entity is null) return null;
        if (string.IsNullOrEmpty(entity.Path)) return null;
        if (!entity.IsValid) return null; // only adding the icon if its valid, to make sure components are available when creating icon... 
        // custom icon by path
        var customPathIconSettings = Settings.CustomPathIcons.FirstOrDefault(setting => entity.Path.StartsWith(setting.Path, StringComparison.Ordinal));
        if (customPathIconSettings != null)
        {
            var icon = new MapIcon(entity);
            icon.Settings = customPathIconSettings;
            icon.Type = MapIconTypes.CustomPath;
            icon.Name = entity.RenderName;
            icon.Show = () => true;
            DebugCustomIcon(icon);
            return icon;
        }
        // SKIP entity types 
        if (entity.Type == EntityType.HideoutDecoration) return null; 
        if (entity.Type == EntityType.Effect) return null; 
        if (entity.Type == EntityType.Light) return null; 
        if (entity.Type == EntityType.ServerObject) return null; 
        if (entity.Type == EntityType.Daemon) return null; 
        if (entity.Type == EntityType.Error) return null;
        if (entity.Type == EntityType.WorldItem) return null; 
        if (entity.Type == EntityType.MiscellaneousObjects) return null; 
        // SKIP entity paths 
        if (entity.Path.StartsWith("Metadata/NPC/Hideout")) return null;
        if (entity.Path.Contains("MapLightBot")) return null;
        // userinterface skipped paths
        if (UserSkippedEntityPaths.Any(path => entity.Path?.StartsWith(path) == true)) return null;

        // LabyrinthTrap Icons
        if (entity.Path.StartsWith("Metadata/Terrain/Labyrinth/Traps/", StringComparison.Ordinal)) return CreateIcon_LabyrinthTrap(entity);
        // Sanctum Mote
        if (entity.Type == EntityType.Terrain && entity.Path.StartsWith("Metadata/Terrain/Leagues/Sanctum/Objects/SanctumMote")) {
            var icon = new MapIcon(entity);
            icon.Type = MapIconTypes.SanctumMote;
            DebugMiscIcon(icon);
            return icon;
        }

        // SKIP entity type terrain 
        if (entity.Type == EntityType.Terrain) return null; 

        // Ingame Icon
        if (entity.HasComponent<MinimapIcon>()) return CreateIcon_Ingame(entity);
        // Delirium Icons
        if (entity.Path.StartsWith("Metadata/Monsters/LeagueAffliction/DoodadDaemons", StringComparison.Ordinal)) return CreateIcon_Delirium(entity);
        // Monster Icon
        if (entity.Type == EntityType.Monster ) {
            if (entity.IsHostile) {
                return CreateIcon_Monster(entity);                
            }
            else {
                return CreateIcon_Friendly(entity);
            }
        }
        // NPC Icon
        if (entity.Type == EntityType.Npc) return CreateIcon_Friendly(entity);
        // Chest
        if (entity.Type == EntityType.Chest && !entity.IsOpened) return CreateIcon_Chest(entity);
        // Player
        if (entity.Type == EntityType.Player) {
            if (!entity.IsValid) return null;
            var icon = new MapIcon(entity);
            icon.Renderer = MapIconRenderers.Friendly;
            icon.Name = icon.RenderName;

            icon.Name = entity.GetComponent<Player>().PlayerName;
            // Local Player
            if (GameController.IngameState.Data.LocalPlayer.Address == entity.Address) {
                icon.Type = MapIconTypes.LocalPlayer;
            }
            // Player
            else {
                icon.Type = MapIconTypes.OtherPlayer;
            }
            DebugFriendlyIcon(icon);
            return icon;
        }

        return null;
    }
}
