using ExileCore;
using ExileCore.Shared.Cache;
using ImGuiNET;
using System.Drawing;
using System.Numerics;
using DT = DieselTools_ExileAPI;
using DBugger = DieselTools_ExileAPI.DBugger;
using DieselTools_ExileAPI;

namespace MapIcons;

public class Plugin : BaseSettingsPlugin<Settings>
{
    //--| Properties |---------------------------------------------------------------------------------------------------
    private DT.IconAtlas _iconAtlas;
    public DT.IconAtlas IconAtlas => _iconAtlas ??= new(Graphics, "Diesel_MapIcons", Path.Combine(Path.GetDirectoryName(typeof(MapIcon).Assembly.Location), "media", "MapIcons.png"), new Vector2(32, 32));
    
    private IconBuilder _iconBuilder;       
    public IconBuilder IconBuilder => _iconBuilder ??= new IconBuilder(this);

    private IconRenderer _iconRenderer;
    private IconRenderer IconRenderer => _iconRenderer ??= new IconRenderer(this);

    private UserInterface _userInterface;
    private UserInterface UserInterface => _userInterface ??= new UserInterface(this);

    //--| Initialise |--------------------------------------------------------------------------------------------------
    public override bool Initialise() {
        CanUseMultiThreading = true;
        InitializeDBugger();

        IconBuilder.Initialise();
        IconRenderer.Initialise();

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
        IconRenderer.Render();  
        DBugger.Render();
    }

    //--| DBugger |-------------------------------------------------------------------------------------------------------
    private void InitializeDBugger() {
        DBugger.PluginName = Name;
        DBugger.Settings = Settings.DBuggerSettings;
        DBugger.ToolbarOptions = new DT.FloatingToolbar.Options {
            Tools = new List<DT.FloatingToolbar.Tool>{
                    new DT.FloatingToolbar.Label { Text = "MapIcons DBugger" },
                    new DT.FloatingToolbar.Button {
                        Label = "LOG",
                        SetChecked = (bool state) => { Settings.DBuggerSettings.ShowLog = state; },
                        GetChecked = () => Settings.DBuggerSettings.ShowLog,
                    },
                    new DT.FloatingToolbar.Button {
                        Label = "Monitor",
                        SetChecked = (bool state) => { Settings.DBuggerSettings.ShowMonitor = state; },
                        GetChecked = () => Settings.DBuggerSettings.ShowMonitor,
                    },

            },
        };
        DBugger.LogHeader = (width, height) => {
            DT.Button.Draw($"{Name}Friendly", ref Settings.DebugFriendlyIcon, new DT.Button.Options { Label = "Friendly", Width = 80, Height = 22 }); ImGui.SameLine();
            DT.Button.Draw($"{Name}Monster", ref Settings.DebugMonsterIcon, new DT.Button.Options { Label = "Monster", Width = 80, Height = 22 }); ImGui.SameLine();
            DT.Button.Draw($"{Name}Chest", ref Settings.DebugChestIcon, new DT.Button.Options { Label = "Chest", Width = 80, Height = 22 }); ImGui.SameLine();
            DT.Button.Draw($"{Name}Ingame", ref Settings.DebugMinimapIcon, new DT.Button.Options { Label = "Ingame", Width = 80, Height = 22 }); ImGui.SameLine();
            DT.Button.Draw($"{Name}Misc", ref Settings.DebugMiscIcon, new DT.Button.Options { Label = "Misc", Width = 80, Height = 22 }); ImGui.SameLine();
            DT.Button.Draw($"{Name}User", ref Settings.DebugUser, new DT.Button.Options { Label = "User", Width = 80, Height = 22 }); ImGui.SameLine();
            if (DT.Button.Draw($"{Name}RebuildIcons", new DT.Button.Options { Label = "Rebuild", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions("Rebuild Icons") })) {
                IconBuilder.RebuildIcons();
            }
        };
        DBugger.Log($"DBugger for {Name} initialized", false);
    }

}