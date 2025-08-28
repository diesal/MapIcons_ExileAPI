using ImGuiNET;
using System.Runtime.CompilerServices;
using System.Text;
using SVector2 = System.Numerics.Vector2;
using DXColor = SharpDX.Color;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Cache;

namespace DieselTools_ExileAPI;


public class DBuggerSettings
{
    public bool ShowLog = false; 
    public bool ShowToolbar = false; 
    public bool ShowMonitor = false;
    public bool ShowPanelDebug = false;

    public int MaxLogEntries = 500; // Maximum number of log entries to keep

    public Dictionary<string, bool> CategoryPanelsCollapsed = new Dictionary<string, bool>();

    public DXColor[] buttonColors = new DXColor[]{
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
        new DXColor(Colors.Button),
    };

}


public static class DBugger
{
    private static class _Logger {
        private const uint BUFFER_SIZE = (uint)65536; // 64 KB buffer size
        private static readonly LinkedList<(DateTime Date, string Description, int Count)> _logEntries = new();
        private static string _logText = string.Empty;
        private static string _lastEntry = string.Empty;
        private static int _repeatCount = 1;

        public static void AddLogEntry(string entry) {
            if (_logEntries.Count > 0 && entry == _logEntries.Last.Value.Description) {
                var last = _logEntries.Last.Value;
                _logEntries.RemoveLast();
                _logEntries.AddLast((DateTime.Now, entry, last.Count + 1));
            }
            else {
                _repeatCount = 1;
                _lastEntry = entry;
                _logEntries.AddLast((DateTime.Now, entry, 1));
            }
            while (_logEntries.Count > Settings.MaxLogEntries)
                _logEntries.RemoveFirst();
        }

