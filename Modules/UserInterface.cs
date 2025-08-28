using ExileCore.Shared.Helpers;
using ImGuiNET;
using System.Reflection;
using DT = DieselTools_ExileAPI;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;

namespace MapIcons
{


    public class Tree
    {
        public List<TreeCategory> Categories { get; set; } = new List<TreeCategory>();
    }
    public class TreeCategory
    {
        public string Name { get; set; }
        public List<TreeIcon> TreeIcons { get; set; } = new List<TreeIcon>();

    }
    public class TreeIcon
    {
        public string Name { get; set; }
        public TreeIconConfigs Config { get; set; }
        public MapIconTypes MapIconType { get; set; }
        public Action<MapIconSettings> CustomDrawAction { get; set; }
        public MapIconSettings DefaultSettings { get; set; } = new MapIconSettings();
    }

    public sealed class UserInterface : PluginModule {
        public UserInterface(Plugin plugin) : base(plugin) { }

        public bool ShowIconPicker { get; set; } = false;
        public string SelectedIconButton { get; set; } = "";

        private Tree UITree = new Tree {
            Categories = new List<TreeCategory> {

                // Ingame Icons
                new TreeCategory {
                    Name = "Ingame Icons",
                    TreeIcons = new List<TreeIcon> {
                        new TreeIcon {
                            Name = "Shrine",
                            MapIconType = MapIconTypes.Shrine,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Always, DrawName = true }
                        },
                        new TreeIcon {
                            Name = "Breach",
                            MapIconType = MapIconTypes.Breach,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Ranged }
                        },
                        new TreeIcon {
                            Name = "Area Transition",
                            MapIconType = MapIconTypes.AreaTransition,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Ranged }
                        },
                        new TreeIcon {
                            Name = "Quest Object",
                            MapIconType = MapIconTypes.QuestObject,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Ranged, DrawName = true }
                        },
                        new TreeIcon {
                            Name = "Ritual",
                            MapIconType = MapIconTypes.Ritual,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Always }
                        },
                        new TreeIcon {
                            Name = "Waypoint",
                            MapIconType = MapIconTypes.Waypoint,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Ranged }
                        },
                        new TreeIcon {
                            Name = "Checkpoint",
                            MapIconType = MapIconTypes.Checkpoint,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Ranged }
                        },
                        new TreeIcon {
                            Name = "Ingame NPC",
                            MapIconType = MapIconTypes.IngameNPC,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Ranged }
                        },
                        new TreeIcon {
                            Name = "Ingame Uncategorized",
                            MapIconType = MapIconTypes.IngameUncategorized,
                            Config = TreeIconConfigs.IngameIcon,
                            DefaultSettings = new MapIconSettings { DrawState = IconDrawStates.Ranged }
                        },
                    }
                },
                // Dangerous Icons
                new TreeCategory {
                    Name = "Dangerous Icons",
                    TreeIcons = new List<TreeIcon> {
                        new TreeIcon {
                            Name = "Volatile Core",
                            MapIconType = MapIconTypes.VolatileCore,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 25,
                                Tint = new SharpDX.Color(213,0,249,255), 
                                HiddenTint = new SharpDX.Color(244,179,255,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Drowning Orb",
                            MapIconType = MapIconTypes.DrowningOrb,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 25,
                                Tint = new SharpDX.Color(213,0,249,255),
                                HiddenTint = new SharpDX.Color(244,179,255,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Lightning Clone",
                            MapIconType = MapIconTypes.LightningClone,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 25,
                                Tint = new SharpDX.Color(213,0,249,255),
                                HiddenTint = new SharpDX.Color(244,179,255,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Consuming Phantasm",
                            MapIconType = MapIconTypes.ConsumingPhantasm,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 25,
                                Tint = new SharpDX.Color(213,0,249,255),
                                HiddenTint = new SharpDX.Color(244,179,255,255),
                            }
                        },
                    },
                },
                // Labyrinth Traps
                new TreeCategory {
                    Name = "Labyrinth Traps",
                    TreeIcons = new List<TreeIcon> {
                        new TreeIcon {
                            Name = "Labyrinth Roomba",
                            MapIconType = MapIconTypes.LabyrinthRoomba,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 55,
                                Tint = new SharpDX.Color(213,0,0,255),
                                HiddenTint = new SharpDX.Color(213,0,0,64),
                            }
                        },
                        new TreeIcon {
                            Name = "Labyrinth Spike Trap",
                            MapIconType = MapIconTypes.LabyrinthCascadeSpikeTrap,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 55,
                                Tint = new SharpDX.Color(213,0,0,255),
                                HiddenTint = new SharpDX.Color(213,0,0,64),
                            }
                        },
                        new TreeIcon {
                            Name = "Labyrinth Saw Blade",
                            MapIconType = MapIconTypes.LabyrinthSawblade,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 50,
                                Tint = new SharpDX.Color(213,0,0,255),
                                HiddenTint = new SharpDX.Color(213,0,0,64),
                            }
                        },
                        new TreeIcon {
                            Name = "Labyrinth Spinner",
                            MapIconType = MapIconTypes.LabyrinthSpinner,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 52,
                                Tint = new SharpDX.Color(255, 0, 0, 255),
                                HiddenTint = new SharpDX.Color(255, 0, 0, 60),
                            }
                        },
                        new TreeIcon {
                            Name = "Labyrinth Roller",
                            MapIconType = MapIconTypes.LabyrinthRoller,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 51,
                                Tint = new SharpDX.Color(255, 0, 0, 255),
                                HiddenTint = new SharpDX.Color(255, 0, 0, 60),
                            }
                        },
                    },
                },
                // Monster Icons
                new TreeCategory {
                    Name = "Monster Icons",
                    TreeIcons = new List<TreeIcon> {
                        new TreeIcon {
                            Name = "Normal Monster",
                            Config = TreeIconConfigs.Monster,
                            MapIconType = MapIconTypes.WhiteMonster,
                            DefaultSettings = new MapIconSettings
                            {
                                Tint = new SharpDX.Color(213,0,0,255), 
                                HiddenTint = new SharpDX.Color(255,185,179,255), // Light Red
                            }
                        },
                        new TreeIcon {
                            Name = "Magic Monster",
                            Config = TreeIconConfigs.Monster,
                            MapIconType = MapIconTypes.MagicMonster,
                            DefaultSettings = new MapIconSettings {
                                Index = 1,
                                Tint = new SharpDX.Color(0,145,234,255),
                                HiddenTint = new SharpDX.Color(179,232,255,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Rare Monster",
                            Config = TreeIconConfigs.Monster,
                            MapIconType = MapIconTypes.RareMonster,
                            DefaultSettings = new MapIconSettings {
                                Index = 2,
                                Tint = new SharpDX.Color(255,214,0,255),
                                HiddenTint = new SharpDX.Color(255,255,179,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Unique Monster",
                            Config = TreeIconConfigs.Monster,
                            MapIconType = MapIconTypes.UniqueMonster,
                            DefaultSettings = new MapIconSettings {
                                AnimateLife = true,
                                Index = 40,
                                Tint = new SharpDX.Color(255,109,0,255),
                                HiddenTint = new SharpDX.Color(255,218,153,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Rogue Exile",
                            Config = TreeIconConfigs.Monster,
                            MapIconType = MapIconTypes.RogueExile,
                            DefaultSettings = new MapIconSettings {
                                Index = 2,
                                Tint = new SharpDX.Color(255,109,0,255),
                                HiddenTint = new SharpDX.Color(255,218,153,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Giant Rogue Exile",
                            Config = TreeIconConfigs.Monster,
                            MapIconType = MapIconTypes.GiantRogueExile,
                            DefaultSettings = new MapIconSettings {
                                Index = 98,
                                Tint = new SharpDX.Color(255,109,0,255),
                                HiddenTint = new SharpDX.Color(255,218,153,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Spirit",
                            Config = TreeIconConfigs.Monster,
                            MapIconType = MapIconTypes.Spirit,
                            DefaultSettings = new MapIconSettings {
                                Index = 2,
                                Tint = new SharpDX.Color(192,202,51,255),
                                HiddenTint = new SharpDX.Color(233,240,168,255),
                            }
                        },
                    },
                },
                // Delirium Icons
                new TreeCategory {
                    Name = "Delirium Icons",
                    TreeIcons = new List<TreeIcon> {
                        new TreeIcon {
                            Name = "Blood Bag",
                            MapIconType = MapIconTypes.BloodBag,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 74,
                                Tint = new SharpDX.Color(213,0,249,255),
                                HiddenTint = new SharpDX.Color(244,179,255,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Egg Fodder",
                            MapIconType = MapIconTypes.EggFodder,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 74,
                                Tint = new SharpDX.Color(213,0,249,255),
                                HiddenTint = new SharpDX.Color(244,179,255,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Glob Spawn",
                            MapIconType = MapIconTypes.GlobSpawn,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 74,
                                Tint = new SharpDX.Color(213,0,249,255),
                                HiddenTint = new SharpDX.Color(244,179,255,255),
                            }
                        },
                    },
                },
                // Beasts
                new TreeCategory {
                    Name = "Valuable Beasts",
                    TreeIcons = new List<TreeIcon> {
                        new TreeIcon {
                            Name = "Black Morrigan",
                            MapIconType = MapIconTypes.BlackMorrigan,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 194,
                                Tint = new SharpDX.Color(0, 239, 255, 255),
                                HiddenTint = new SharpDX.Color(205, 252, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Vivid Vulture",
                            MapIconType = MapIconTypes.VividVulture,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 193,
                                Tint = new SharpDX.Color(0, 239, 255, 255),
                                HiddenTint = new SharpDX.Color(205, 252, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Craicic Chimeral",
                            MapIconType = MapIconTypes.CraicicChimeral,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 193,
                                Tint = new SharpDX.Color(0, 239, 255, 255),
                                HiddenTint = new SharpDX.Color(205, 252, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Wild Bristle Matron",
                            MapIconType = MapIconTypes.WildBristleMatron,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 192,
                                Tint = new SharpDX.Color(0, 239, 255, 255),
                                HiddenTint = new SharpDX.Color(205, 252, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Wild Hellion Alpha",
                            MapIconType = MapIconTypes.WildHellionAlpha,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 192,
                                Tint = new SharpDX.Color(0, 239, 255, 255),
                                HiddenTint = new SharpDX.Color(205, 252, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Fenumal Plagued Arachnid",
                            MapIconType = MapIconTypes.FenumalPlaguedArachnid,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 192,
                                Tint = new SharpDX.Color(0, 239, 255, 255),
                                HiddenTint = new SharpDX.Color(205, 252, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Fenumus, First of the Night",
                            MapIconType = MapIconTypes.FenumusFirstOfTheNight,
                            Config = TreeIconConfigs.Default,
                            DefaultSettings = new MapIconSettings {
                                Index = 192,
                                Tint = new SharpDX.Color(0, 239, 255, 255),
                                HiddenTint = new SharpDX.Color(205, 252, 255, 255),
                            },
                        },
                    },
                },               
                // Friendly Icons 
                new TreeCategory {
                    Name = "Friendly Icons",
                    TreeIcons = new List<TreeIcon> {
                        new TreeIcon {
                            Name = "Local Player",
                            Config = TreeIconConfigs.Friendly,
                            MapIconType = MapIconTypes.LocalPlayer,
                            DefaultSettings = new MapIconSettings
                            {
                                Draw = false,
                                Tint = new SharpDX.Color(100,221,23,255),
                                HiddenTint = new SharpDX.Color(210,247,186,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Other Player",
                            Config = TreeIconConfigs.Friendly,
                            MapIconType = MapIconTypes.OtherPlayer,
                            DefaultSettings = new MapIconSettings
                            {
                                Draw = false,
                                Tint = new SharpDX.Color(100,221,23,255),
                                HiddenTint = new SharpDX.Color(210,247,186,255),
                            }
                        },
                        new TreeIcon {
                            Name = "NPC",
                            Config = TreeIconConfigs.Friendly,
                            MapIconType = MapIconTypes.NPC,
                            DefaultSettings = new MapIconSettings
                            {
                                Draw = false,
                                Index = 1,
                                Tint = new SharpDX.Color(100,221,23,255),
                                HiddenTint = new SharpDX.Color(210,247,186,255),
                            }
                        },
                        new TreeIcon { 
                            Name = "Minion",
                            Config = TreeIconConfigs.Friendly,
                            MapIconType = MapIconTypes.Minion,
                            DefaultSettings = new MapIconSettings
                            {
                                Draw = false,
                                Tint = new SharpDX.Color(100,221,23,255),
                                HiddenTint = new SharpDX.Color(210,247,186,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Decoy Totem",
                            Config = TreeIconConfigs.Friendly,
                            MapIconType = MapIconTypes.DecoyTotem,
                            DefaultSettings = new MapIconSettings
                            {
                                AnimateLife = true,
                                Index = 16,
                                Tint = new SharpDX.Color(100,221,23,255),
                                HiddenTint = new SharpDX.Color(210,247,186,255), 
                            }
                        },
                    },

                },
                // Chest Icons 
                new TreeCategory {
                    Name = "Chest Icons",
                    TreeIcons = new List<TreeIcon> {
                        new TreeIcon {
                            Name = "Breakable Object",
                            MapIconType = MapIconTypes.BreakableObject,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Draw = false,
                                Index = 360,
                                Tint = new SharpDX.Color(137, 137, 137, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Chest White",
                            MapIconType = MapIconTypes.ChestWhite,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Draw = false,
                                Index = 360,
                                Tint = new SharpDX.Color(255, 255, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Chest Magic",
                            MapIconType = MapIconTypes.ChestMagic,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(0,145,234,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Chest Rare",
                            MapIconType = MapIconTypes.ChestRare,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(255,214,0,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Chest Unique",
                            MapIconType = MapIconTypes.ChestUnique,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(255,109,0,255),
                                HiddenTint = new SharpDX.Color(255,218,153,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Breach Chest Normal",
                            MapIconType = MapIconTypes.BreachChestNormal,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(233, 0, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Breach Chest Large",
                            MapIconType = MapIconTypes.BreachChestLarge,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 361,
                                Tint = new SharpDX.Color(233, 0, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Expedition Chest White",
                            MapIconType = MapIconTypes.ExpeditionChestWhite,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(255, 255, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Expedition Chest Magic",
                            MapIconType = MapIconTypes.ExpeditionChestMagic,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(0,145,234,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Expedition Chest Rare",
                            MapIconType = MapIconTypes.ExpeditionChestRare,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(255,214,0,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Sanctum Chest",
                            MapIconType = MapIconTypes.SanctumChest,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(219, 0, 255, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Pirate Chest",
                            MapIconType = MapIconTypes.PirateChest,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(255,214,0,255),
                            }
                        },
                        new TreeIcon {
                            Name = "Abyss Chest",
                            MapIconType = MapIconTypes.AbyssChest,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(0, 160, 0, 255),
                            }
                        },
                        new TreeIcon {
                            Name = "Sanctum Mote",
                            MapIconType = MapIconTypes.SanctumMote,
                            Config = TreeIconConfigs.Chest,
                            DefaultSettings = new MapIconSettings {
                                Index = 360,
                                Tint = new SharpDX.Color(180, 0, 255, 255),
                            }
                        },
                    },
                },                  
            },
        };

        //--| Helper methods |-----------------------------------------------------------------------------------------------

        private static void IconSizeSliderInt(string id, ref int v, int v_min, int v_max) {
            ImGui.PushItemWidth(100);
            ImGui.SliderInt($"##{id}", ref v, v_min, v_max);
            if (ImGui.IsItemHovered()) {
                ImGui.BeginTooltip();
                ImGui.Text("Icon Size");
                ImGui.EndTooltip();
            }
            ImGui.PopItemWidth();
        }
        private string[] iconDrawStates = Enum.GetNames(typeof(IconDrawStates));
        private bool IngameIconComboBox(string label, ref IconDrawStates selectedState) {
            bool itemChanged = false;
            ImGui.PushItemWidth(100);
            int selectedIndex = (int)selectedState;
            if (ImGui.BeginCombo(label, iconDrawStates[selectedIndex])) {
                for (int i = 0; i < iconDrawStates.Length; i++) {
                    bool isSelected = (selectedIndex == i);
                    if (ImGui.Selectable(iconDrawStates[i], isSelected)) {
                        selectedState = (IconDrawStates)i;
                        itemChanged = true;
                    }
                    if (isSelected) {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            return itemChanged;
        }

        private void DrawCustomPathIcons() {

            for (int i = 0; i < Settings.CustomPathIcons.Count; i++) {
                var setting = Settings.CustomPathIcons[i];

                DT.ImGUITools.Checkbox($"##CustomPath{i}", "Draw Custom Path", ref setting.Draw); ImGui.SameLine();
                DT.ColorSelect.Draw($"ICP{i}","Icon Color", ref setting.Tint); ImGui.SameLine();
                DT.ColorSelect.Draw($"IHCP{i}","Icon Hidden Color", ref setting.HiddenTint); ImGui.SameLine();
                DT.IconSelect.Draw($"CustomPathIcon{i}", "Custom Path Icon", ref setting.Index, Plugin.IconAtlas, new DT.IconSelect.Options { IconColor = setting.Tint.ToImgui() }); ImGui.SameLine();
                IconSizeSliderInt($"##CustomPath{i}", ref setting.Size, 0, 32); ImGui.SameLine();
                DT.ImGUITools.Checkbox($"##CustomPathText{i}", "Draw Text", ref setting.DrawName); ImGui.SameLine();
                DT.ImGUITools.Checkbox($"##CustomPathAlive{i}", "Check if Entity is Alive", ref setting.Check_IsAlive); ImGui.SameLine();
                DT.ImGUITools.Checkbox($"##CustomPathOpened{i}", "Check if Entity is Opened", ref setting.Check_IsOpened); ImGui.SameLine();
                float inputTextWidth = ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("Remove").X - ImGui.GetStyle().ItemSpacing.X;
                ImGui.SetNextItemWidth(inputTextWidth);
                ImGui.InputText($"##Path{i}", ref setting.Path, 100); ImGui.SameLine();
                ImGui.PopItemWidth();
                if (ImGui.Button($"Remove##Path{i}")) Settings.RemoveCustomPathIcon(i);
            }

            if (ImGui.Button("Add Icon")) Settings.NewCustomPathIcon(); ImGui.SameLine();
            if (ImGui.Button("Rebuild Icons")) Plugin.IconBuilder.RebuildIcons();
        }
        
        //--| Draw UI |-----------------------------------------------------------------------------------------------

        private void DrawTree() {

            foreach (var category in UITree.Categories) {

                bool isOpen = Settings.GetCategoryHeaderOpen(category.Name);
                if (DT.ImGUITools.CollapsingHeader(category.Name, ref isOpen)) {

                    ImGui.Indent();
                    foreach (var treeIcon in category.TreeIcons) {
                        var iconSettings = Settings.SetDefaultIconSettings(treeIcon.MapIconType, treeIcon.DefaultSettings);

                        switch (treeIcon.Config) {
                            case TreeIconConfigs.Default:
                                DT.ImGUITools.Checkbox($"##Draw_{treeIcon.Name}", $"Draw {treeIcon.Name}", ref iconSettings.Draw); ImGui.SameLine();
                                DT.ColorSelect.Draw($"IC{treeIcon.Name}", $"Icon Color", ref iconSettings.Tint); ImGui.SameLine();
                                DT.ColorSelect.Draw($"IHC{treeIcon.Name}", $"Icon Hidden Color", ref iconSettings.HiddenTint); ImGui.SameLine();
                                DT.IconSelect.Draw($"Icon{treeIcon.Name}", $"Icon", ref iconSettings.Index, Plugin.IconAtlas, new DT.IconSelect.Options { IconColor = iconSettings.Tint.ToImgui() }); ImGui.SameLine();
                                IconSizeSliderInt($"Icons Slider {treeIcon.Name}", ref iconSettings.Size, 0, 32); ImGui.SameLine();
                                ImGui.Text($"{treeIcon.Name}");
                                break;
                            case TreeIconConfigs.IngameIcon:
                                IngameIconComboBox($"##{treeIcon.Name}", ref iconSettings.DrawState); ImGui.SameLine();
                                DT.ImGUITools.Checkbox($"##checkbox{treeIcon.Name}", $"Show {treeIcon.Name} Name", ref iconSettings.DrawName); ImGui.SameLine();
                                ImGui.Text($"{treeIcon.Name}");
                                break;
                            case TreeIconConfigs.Monster:
                                DT.ImGUITools.Checkbox($"##Draw_{treeIcon.Name}", $"Draw {treeIcon.Name}", ref iconSettings.Draw); ImGui.SameLine();
                                DT.ColorSelect.Draw($"IC{treeIcon.Name}", $"Icon Color", ref iconSettings.Tint); ImGui.SameLine();
                                DT.ColorSelect.Draw($"IHC{treeIcon.Name}", $"Icon Hidden Color", ref iconSettings.HiddenTint); ImGui.SameLine();
                                DT.IconSelect.Draw($"Icon{treeIcon.Name}", $"Icon", ref iconSettings.Index, Plugin.IconAtlas, new DT.IconSelect.Options { IconColor = iconSettings.Tint.ToImgui() }); ImGui.SameLine();
                                DT.ImGUITools.Checkbox($"##Animate_{treeIcon.Name}", $"Animate Icon Health, uses 8 sequential icons to visualise health", ref iconSettings.AnimateLife); ImGui.SameLine();
                                IconSizeSliderInt($"Icons Slider {treeIcon.Name}", ref iconSettings.Size, 0, 32); ImGui.SameLine();
                                ImGui.Text(treeIcon.Name);
                                break;
                            case TreeIconConfigs.Friendly:
                                DT.ImGUITools.Checkbox($"##Draw_{treeIcon.Name}", $"Draw {treeIcon.Name}", ref iconSettings.Draw); ImGui.SameLine();
                                DT.ColorSelect.Draw($"IC{treeIcon.Name}", $"Icon Color", ref iconSettings.Tint); ImGui.SameLine();
                                DT.ColorSelect.Draw($"IHC{treeIcon.Name}", $"Icon Hidden Color", ref iconSettings.HiddenTint); ImGui.SameLine();
                                DT.IconSelect.Draw($"Icon{treeIcon.Name}", $"Icon", ref iconSettings.Index, Plugin.IconAtlas, new DT.IconSelect.Options { IconColor = iconSettings.Tint.ToImgui() }); ImGui.SameLine();
                                DT.ImGUITools.Checkbox($"##Animate_{treeIcon.Name}", $"Animate Icon Health, uses 8 sequential icons to visualise health", ref iconSettings.AnimateLife); ImGui.SameLine();
                                IconSizeSliderInt($"Icons Slider {treeIcon.Name}", ref iconSettings.Size, 0, 32); ImGui.SameLine();
                                DT.ImGUITools.Checkbox($"##checkboxname{treeIcon.Name}", $"Show Name", ref iconSettings.DrawName); ImGui.SameLine();
                                DT.ImGUITools.Checkbox($"##checkboxhealth{treeIcon.Name}", $"Show Health", ref iconSettings.DrawHealth); ImGui.SameLine();
                                ImGui.Text(treeIcon.Name);
                                break;
                            case TreeIconConfigs.Chest:
                                DT.ImGUITools.Checkbox($"##Draw_{treeIcon.Name}", $"Draw {treeIcon.Name}", ref iconSettings.Draw); ImGui.SameLine();
                                DT.ColorSelect.Draw($"IC{treeIcon.Name}",$"Icon Color", ref iconSettings.Tint); ImGui.SameLine();
                                DT.IconSelect.Draw($"Icon{treeIcon.Name}", $"Icon", ref iconSettings.Index, Plugin.IconAtlas, new DT.IconSelect.Options { IconColor = iconSettings.Tint.ToImgui() }); ImGui.SameLine();
                                IconSizeSliderInt($"Icons Slider {treeIcon.Name}", ref iconSettings.Size, 0, 32); ImGui.SameLine();
                                ImGui.Text(treeIcon.Name);
                                break;
                            case TreeIconConfigs.Custom:
                                treeIcon.CustomDrawAction?.Invoke(iconSettings);
                                break;
                        }
                    }
                    ImGui.Unindent();
                }
                Settings.SetCategoryHeader(category.Name, isOpen);
            }
        }

        public void Draw() {

            ImGui.PushItemWidth(100); // Set slider width
            ImGui.SliderInt("Rebuild", ref Settings.RunEveryXTicks, 1, 20);
            if (ImGui.IsItemHovered()) {
                ImGui.BeginTooltip();
                ImGui.Text("Set the interval (in ticks) for rebuilding the icons");
                ImGui.EndTooltip();
            }
            ImGui.SameLine();
            ImGui.SliderInt("ReCache", ref Settings.IconListUpdatePeriod, 10, 1000);
            if (ImGui.IsItemHovered()) {
                ImGui.BeginTooltip();
                ImGui.Text("Set the interval (in milliseconds) for refreshing the icon cache");
                ImGui.EndTooltip();
            }
            ImGui.SameLine();
            ImGui.PopItemWidth(); // Reset slider width
            DT.Button.Draw("ShowDBugger", ref Settings.DBuggerSettings.ShowToolbar, new DT.Button.Options {
                Label = "DBugger",
                Width = 120,
                Height = 22,
            });
            
            if (DT.ImGUITools.CollapsingHeader("Draw Settings", ref Settings.DrawSettingsOpen)) {
                ImGui.Indent();
                DT.ImGUITools.Checkbox("Draw on Minimap", "Draw Monsters on the minimap", ref Settings.DrawOnMinimap);
                DT.ImGUITools.Checkbox("Draw Pixel Perfect Icons", "Enable pixel perfect icons", ref Settings.PixelPerfectIcons);
                DT.ImGUITools.Checkbox("Draw cached Entities", "Draw entities that are cached but no longer in proximity", ref Settings.DrawCachedEntities);
                DT.ImGUITools.Checkbox("Draw Over Large Panels", "Enable drawing over large panels", ref Settings.DrawOverLargePanels);
                DT.ImGUITools.Checkbox("Draw Over Fullscreen Panels", "Enable drawing over fullscreen panels", ref Settings.DrawOverFullscreenPanels);
                ImGui.Unindent();
            }
            if (DT.ImGUITools.CollapsingHeader("Ignored Entities", ref Settings.IgnoredEntitesOpen)) {
                ImGui.Indent();
                if (ImGui.Button("Update")) { Plugin.IconBuilder.UpdateUserSkippedEntities(); }
                ImGui.SameLine();
                ImGui.SliderInt("Height", ref Settings.IgnoredEntitiesHeight, 100, 1000);
                ImGui.InputTextMultiline("##ignoredEntitiesInput", ref Settings.ignoredPaths, 1000, new SVector2(ImGui.GetContentRegionAvail().X, Settings.IgnoredEntitiesHeight));
                ImGui.Unindent();
            };
            if (DT.ImGUITools.CollapsingHeader("Custom Path Icons", ref Settings.CustomIconsOpen)) {
                ImGui.Indent();
                DrawCustomPathIcons();
                ImGui.Unindent();
            };

            DrawTree();

        }

    }
}
