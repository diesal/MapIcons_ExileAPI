namespace DieselTools_ExileAPI;

public static class Colors {

    // UI colors 
    public static readonly uint White = ColorTools.RGBA2Uint(255, 255, 255);
    public static readonly uint Black = ColorTools.RGBA2Uint(0, 0, 0);
    public static readonly uint Transparent = ColorTools.RGBA2Uint(0, 0, 0, 0);
    public static readonly uint WindowBackground = ColorTools.RGBA2Uint(0, 0, 0);
    public static readonly uint Highlight = ColorTools.RGBA2Uint(255, 255, 255, 30);

    public static readonly uint Border = ColorTools.RGBA2Uint(0, 0, 0);

    public static readonly uint Panel = ColorTools.HSLA2Uint(220, 15, 19);
    public static readonly uint PanelInnerGlow = ColorTools.RGBA2Uint(255, 255, 255, 10);

    public static readonly uint SwatchInnerGlow = ColorTools.RGBA2Uint(255, 255, 255, 15);


    public static readonly uint ControlRed = ColorTools.RGBA2Uint(130, 23, 23, 255);
    public static readonly uint ControlGreen = ColorTools.RGBA2Uint(27, 152, 27, 255);
    public static readonly uint ControlBlue = ColorTools.RGBA2Uint(25, 128, 230, 255);
    public static readonly uint ControlOrange = ColorTools.RGBA2Uint(173, 81, 31, 255);
    public static readonly uint ControlYellow = ColorTools.RGBA2Uint(188, 161, 21, 255);
    public static readonly uint ControlPurple = ColorTools.RGBA2Uint(119, 31, 173, 255);
    public static readonly uint ControlPink = ColorTools.RGBA2Uint(173, 31, 93, 255);

    public static readonly uint ControlOuterGlow = ColorTools.RGBA2Uint(255, 255, 255, 5);
    public static readonly uint ControlInnerGlow = ColorTools.RGBA2Uint(255, 255, 255, 12);
    public static readonly uint ControlText = ColorTools.HSLA2Uint(220, 15, 90);
    public static readonly uint Input = ColorTools.HSLA2Uint(220, 15, 12);
    public static readonly uint Button = ColorTools.HSLA2Uint(220, 15, 30);
    public static readonly uint ButtonChecked = ControlBlue;
    public static readonly uint ButtonHovered = Highlight;


}