        public static void Clear() {
            _logEntries.Clear();
            _lastEntry = null;
            _repeatCount = 1;
        }
        public static void Render(bool newestFirst = false) {
            if (Window.Begin($"{PluginName}Log", ref Settings.ShowLog, new Window.Options { Title = $"{PluginName} DBugger Log", Resizable = true })) {
                var contentPos = ImGui.GetCursorScreenPos();
                ImGui.Indent(2);

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new SVector2(1, 0));
                if (Button.Draw($"{PluginName}LogClear", new Button.Options { Label = "Clear", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions("Clear Log History") })) {
                    Clear();
                }
                ImGui.SameLine();
                LogHeader?.Invoke(80, 22);
                ImGui.PopStyleVar();

                ImGui.Dummy(new SVector2(0, 2));
                ImGui.Indent(1);

                var panelName = $"{PluginName}LogPanel";
                if (Panel.Begin(panelName, new Panel.Options { Width = -3, Height = -3 })) {
                
                    var logString = new StringBuilder((int)BUFFER_SIZE);
                    var entries = newestFirst ? _logEntries.Reverse() : _logEntries;
                    foreach (var (dateTime, message, count) in entries) {
                        var display = count > 1 ? $"[{count}] {message}" : message;
                        logString.AppendLine($"{dateTime:HH:mm:ss.fff}: {display}");
                    }
                    string logText = logString.ToString();
                    var avail = ImGui.GetContentRegionAvail();
                    ImGui.InputTextMultiline($"{PluginName}LogText", ref logText, BUFFER_SIZE, new SVector2(avail.X -4, avail.Y-4), ImGuiInputTextFlags.ReadOnly);
                
                    Panel.End(panelName);
                }

                ImGui.Unindent(3);
                Window.End();
            }

        }
    }
    private static class _Monitor
    {
        private static SortedDictionary<string, SortedDictionary<string, MonitoredVariable>> monitoredVariables = new SortedDictionary<string, SortedDictionary<string, MonitoredVariable>>();
        private class MonitoredVariable
        {
            public DateTime DateUpdated;
            public string Category;
            public string Name;
            public object Value;
            public string FilePath;
            public string? FileName;
            public string? ShortFilePath;
            public int Line;
            public string Member;
            public List<(DateTime Time, object Value)> ChangeHistory = new(); // Store up to 10 changes
        }

        public static void AddMonitoredVariable(string category, string name, object variable, string filePath, int line, string member) {
            // ensure setting exists for categories collapsing panel
            if (!Settings.CategoryPanelsCollapsed.ContainsKey(category)) Settings.CategoryPanelsCollapsed[category] = false;

            // Get or create category dictionary
            if (!monitoredVariables.TryGetValue(category, out var categoryDict)) {
                categoryDict = new SortedDictionary<string, MonitoredVariable>();
                monitoredVariables[category] = categoryDict;
            }

            // Get or create monitored variable
            if (!categoryDict.TryGetValue(name, out var monitored)) {
                string shortPath = null, fileName = null;
                if (!string.IsNullOrEmpty(filePath)) {
                    string directory = Path.GetDirectoryName(filePath);
                    string lastDir = directory?.Split(Path.DirectorySeparatorChar).Last();
                    fileName = Path.GetFileName(filePath);
                    shortPath = lastDir != null ? $"{lastDir}{Path.DirectorySeparatorChar}{fileName}" : fileName;
                }

                monitored = new MonitoredVariable {
                    Category = category,
                    Name = name,
                    Value = variable,
                    DateUpdated = DateTime.Now,
                    FilePath = filePath,
                    Line = line,
                    Member = member,
                    ShortFilePath = shortPath,
                    FileName = fileName,
                    ChangeHistory = new List<(DateTime, object)>()
                };
                categoryDict[name] = monitored;
                return;
            }

            // Track changes
            bool changed = monitored.Value == null ? variable != null : !Equals(monitored.Value, variable);
            if (changed) {
                // Add previous value to history
                if (monitored.Value != null) {
                    monitored.ChangeHistory.Add((monitored.DateUpdated, monitored.Value));
                    if (monitored.ChangeHistory.Count > 10)
                        monitored.ChangeHistory.RemoveAt(0); // Keep only last 10
                }
                monitored.DateUpdated = DateTime.Now;
            }

            monitored.Value = variable;
        }

        private static uint white = Colors.ControlText;
        private static uint red = Palettes.GetMaterialColor("Red A700");
        private static uint yellow = Palettes.GetMaterialColor("Yellow A700");
        private static uint orange = Palettes.GetMaterialColor("Orange A700");
        private static uint green = Palettes.GetMaterialColor("Green A700");
        private static uint lightGreen = Palettes.GetMaterialColor("Light Green A700");
        private static uint lightBlue = Palettes.GetMaterialColor("Light Blue A700");
        private static uint purple = Palettes.GetMaterialColor("Purple A700");
        private static (uint Color, string Value) FormatVariable(object variable) {
            if (variable == null) return (red, "null"); // Use a distinct color for nulls if you want

            switch (variable) {
                case int i:
                    return (yellow, i.ToString());
                case float f:
                    return (yellow, f.ToString());
                case double d:
                    return (yellow, d.ToString());
                case bool b:
                    return (purple, b.ToString());
                case Enum e:
                    return (purple, e.ToString());
                case string s:
                    if (string.IsNullOrWhiteSpace(s)) return (red, "empty string");
                    return (lightGreen, s);
                case IEnumerable<object> list:
                    return (white, $"List[{list.Count()}]");
                case DateTime dt:
                    return (lightBlue, dt.ToString("yyyy-MM-dd HH:mm:ss"));
                default:
                    return (white, variable.ToString());
            }
        }

        private static int windowHeight = 400;
        public static void RenderTest() {
            if (true) return;
            // Basic types
            String s = "t";
            Monitor("Basic", "Bool", true);
            Monitor("Basic", "Int", _int++);
            Monitor("Basic", "Float", 0.016f);
            Monitor("Basic", "String", s);

            Monitor("Edge", "NullValue", null);
            Monitor("Edge", "EmptyString", "");
            Monitor("Edge", "NegativeInt", -42);
            Monitor("Edge", "LargeFloat", 1e10f);

            // Complex types
            Monitor("Complex", "Position", new SVector2(100, 200));
            Monitor("Complex", "Inventory", new List<string> { "Sword", "Shield" });
            Monitor("Complex", "Settings", Settings);
        }
        public static void Render(bool newestFirst = false) {
            RenderTest();
            if (Window.Begin($"{PluginName}DBuggerMonitor", ref Settings.ShowMonitor, new Window.Options { Title = $"{PluginName} DBugger Monitor", Resizable = true, MinWidth = 200, LockHeight = windowHeight })) {
                windowHeight = 20; // titel bar
                var startinPos = ImGui.GetCursorScreenPos();
                ImGui.Indent(2);

                foreach (var categoryPair in monitoredVariables) {
                    bool isCollapsed = Settings.CategoryPanelsCollapsed[categoryPair.Key];
                    var panelName = $"{PluginName}DBuggerMonitor{categoryPair.Key}Panel";
                    if (CollapsingPanel.Begin(panelName, ref isCollapsed, new CollapsingPanel.Options { Label = $"{categoryPair.Key}", Width = -3, Padding = new(4,4,4,4)  })) {
                        var first = true;
                        foreach (var variablePair in categoryPair.Value) {
                            if (!first) ImGui.Dummy(new SVector2(0, 3));

                            var monitoredVariable = variablePair.Value;
                            var avail = ImGui.GetContentRegionAvail();
                            var inputWidth = (int)(avail.X * 0.6f); // float is percent of available width 0.7f = 70%
                            var textWidth = avail.X - inputWidth;

                            Display.Draw($"{panelName}{variablePair.Key}_name", monitoredVariable.Name, new Display.Options { DrawBackground = false, Width = (int)textWidth, Height = 20 });
                            ImGui.SameLine();
                            var (color, value) = FormatVariable(monitoredVariable.Value);
                            Display.Draw($"{panelName}{variablePair.Key}_var", value, new Display.Options { Width = inputWidth - 3, Height = 20, TextColor=color });
                            if (ImGui.IsItemHovered()) {
                                if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
                                    ImGui.SetClipboardText(value.ToString());
                                }

                                var tooltipLines = new List<Tooltip.Line> {
                                    new Tooltip.DoubleLine { LeftText = monitoredVariable.Name , LeftColor = Colors.ControlText, RightText = monitoredVariable.Value != null ? monitoredVariable.Value.GetType().ToString() : "null" },
                                    new Tooltip.Separator { },
                                    new Tooltip.DoubleLine { LeftText = "Updated:", RightText = $"{monitoredVariable.DateUpdated:HH:mm:ss.fff}" },
                                    new Tooltip.DoubleLine { LeftText = "File:", RightText = $"{monitoredVariable.ShortFilePath ?? monitoredVariable.FileName ?? monitoredVariable.FilePath}" },
                                    new Tooltip.DoubleLine { LeftText = "Line:", RightText =  $"{monitoredVariable.Line}" },
                                    new Tooltip.DoubleLine { LeftText = "Member:", RightText =  $"{monitoredVariable.Member}" },
                                    new Tooltip.Separator { }
                                };
                                // Add change history if available
                                if (monitoredVariable.ChangeHistory.Count > 0) {
                                    foreach (var (time, val) in monitoredVariable.ChangeHistory) {
                                        var (c, s) = FormatVariable(val);
                                        tooltipLines.Add(new Tooltip.DoubleLine {
                                            LeftText = time.ToString("HH:mm:ss.fff"),
                                            RightText = s
                                        });
                                    }
                                    tooltipLines.Add(new Tooltip.Separator { });
                                }
                                tooltipLines.Add(new Tooltip.DoubleLine { LeftText = "Ctrl + LeftClick:", RightText = "Copy to clipboard" });

                                Tooltip.Draw(new Tooltip.Options { Lines = tooltipLines });

                            }
                            first = false;
                        }
                        CollapsingPanel.End(panelName);
                    }
                    ImGui.Dummy(new SVector2(0, 3)); // panel bottom spacing

                    Settings.CategoryPanelsCollapsed[categoryPair.Key] = isCollapsed;
                }

                ImGui.Unindent(2);
                windowHeight += (int)(ImGui.GetCursorScreenPos().Y - startinPos.Y);
                Window.End();
            }
        }
    }
    private static class _PanelDebug {
        private static bool testCollapsed = false;
        private static bool testCollapsed1 = false;
        private static bool testCollapsed2 = true;
        private static bool testCollapsed3 = false;

        private static int windowHeight = 300;
        private static string collapsingPanel1_Size;
        private static string collapsingPanel2_Size;
        private static string collapsingPanel3_Size;
        private static string collapsingPanel4_Size;

        public static void Render( ) {

            if (Window.Begin($"{PluginName}WindowPanelDebug", ref Settings.ShowPanelDebug, new Window.Options { Title = $"Panels Debug", Resizable = true })) {

                // side by side panel example
                var panelOptions = new Panel.Options { Debug = true };
                var panelName = $"{PluginName}PaenlDebug1";
                if (Panel.Begin(panelName, panelOptions)) {
                    ImGui.Text($"Width={panelOptions.Width}, Height={panelOptions.Height}");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    Panel.End(panelName);
                }

                ImGui.SameLine();
                panelOptions = new Panel.Options { Debug = true };
                panelName = $"{PluginName}PaenlDebug2";
                if (Panel.Begin(panelName, panelOptions)) {
                    ImGui.Text($"Width={panelOptions.Width}, Height={panelOptions.Height}");
                    ImGui.Text($"ImGui.SameLine()");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    Panel.End(panelName);
                }

                panelOptions = new Panel.Options { Debug = true };
                panelName = $"{PluginName}PaenlDebug3";
                if (Panel.Begin(panelName, panelOptions)) {
                    ImGui.Text($"Width={panelOptions.Width}, Height={panelOptions.Height}");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    Panel.End(panelName);
                }

                panelOptions = new Panel.Options { Width = 200, Height = 150, Debug = true };
                panelName = $"{PluginName}PaenlDebug4";
                if (Panel.Begin(panelName, panelOptions)) {
                    ImGui.Text($"Width={panelOptions.Width}, Height={panelOptions.Height}");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    Panel.End(panelName);
                }

                ImGui.SameLine();
                panelOptions = new Panel.Options { Width = 200, Height = 150, Debug = true };
                panelName = $"{PluginName}PaenlDebug5";
                if (Panel.Begin(panelName, panelOptions)) {
                    ImGui.Text($"Width={panelOptions.Width}, Height={panelOptions.Height}");
                    ImGui.Text($"ImGui.SameLine()");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    Panel.End(panelName);
                }

                panelOptions = new Panel.Options { Width = 200, Height = 150, Debug = true };
                panelName = $"{PluginName}PaenlDebug6";
                if (Panel.Begin(panelName, panelOptions)) {
                    ImGui.Text($"Width={panelOptions.Width}, Height={panelOptions.Height}");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    Panel.End(panelName);
                }

                panelOptions = new Panel.Options { Width = -10, Height = 150, Debug = true };
                panelName = $"{PluginName}PaenlDebug7";
                if (Panel.Begin(panelName, panelOptions)) {
                    ImGui.Text($"Width={panelOptions.Width}, Height={panelOptions.Height}");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    Panel.End(panelName);
                }

                panelOptions = new Panel.Options { Width = -10, Height = -10, Debug = true };
                panelName = $"{PluginName}PaenlDebug8";
                if (Panel.Begin(panelName, panelOptions)) {
                    ImGui.Text($"Width={panelOptions.Width}, Height={panelOptions.Height}");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    Panel.End(panelName);
                }

                Window.End();
            }

            if (Window.Begin($"Filled", ref Settings.ShowPanelDebug, new Window.Options { Title = $"Filled Panel Debug, MinWidth=400, MinHeigt=200", MinWidth=400, MinHeight=200, Resizable = true })) {

                var panelName = $"{PluginName}FilledPanel";
                if (Panel.Begin(panelName, new Panel.Options { Width=0, Height=0})) {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new SVector2(0, 3));
                    for (int i = 0; i < 10; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label =$"Button {i}", Color = Settings.buttonColors[i].ToImgui(), Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                        ImGui.SameLine();
                        ColorSelect.Draw($"{panelName}button{i}_Swatch", $"Button {i}", ref Settings.buttonColors[i], new ColorSelect.Options { Size = new(22,22)}  );
                    }
                    Panel.End(panelName);
                    ImGui.PopStyleVar();
                }

                Window.End();
            }

            if (Window.Begin($"Collapsing", ref Settings.ShowPanelDebug, new Window.Options { Title = $"Collapsing Panel Debug", Resizable = true, LockHeight = windowHeight })) {
                windowHeight = 20; // titel bar
                var startinPos = ImGui.GetCursorScreenPos();

                var panelOptions = new CollapsingPanel.Options { Label = $"Width=0 Height=100 {collapsingPanel1_Size}", Width = 0, Height = 100, Debug=true };
                var panelName = $"{PluginName}CollapsingPanel1";
                if (CollapsingPanel.Begin(panelName, ref testCollapsed, panelOptions)) {
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    collapsingPanel1_Size = ImGui.GetWindowSize().ToString();
                    CollapsingPanel.End(panelName);
                }

                panelOptions = new CollapsingPanel.Options { Label = $"Width=0 Height=100 HeaderWidth=400 {collapsingPanel2_Size}", Width = 0, Height = 100, HeaderWidth=400,Debug = true };
                panelName = $"{PluginName}CollapsingPanel2";
                if (CollapsingPanel.Begin(panelName, ref testCollapsed1, panelOptions)) {
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    collapsingPanel2_Size = ImGui.GetWindowSize().ToString();
                    CollapsingPanel.End(panelName);
                }

                panelOptions = new CollapsingPanel.Options { Label = $"Width=null Height=null {collapsingPanel3_Size}", Debug = true };
                panelName = $"{PluginName}CollapsingPanel3";
                if (CollapsingPanel.Begin(panelName, ref testCollapsed2, panelOptions)) {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new SVector2(0, 3));
                    ImGui.Text("ItemSpacing=(0,3)");
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    ImGui.PopStyleVar();
                    collapsingPanel3_Size = ImGui.GetWindowSize().ToString();
                    CollapsingPanel.End(panelName);
                }

                panelOptions = new CollapsingPanel.Options { Label = $"Width=0 Height=null {collapsingPanel4_Size}", Width = 0, Debug = true };
                panelName = $"{PluginName}CollapsingPanel4";
                if (CollapsingPanel.Begin(panelName, ref testCollapsed3, panelOptions)) {
                    for (int i = 0; i < 4; i++) {
                        Button.Draw($"{panelName}button{i}", new Button.Options { Label = $"Button {i}", Width = 80, Height = 22, Tooltip = Tooltip.BasicOptions($"Button {i} Test") });
                    }
                    collapsingPanel4_Size = ImGui.GetWindowSize().ToString();
                    CollapsingPanel.End(panelName);
                }
                windowHeight += (int)(ImGui.GetCursorScreenPos().Y - startinPos.Y) ;


                Window.End();

            }

        }
    }




    public static string PluginName { get; set; } = "place_holder";
    public static DBuggerSettings Settings { get; set; } = new();

    public static FloatingToolbar.Options ToolbarOptions = new();

    public static Action<int, int > LogHeader;

    public static void Log(string message, bool whenVisibleOnly = true) {
        if (!whenVisibleOnly || Settings.ShowLog) {
            _Logger.AddLogEntry(message);
        }
    }
    public static void ClearLog() {
        _Logger.Clear();
    }


    public static void Monitor(string category, string name, object variable, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0, [CallerMemberName] string member = "") {
        if (!Settings.ShowMonitor) return;
        _Monitor.AddMonitoredVariable(category, name, variable, file, line, member);
    }


    private static int _int = 0; 

    public static void Render() {
        if (Settings.ShowToolbar) FloatingToolbar.Draw($"{PluginName}DBuggerToolbar", ToolbarOptions);

        _Monitor.Render();
        _Logger.Render();
        _PanelDebug.Render();
    }
}
