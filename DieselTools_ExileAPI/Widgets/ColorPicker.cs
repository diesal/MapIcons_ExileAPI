using ImGuiNET;
using System.Text.RegularExpressions;
using SVector2 = System.Numerics.Vector2;
using SVector4 = System.Numerics.Vector4;

namespace DieselTools_ExileAPI;

public static class ColorPicker {
    private const int ControlHeight = 20;
    private static readonly SVector2 ContentPadding = new SVector2(10, 10);
    private static readonly SVector2 ControlSpacing = new SVector2(8, 8);
    private static readonly SVector2 InputSpacing = new SVector2(5, 5);
    private static readonly SVector2 SliderSize = new(362,ControlHeight);
    private static readonly SVector2 SliderInputSize = new(45,ControlHeight);
    private static readonly SVector2 DefaultWindowOffset = new SVector2(10, 10);
    private static Dictionary<string, List<Palettes.Swatch>> MaterialNo50 =>
    Palettes.Material.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value.Where(swatch => !swatch.Name.EndsWith(" 50")).ToList()
    );

    private static SVector4 default_rgbaNormalized;
    private static HSLA working_HSLA;







    /**

float oldH = _hue, oldS = _sat, oldL = _light, oldA = _alpha;
DrawHueSlider(contentPos + new SVector2(10, 10), ref _hue, _sat / 100f, _light / 100f);
DrawSaturationSlider(contentPos + new SVector2(10, 10 + SliderHeight + 10), ref _sat, _hue, _light / 100f);
DrawLightnessSlider(contentPos + new SVector2(10, 10 + 2 * (SliderHeight + 10)), ref _light, _hue, _sat / 100f);
DrawAlphaSlider(contentPos + new SVector2(10, 10 + 3 * (SliderHeight + 10)), ref _alpha, _hue, _sat / 100f, _light / 100f);
if (_hue != oldH || _sat != oldS || _light != oldL || _alpha != oldA) {
color = ColorTools.HSLA2RGBA(_hue, _sat / 100f, _light / 100f, _alpha / 100f);
}

SVector4 oldColor = color;
int colorInputWidth = 40;
int ColorInputGap = 6;
int hexInputWidth = colorInputWidth * 3 + ColorInputGap * 2;
DrawHexColorInput(contentPos + new SVector2(10, 126), hexInputWidth, ref color);
DrawRedColorInput(contentPos + new SVector2(10, 151), colorInputWidth, ref color);
DrawGreenColorInput(contentPos + new SVector2(10 + colorInputWidth + ColorInputGap, 151), colorInputWidth, ref color);
DrawBlueColorInput(contentPos + new SVector2(10 + colorInputWidth * 2 + ColorInputGap * 2, 151), colorInputWidth, ref color);
DrawRGBAInput(contentPos + new SVector2(10 + hexInputWidth + 10, 126), 218, ref color);
DrawColorWithReset(contentPos + new SVector2(380, 126), 45, 44, ref color, _defaultColor);

if (!color.Equals(oldColor)) {
ColorTools.RGBA2HSLA(color, out _hue, out _sat, out _light, out _alpha);
_sat *= 100f;
_light *= 100f;
_alpha *= 100f;
}
**/

    public class Options
    {
        public string Title = "Color Picker";
        public SVector2 WindowOffset = DefaultWindowOffset;
        public SVector2 Size = new(441, 395);
    }

    public static void Open(string unique_id, SVector4 rgbaNormalized, SVector2? windowOffset) {
        default_rgbaNormalized = rgbaNormalized;
        working_HSLA = HSLA.FromNormalisedRGBA(rgbaNormalized);
        PopupWindow.Open(unique_id, windowOffset ?? DefaultWindowOffset);
    }

    public static void Draw(string unique_id, ref SVector4 rgbaNormalized, Options options = null) {
        // Initialize default values if not set
        if (options == null) options = new Options();

        var working_RGBA = rgbaNormalized;

        PopupWindow.Draw(unique_id, new PopupWindow.Options { Size = options.Size, Title = options.Title, PanelPadding = new(0,3,200,3) }, (contentPos) => {
            var old_HSLA = working_HSLA;
            SVector2 layoutPos = contentPos + ContentPadding;

            DrawHueSlider(layoutPos, ref working_HSLA);
            layoutPos.Y += ControlHeight + ControlSpacing.Y;
            DrawSaturationSlider(layoutPos, ref working_HSLA);
            layoutPos.Y += ControlHeight + ControlSpacing.Y;
            DrawLightnessSlider(layoutPos, ref working_HSLA);
            layoutPos.Y += ControlHeight + ControlSpacing.Y;
            DrawAlphaSlider(layoutPos, ref working_HSLA);
            if (old_HSLA != working_HSLA) working_RGBA = working_HSLA.ToNormalisedRGBA();

            SVector4 old_RGBA = working_RGBA;
            int red = (int)Math.Round(working_RGBA.X * 255);
            int green = (int)Math.Round(working_RGBA.Y * 255);
            int blue = (int)Math.Round(working_RGBA.Z * 255);

            // Draw hexinput
            layoutPos.Y += ControlHeight + ControlSpacing.Y;
            int hexInputWidth = (int)(SliderInputSize.X * 3 + InputSpacing.X * 2);
            DrawHexColorInput(layoutPos, hexInputWidth, ref working_RGBA);
            //Draw RGBA Input
            layoutPos.X += hexInputWidth + ControlSpacing.X;
            int rgbaInputWidth = (int)(SliderSize.X - hexInputWidth - ControlSpacing.X);
            DrawRGBAInput(layoutPos, rgbaInputWidth, ref working_RGBA);
            // Draw Color Swatch with Reset Button
            layoutPos.X += ControlSpacing.X + rgbaInputWidth;
            DrawColorWithReset(layoutPos, (int)SliderInputSize.X, (int)SliderInputSize.X, ref working_RGBA);

            // Draw Red Input
            layoutPos.X = contentPos.X + ContentPadding.X; // Reset X position for inputs
            layoutPos.Y += ControlHeight + InputSpacing.Y;
            ImGui.SetCursorScreenPos(layoutPos);
            if (InputSpinner.Draw("ColorPickerRed", ref red, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y, Max = 255, Tooltip = Tooltip.BasicOptions("Red Component") }))
                working_RGBA.X = Math.Clamp(red, 0, 255) / 255f;
            // Draw Green Input
            layoutPos.X += SliderInputSize.X + InputSpacing.X;
            ImGui.SetCursorScreenPos(layoutPos);
            if (InputSpinner.Draw("ColorPickerGreen", ref green, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y, Max = 255, Tooltip = Tooltip.BasicOptions("Green Component") }))
                working_RGBA.Y = Math.Clamp(green, 0, 255) / 255f;
            // Draw Blue Input
            layoutPos.X += SliderInputSize.X + InputSpacing.X;
            ImGui.SetCursorScreenPos(layoutPos);
            if (InputSpinner.Draw("ColorPickerBlue", ref blue, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y, Max = 255, Tooltip = Tooltip.BasicOptions("Blue Component") }))
                working_RGBA.Z = Math.Clamp(blue, 0, 255) / 255f;

            // Draw Palette 
            layoutPos.X = contentPos.X - 1 ; // Reset X position for palette
            layoutPos.Y += ControlHeight + 12;
            ImGui.SetCursorScreenPos(layoutPos);
            DrawPalette( MaterialNo50, 437, 16, new SVector2(-1,-1), ref working_RGBA);



            if (old_RGBA != working_RGBA) 
                working_HSLA = HSLA.FromNormalisedRGBA(working_RGBA);

        });
        rgbaNormalized = working_RGBA;
    }

    //private static void DrawGreenColorInput(SVector2 inputPos, int inputWidth, ref SVector4 color) {
    //    // Custom draw background and borders
    //    var drawList = ImGui.GetWindowDrawList();
    //    drawList.AddRectFilled(inputPos, inputPos + new SVector2(inputWidth, ControlHeight), UIColors.ControlInput);
    //    drawList.AddRect(inputPos - new SVector2(1, 1), inputPos + new SVector2(inputWidth + 1, ControlHeight + 1), UIColors.Black);
    //    drawList.AddRect(inputPos - new SVector2(2, 2), inputPos + new SVector2(inputWidth + 2, ControlHeight + 2), UIColors.ControlBorder);
    //
    //    int g = (int)Math.Round(color.Y * 255);
    //    string gBuf = g.ToString();
    //
    //    ImGui.PushStyleColor(ImGuiCol.FrameBg, 0x00000000);
    //    ImGui.PushStyleColor(ImGuiCol.Border, 0x00000000);
    //    ImGui.SetCursorScreenPos(inputPos + new SVector2(4, 0));
    //    ImGui.PushItemWidth(inputWidth - 8);
    //
    //    bool valueChanged = false;
    //    if (ImGui.InputText("##ColorPickergreeninput", ref gBuf, 4, ImGuiInputTextFlags.CharsDecimal))
    //        valueChanged = true;
    //
    //    if (ImGui.IsItemHovered()) {
    //        float wheel = ImGui.GetIO().MouseWheel;
    //        if (wheel != 0) {
    //            int newG;
    //            if (!int.TryParse(gBuf, out newG)) newG = g;
    //            newG += Math.Sign(wheel);
    //            newG = Math.Clamp(newG, 0, 255);
    //            gBuf = newG.ToString();
    //            valueChanged = true;
    //        }
    //        // Tooltip
    //        Tooltip.Draw(new Tooltip.Options {
    //            Lines = {
    //            new Tooltip.Title { Text = "Green Component" },
    //        }
    //        });
    //    }
    //
    //    if (valueChanged) {
    //        if (int.TryParse(gBuf, out int newG)) {
    //            color.Y = Math.Clamp(newG, 0, 255) / 255f;
    //        }
    //    }
    //
    //    ImGui.PopItemWidth();
    //    ImGui.PopStyleColor(2);
    //}



    private static void DrawHueSlider(SVector2 pos, ref HSLA working_HSLA) {
        var drawList = ImGui.GetWindowDrawList();
        // Draw border
        drawList.AddRect(pos - new SVector2(1, 1), pos + SliderSize + new SVector2(1, 1), Colors.ControlOuterBorder); // Outer border
        drawList.AddRect(pos, pos + SliderSize, Colors.Black); // Black Border
        // Draw Slider
        SVector2 sliderPos = pos + new SVector2(1, 1);
        var sliderBarSize = SliderSize - new SVector2(2, 2); // Inner area for gradient
        int segments = 180;
        float segmentWidth = sliderBarSize.X / segments;
        // Draw hue gradient using current s, l, a
        for (int i = 0; i < segments; i++) {
            float hueVal = i * 360f / segments;
            var new_hsla = working_HSLA;
            new_hsla.H = hueVal;
            new_hsla.A = 1.0f;
            uint col = new_hsla.ToImGuiColor();
            SVector2 p0 = sliderPos + new SVector2(i * segmentWidth, 0);
            SVector2 p1 = sliderPos + new SVector2((i + 1) * segmentWidth, sliderBarSize.Y);
            drawList.AddRectFilled(p0, p1, col);
        }
        // Draw slider handle
        float handleX = (float)Math.Round(sliderPos.X + (working_HSLA.H / 360f) * sliderBarSize.X);
        SVector2 handleTopLeft = new SVector2(handleX - 1, sliderPos.Y - 1);
        SVector2 handleBottomRight = new SVector2(handleX + 1, sliderPos.Y + sliderBarSize.Y + 1);
        drawList.AddRectFilled(handleTopLeft, handleBottomRight, 0xFFFFFFFF);
        drawList.AddRect(handleTopLeft - new SVector2(1, 1), handleBottomRight + new SVector2(1, 1), Colors.Black);

        // Mouse interaction
        ImGui.SetCursorScreenPos(pos);
        ImGui.InvisibleButton("##hueslidergrip", SliderSize);
        if (ImGui.IsItemActive() && ImGui.IsMouseDown(0)) {
            float mouseX = ImGui.GetIO().MousePos.X;
            float relX = Math.Clamp(mouseX - sliderPos.X, 0, sliderBarSize.X);
            working_HSLA.H = relX / sliderBarSize.X * 360f;
        }
        // Mouse wheel
        if (ImGui.IsItemHovered()) {
            float wheel = ImGui.GetIO().MouseWheel;
            if (wheel != 0) {
                working_HSLA.H = working_HSLA.H + wheel;
            }
            Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Hue Slider" }, } });
        }

        // Draw Input box
        SVector2 inputPos = pos + new SVector2(SliderSize.X + ControlSpacing.X, 0);
        ImGui.SetCursorScreenPos(inputPos);
        int value = (int)Math.Round(working_HSLA.H);
        if (InputSpinner.Draw("ColorPickerHueSlider", ref value, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y,  Max = 360 }))
            working_HSLA.H = value;
    }
    private static void DrawSaturationSlider(SVector2 pos, ref HSLA working_HSLA) {
        var drawList = ImGui.GetWindowDrawList();
        // Draw border
        drawList.AddRect(pos - new SVector2(1, 1), pos + SliderSize + new SVector2(1, 1), Colors.ControlOuterBorder); // Outer border
        drawList.AddRect(pos, pos + SliderSize, Colors.Black); // Black Border
        // Draw Slider
        SVector2 sliderPos = pos + new SVector2(1, 1);
        var sliderBarSize = SliderSize - new SVector2(2, 2); // Inner area for gradient
        int segments = 100;
        float segmentWidth = sliderBarSize.X / segments;
        // Draw saturation gradient using current hue and lightness
        for (int i = 0; i < segments; i++) {
            var new_hsla = working_HSLA;
            new_hsla.A = 1.0f;
            new_hsla.S = i / (float)(segments - 1); // normalized [0,1]
            var col = new_hsla.ToImGuiColor();

            SVector2 p0 = sliderPos + new SVector2(i * segmentWidth, 0);
            SVector2 p1 = sliderPos + new SVector2((i + 1) * segmentWidth, sliderBarSize.Y);
            drawList.AddRectFilled(p0, p1, col);
        }
        // Draw slider handle
        float handleX = (float)Math.Round(sliderPos.X + working_HSLA.S * sliderBarSize.X);
        SVector2 handleTopLeft = new SVector2(handleX - 1, sliderPos.Y - 1);
        SVector2 handleBottomRight = new SVector2(handleX + 1, sliderPos.Y + sliderBarSize.Y + 1);
        drawList.AddRectFilled(handleTopLeft, handleBottomRight, 0xFFFFFFFF);
        drawList.AddRect(handleTopLeft - new SVector2(1, 1), handleBottomRight + new SVector2(1, 1), Colors.Black);

        // Mouse interaction
        ImGui.SetCursorScreenPos(pos);
        ImGui.InvisibleButton("##satslidergrip", SliderSize);
        if (ImGui.IsItemActive() && ImGui.IsMouseDown(0)) {
            float mouseX = ImGui.GetIO().MousePos.X;
            float relX = Math.Clamp(mouseX - sliderPos.X, 0, sliderBarSize.X);
            working_HSLA.S = relX / sliderBarSize.X;
        }
        // Mouse wheel
        if (ImGui.IsItemHovered()) {
            float wheel = ImGui.GetIO().MouseWheel;
            if (wheel != 0) {
                if (ImGui.GetIO().KeyShift) {
                    // Snap to nearest 0.1 and increment by 0.1 per wheel tick
                    float snapped = MathF.Round(working_HSLA.S * 1000);
                    working_HSLA.S = Math.Clamp((snapped + wheel) / 1000f, 0f, 1f);
                }
                else {
                    // Snap to nearest 0.01 and increment by 0.01 per wheel tick
                    float snapped = MathF.Round(working_HSLA.S * 100);
                    working_HSLA.S = Math.Clamp((snapped + wheel) / 100f, 0f, 1f);
                }
            }
            Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Saturation Slider" }, }});
        }

        // Draw Input box
        SVector2 inputPos = pos + new SVector2(SliderSize.X + ControlSpacing.X, 0);
        ImGui.SetCursorScreenPos(inputPos);
        float value = working_HSLA.S * 100;
        if (InputSpinner.Draw("ColorPickerHueSlider", ref value, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y }))
            working_HSLA.S = value / 100;
    }
    private static void DrawLightnessSlider(SVector2 pos, ref HSLA working_HSLA) {
        var drawList = ImGui.GetWindowDrawList();
        // Draw border
        drawList.AddRect(pos - new SVector2(1, 1), pos + SliderSize + new SVector2(1, 1), Colors.ControlOuterBorder); // Outer border
        drawList.AddRect(pos, pos + SliderSize, Colors.Black); // Black Border

        // Draw Slider
        SVector2 sliderPos = pos + new SVector2(1, 1);
        var sliderBarSize = SliderSize - new SVector2(2, 2); // Inner area for gradient
        int segments = 100;
        float segmentWidth = sliderBarSize.X / segments;
        // Draw lightness gradient using current hue and saturation
        for (int i = 0; i < segments; i++) {
            var new_hsla = working_HSLA;
            new_hsla.A = 1.0f;
            new_hsla.L = i / (float)(segments - 1); // normalized [0,1]
            var col = new_hsla.ToImGuiColor();

            SVector2 p0 = sliderPos + new SVector2(i * segmentWidth, 0);
            SVector2 p1 = sliderPos + new SVector2((i + 1) * segmentWidth, sliderBarSize.Y);
            drawList.AddRectFilled(p0, p1, col);
        }
        // Draw slider handle
        float handleX = (float)Math.Round(sliderPos.X + working_HSLA.L * sliderBarSize.X);
        SVector2 handleTopLeft = new SVector2(handleX - 1, sliderPos.Y - 1);
        SVector2 handleBottomRight = new SVector2(handleX + 1, sliderPos.Y + sliderBarSize.Y + 1);
        drawList.AddRectFilled(handleTopLeft, handleBottomRight, 0xFFFFFFFF);
        drawList.AddRect(handleTopLeft - new SVector2(1, 1), handleBottomRight + new SVector2(1, 1), Colors.Black);

        // Mouse interaction
        ImGui.SetCursorScreenPos(pos);
        ImGui.InvisibleButton("##lightslidergrip", SliderSize);
        if (ImGui.IsItemActive() && ImGui.IsMouseDown(0)) {
            float mouseX = ImGui.GetIO().MousePos.X;
            float relX = Math.Clamp(mouseX - sliderPos.X, 0, sliderBarSize.X);
            working_HSLA.L = relX / sliderBarSize.X;
        }
        // Mouse wheel
        if (ImGui.IsItemHovered()) {
            float wheel = ImGui.GetIO().MouseWheel;
            if (wheel != 0) {
                if (ImGui.GetIO().KeyShift) {
                    float snapped = MathF.Round(working_HSLA.L * 1000);
                    working_HSLA.L = Math.Clamp((snapped + wheel) / 1000f, 0f, 1f);
                }
                else {
                    float snapped = MathF.Round(working_HSLA.L * 100);
                    working_HSLA.L = Math.Clamp((snapped + wheel) / 100f, 0f, 1f);
                }
            }
            Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Lightness Slider" }, } });
        }

        // Draw Input box
        SVector2 inputPos = pos + new SVector2(SliderSize.X + ControlSpacing.X, 0);
        ImGui.SetCursorScreenPos(inputPos);
        float value = working_HSLA.L * 100;
        if (InputSpinner.Draw("ColorPickerLightnessSlider", ref value, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y }))
            working_HSLA.L = value / 100;
    }
    private static void DrawAlphaSlider(SVector2 pos, ref HSLA working_HSLA) {
        var drawList = ImGui.GetWindowDrawList();
        // Draw border
        drawList.AddRect(pos - new SVector2(1, 1), pos + SliderSize + new SVector2(1, 1), Colors.ControlOuterBorder); // Outer border
        drawList.AddRect(pos, pos + SliderSize, Colors.Black); // Black Border

        // Draw Slider
        SVector2 sliderPos = pos + new SVector2(1, 1);
        var sliderBarSize = SliderSize - new SVector2(2, 2); // Inner area for gradient
        ImGUITools.DrawCheckerboard(sliderPos, sliderBarSize.X, sliderBarSize.Y);
        int segments = 100;
        float segmentWidth = sliderBarSize.X / segments;
        // Draw alpha gradient using current hue, saturation, lightness
        for (int i = 0; i < segments; i++) {
            var new_hsla = working_HSLA;
            new_hsla.A = i / (float)(segments - 1); // normalized [0,1]
            var col = new_hsla.ToImGuiColor();

            SVector2 p0 = sliderPos + new SVector2(i * segmentWidth, 0);
            SVector2 p1 = sliderPos + new SVector2((i + 1) * segmentWidth, sliderBarSize.Y);
            drawList.AddRectFilled(p0, p1, col);
        }
        // Draw slider handle
        float handleX = (float)Math.Round(sliderPos.X + working_HSLA.A * sliderBarSize.X);
        SVector2 handleTopLeft = new SVector2(handleX - 1, sliderPos.Y - 1);
        SVector2 handleBottomRight = new SVector2(handleX + 1, sliderPos.Y + sliderBarSize.Y + 1);
        drawList.AddRectFilled(handleTopLeft, handleBottomRight, 0xFFFFFFFF);
        drawList.AddRect(handleTopLeft - new SVector2(1, 1), handleBottomRight + new SVector2(1, 1), Colors.Black);

        // Mouse interaction
        ImGui.SetCursorScreenPos(pos);
        ImGui.InvisibleButton("##alphaslidergrip", SliderSize);
        if (ImGui.IsItemActive() && ImGui.IsMouseDown(0)) {
            float mouseX = ImGui.GetIO().MousePos.X;
            float relX = Math.Clamp(mouseX - sliderPos.X, 0, sliderBarSize.X);
            working_HSLA.A = relX / sliderBarSize.X;
        }
        // Mouse wheel
        if (ImGui.IsItemHovered()) {
            float wheel = ImGui.GetIO().MouseWheel;
            if (wheel != 0) {
                if (ImGui.GetIO().KeyShift) {
                    float snapped = MathF.Round(working_HSLA.A * 1000);
                    working_HSLA.A = Math.Clamp((snapped + wheel) / 1000f, 0f, 1f);
                }
                else {
                    float snapped = MathF.Round(working_HSLA.A * 100);
                    working_HSLA.A = Math.Clamp((snapped + wheel) / 100f, 0f, 1f);
                }
            }
            Tooltip.Draw(new Tooltip.Options { Lines = { new Tooltip.Title { Text = "Alpha Slider" }, } });
        }

        // Draw Input box
        SVector2 inputPos = pos + new SVector2(SliderSize.X + ControlSpacing.X, 0);
        ImGui.SetCursorScreenPos(inputPos);
        float value = working_HSLA.A * 100;
        if (InputSpinner.Draw("ColorPickerAlphaSlider", ref value, new InputSpinner.Options { Width = (int)SliderInputSize.X, Height = (int)SliderInputSize.Y }))
            working_HSLA.A = value / 100;
    }

    private static void DrawHexColorInput(SVector2 pos, int inputWidth, ref SVector4 color) {
        // convert color to hex
        int r = (int)Math.Round(color.X * 255);
        int g = (int)Math.Round(color.Y * 255);
        int b = (int)Math.Round(color.Z * 255);
        int a = (int)Math.Round(color.W * 255);
        string hexBuf = $"#{r:X2}{g:X2}{b:X2}{a:X2}";

        ImGui.SetCursorScreenPos(pos);
        Input.Draw("##ColorPickerHexInput", ref hexBuf, new Input.Options { Width = inputWidth, Height = ControlHeight, DisplayOnly = true });

        if (ImGui.IsItemHovered()) {
            if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
                ImGui.SetClipboardText(hexBuf);
            }
            if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Right)) {
                string clipboard = ImGui.GetClipboardText() ?? "";
                string hex = clipboard.Replace("#", "");
                bool valid = (hex.Length == 6 || hex.Length == 8) && hex.All(c =>
                    c >= '0' && c <= '9' ||
                    c >= 'A' && c <= 'F' ||
                    c >= 'a' && c <= 'f'
                );
                if (valid) {
                    try {
                        r = Convert.ToInt32(hex.Substring(0, 2), 16);
                        g = Convert.ToInt32(hex.Substring(2, 2), 16);
                        b = Convert.ToInt32(hex.Substring(4, 2), 16);
                        a = hex.Length == 8 ? Convert.ToInt32(hex.Substring(6, 2), 16) : 255;
                        color = new SVector4(r / 255f, g / 255f, b / 255f, a / 255f);
                    }
                    catch {
                        // Do nothing if conversion fails
                    }
                }
            }

            Tooltip.Draw(new Tooltip.Options {
                Lines = {
                new Tooltip.Title { Text = "Color Slider" },
                new Tooltip.Separator { },
                new Tooltip.DoubleLine { LeftText = "Ctrl + LeftClick:", RightText = "Copy to clipboard" },
                new Tooltip.DoubleLine { LeftText = "Ctrl + RightClick:", RightText = "Paste from clipboard" },
            }
            });
        }
    }
    private static void DrawRGBAInput(SVector2 pos, int inputWidth, ref SVector4 color) {
        int r = (int)Math.Round(color.X * 255);
        int g = (int)Math.Round(color.Y * 255);
        int b = (int)Math.Round(color.Z * 255);
        int a = (int)Math.Round(color.W * 255);
        string rgbaBuf = $"Color({r},{g},{b},{a})";

        ImGui.SetCursorScreenPos(pos);
        Input.Draw("##ColorPickerRGBAInput", ref rgbaBuf, new Input.Options { Width = inputWidth, Height = ControlHeight, DisplayOnly = true });

        if (ImGui.IsItemHovered()) {
            if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Left)) {
                ImGui.SetClipboardText(rgbaBuf);
            }
            if (ImGui.GetIO().KeyCtrl && ImGui.IsMouseClicked(ImGuiMouseButton.Right)) {
                string clipboard = ImGui.GetClipboardText() ?? "";
                var matches = Regex.Matches(clipboard, @"\b([0-9]{1,3})\b");
                var parts = matches.Cast<Match>()
                    .Select(m => m.Value)
                    .Where(s => int.TryParse(s, out int val) && val >= 0 && val <= 255)
                    .ToArray();
                if (parts.Length == 4) {
                    r = int.Parse(parts[0]);
                    g = int.Parse(parts[1]);
                    b = int.Parse(parts[2]);
                    a = int.Parse(parts[3]);
                    color = new SVector4(r / 255f, g / 255f, b / 255f, a / 255f);
                }
            }

            Tooltip.Draw(new Tooltip.Options {
                Lines = {
                new Tooltip.Title { Text = $"Red: {r} Green: {g} Blue: {b} Alpha: {a}" },
                new Tooltip.Separator { },
                new Tooltip.DoubleLine { LeftText = "Ctrl + LeftClick:", RightText = "Copy to clipboard" },
                new Tooltip.DoubleLine { LeftText = "Ctrl + RightClick:", RightText = "Paste from clipboard" },
            }
            });
        }
    }
    private static void DrawColorWithReset(SVector2 pos, int inputWidth, int inputHeight, ref SVector4 color) {
        int smallBoxSize = 19;
        var drawList = ImGui.GetWindowDrawList();
        // Large color box
        ImGUITools.DrawCheckerboard(pos, inputWidth, inputHeight);
        drawList.AddRectFilled(pos, pos + new SVector2(inputWidth, inputHeight), ImGui.ColorConvertFloat4ToU32(color));
        drawList.AddRect(pos, pos + new SVector2(inputWidth, inputHeight), Colors.Black);
        drawList.AddRect(pos - new SVector2(1, 1), pos + new SVector2(inputWidth + 1, inputHeight + 1), Colors.ControlOuterBorder);

        // Small reset box (top-left corner, flush with big box)
        SVector2 smallPos = pos;
        ImGUITools.DrawCheckerboard(pos, smallBoxSize, smallBoxSize);
        drawList.AddRectFilled(smallPos, smallPos + new SVector2(smallBoxSize, smallBoxSize), ImGui.ColorConvertFloat4ToU32(default_rgbaNormalized));
        drawList.AddRect(smallPos, smallPos + new SVector2(smallBoxSize, smallBoxSize), Colors.Black);

        // Invisible button for small box
        ImGui.SetCursorScreenPos(smallPos);
        if (ImGui.InvisibleButton("##ResetColorBox", new SVector2(smallBoxSize, smallBoxSize))) {
            color = default_rgbaNormalized;
        }

        // Tooltip for small box
        if (ImGui.IsItemHovered()) {
            int r = (int)Math.Round(default_rgbaNormalized.X * 255);
            int g = (int)Math.Round(default_rgbaNormalized.Y * 255);
            int b = (int)Math.Round(default_rgbaNormalized.Z * 255);
            int a = (int)Math.Round(default_rgbaNormalized.W * 255);
            Tooltip.Draw(new Tooltip.Options { Lines = {
                new Tooltip.Title { Text = "Original Color" },
                new Tooltip.Separator { },
                new Tooltip.DoubleLine { LeftText = "Red:", RightText = r.ToString() },
                new Tooltip.DoubleLine { LeftText = "Green:", RightText = g.ToString() },
                new Tooltip.DoubleLine { LeftText = "Blue:", RightText = b.ToString() },
                new Tooltip.DoubleLine { LeftText = "Alpha:", RightText = a.ToString() },
            }});
        }
    }
    /// <summary>
    /// Draws the Material palette as rows of color swatches.
    /// </summary>
    /// <param name="palette">The palette dictionary (e.g., Palettes.Material).</param>
    /// <param name="width">Total width for the swatch row.</param>
    /// <param name="colorSpacing">Spacing between swatches (X: horizontal, Y: vertical).</param>
    /// <param name="selectedColor">Ref to normalized RGBA color, set when a swatch is clicked.</param>
    public static void DrawPalette(Dictionary<string, List<Palettes.Swatch>> palette, int paletteWidth, int colorHeight, SVector2 colorSpacing, ref SVector4 selectedColor) {
        var drawList = ImGui.GetWindowDrawList();
        SVector2 cursor = ImGui.GetCursorScreenPos();

        int columnCount = palette.Count;
        int spacingX = (int)Math.Round(colorSpacing.X);

        // Calculate base column width and remainder for last column
        int baseColumnWidth = (paletteWidth - (columnCount - 1) * spacingX) / columnCount;
        int remainder = paletteWidth - ((baseColumnWidth + spacingX) * (columnCount - 1) + baseColumnWidth);

        int colIndex = 0;
        foreach (var group in palette) {
            int thisColumnWidth = baseColumnWidth;
            if (colIndex == columnCount - 1)
                thisColumnWidth += remainder; // Last column absorbs rounding error

            int colX = (int)Math.Round(cursor.X + colIndex * (baseColumnWidth + spacingX));
            int colY = (int)Math.Round(cursor.Y);

            int swatchCount = group.Value.Count;
            int spacingY = (int)Math.Round(colorSpacing.Y);

            for (int i = 0; i < swatchCount; i++) {
                var swatch = group.Value[i];
                int swatchY = colY + i * (colorHeight + spacingY);

                var swatchPos = new SVector2(colX, swatchY);

                // Draw swatch background
                drawList.AddRectFilled(
                    swatchPos,
                    swatchPos + new SVector2(thisColumnWidth, colorHeight),
                    swatch.Value);

                // Draw black border
                drawList.AddRect(
                    swatchPos,
                    swatchPos + new SVector2(thisColumnWidth, colorHeight),
                    0xFF000000);

                // Draw invisible button for interaction
                ImGui.SetCursorScreenPos(swatchPos);
                if (ImGui.InvisibleButton($"##swatch_{group.Key}_{i}", new SVector2(thisColumnWidth, colorHeight))) {
                    selectedColor = new SVector4(
                        swatch.Red / 255f,
                        swatch.Green / 255f,
                        swatch.Blue / 255f,
                        1.0f);
                }

                // Optional: Tooltip
                if (ImGui.IsItemHovered()) {
                    Tooltip.Draw(new Tooltip.Options {
                        Lines = {
                            new Tooltip.Title { Text = $"{swatch.Name}" },
                        }
                    });
                }
            }

            colIndex++;
        }
    }
}


