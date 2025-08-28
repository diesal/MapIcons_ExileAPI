namespace MapIcons;

public enum MapIconTypes {
    Unset,

    CustomPath,
    // Friendly Icons
    NPC,
    LocalPlayer,
    OtherPlayer,
    DecoyTotem,
    Minion,
    // Monsters
    WhiteMonster,
    MagicMonster,
    RareMonster,
    UniqueMonster,
    RogueExile,
    GiantRogueExile,
    Spirit,

    // Einhar Beasts
    VividVulture,
    BlackMorrigan,
    CraicicChimeral,
    WildBristleMatron,
    WildHellionAlpha,
    FenumalPlaguedArachnid,
    FenumusFirstOfTheNight,
    // Dangerous icons 
    DrowningOrb,
    VolatileCore,
    ConsumingPhantasm,
    LightningClone,
    // minimap icons 
    Shrine,
    Breach,
    QuestObject,
    Ritual,
    Waypoint,
    Checkpoint,
    AreaTransition,
    IngameNPC,
    IngameUncategorized,
    // Delirium 
    BloodBag,
    EggFodder,
    GlobSpawn,
    // Chest
    BreakableObject,
    BreachChestNormal,
    BreachChestLarge,
    ExpeditionChestWhite,
    ExpeditionChestMagic,
    ExpeditionChestRare,
    SanctumChest,
    PirateChest,
    AbyssChest,
    ChestWhite,
    ChestMagic,
    ChestRare,
    ChestUnique,    

    SanctumMote,

    // Strongbox types
    UnknownStrongbox,
    ArcanistStrongbox,
    ArmourerStrongbox,
    BlacksmithStrongbox,
    ArtisanStrongbox,
    CartographerStrongbox,
    ChemistStrongbox,
    GemcutterStrongbox,
    JewellerStrongbox,
    LargeStrongbox,
    OrnateStrongbox,
    DivinerStrongbox,
    OperativeStrongbox,
    ArcaneStrongbox,
    ResearcherStrongbox,
    
    // Traps
    LabyrinthRoomba,
    LabyrinthCascadeSpikeTrap,
    LabyrinthSawblade,
    LabyrinthSpinner,
    LabyrinthRoller,
}

public enum MapIconRenderers
{ // based on render methods
    Default,
    Monster,
    Friendly,
    IngameIcon,
    Chest,
    Player,
}

public enum TreeIconConfigs
{
    Default,
    Custom,
    IngameIcon,
    Monster,
    Chest,
    Friendly
}

public enum IconDrawStates
{
    Off = 0,
    Ranged = 1,
    Always = 2
}


