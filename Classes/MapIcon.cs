
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using RectangleF = SharpDX.RectangleF;

namespace MapIcons;

public class MapIcon
{

    public int Version;
    public Entity Entity { get; }
    public RectangleF? DrawRect { get; set; }
    public Func<Vector2> GridPosition { get; set; }
    public MonsterRarity Rarity { get; protected set; }
    public IconPriority Priority { get; set; }
    public Func<bool> Show { get; set; }
    public Func<bool> Hidden { get; set; } = () => false;
    public HudTexture InGameTexture { get; set; }
    public string RenderName => Entity.RenderName;
    public string Text { get; set; }
    public MapIconRenderers Renderer { get; set; } = MapIconRenderers.Default;
    public MapIconTypes Type { get; set; }
    public MapIconSettings Settings { get; set; }

    public MapIcon(Entity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        Entity = entity;
        Rarity = Entity.Rarity;
        Priority = Rarity switch
        {
            MonsterRarity.White => IconPriority.Low,
            MonsterRarity.Magic => IconPriority.Medium,
            MonsterRarity.Rare => IconPriority.High,
            MonsterRarity.Unique => IconPriority.VeryHigh,
            _ => IconPriority.Medium
        };
        Show = () => Entity.IsValid;
        Hidden = () => Entity.IsHidden;
        GridPosition = () => Entity.GridPosNum;
    }

}