using TeleCore;
using UnityEngine;
using Verse;

namespace TR;

[StaticConstructorOnStartup]
public static class TRContent
{
    public static readonly Texture2D BGPlanet = ContentFinder<Texture2D>.Get("UI/Menu/Background", true);
    public static readonly Texture2D ResearchBG = ContentFinder<Texture2D>.Get("UI/Menu/ResearchBG", true);
    public static readonly Texture2D MainMenu = ContentFinder<Texture2D>.Get("UI/Menu/MainMenu", true);
    public static readonly Texture2D MenuWindow = ContentFinder<Texture2D>.Get("UI/Menu/MenuWindow", true);
    public static readonly Texture2D Banner = ContentFinder<Texture2D>.Get("UI/Menu/Banner", true);
    public static readonly Texture2D LockedBanner = ContentFinder<Texture2D>.Get("UI/Menu/LockedBanner", true);

    public static readonly Texture2D TopBar = ContentFinder<Texture2D>.Get("UI/Menu/TopBar", true);
    public static readonly Texture2D TibOptionBG = ContentFinder<Texture2D>.Get("UI/Menu/TibOptionBG", true);
    public static readonly Texture2D TibOptionBG_Cut = ContentFinder<Texture2D>.Get("UI/Menu/TibOptionBG_Cut", true);

    public static readonly Texture2D TibOptionBG_CutFade =
        ContentFinder<Texture2D>.Get("UI/Menu/TibOptionBG_CutFade", true);

    //Research
    public static readonly Texture2D UnseenResearch =
        ContentFinder<Texture2D>.Get("UI/Icons/Research/UnseenResearch", true);

    public static readonly Texture2D NewResearch = ContentFinder<Texture2D>.Get("UI/Icons/Research/NewResearch", true);

    //
    public static readonly Texture2D OpenMenu = ContentFinder<Texture2D>.Get("UI/Icons/Research/OpenMenu", true);
    public static readonly Texture2D Construct = ContentFinder<Texture2D>.Get("UI/Icons/Research/Construct", true);
    public static readonly Texture2D SelectThing = ContentFinder<Texture2D>.Get("UI/Icons/Research/SelectThing", true);

    //Misc
    public static readonly Texture2D VectorArrow = ContentFinder<Texture2D>.Get("Misc/Arrow");
    public static readonly Material ArrowMat = MaterialPool.MatFrom("Misc/Arrow");

    public static readonly Texture2D MarkedForDeath = ContentFinder<Texture2D>.Get("UI/Icons/Marked");
    public static readonly Texture2D Icon_EVA = ContentFinder<Texture2D>.Get("UI/Icons/PlaySettings/EVA");

    public static readonly Texture2D Icon_Radiation =
        ContentFinder<Texture2D>.Get("UI/Icons/PlaySettings/RadiationOverlay");

    //Turrets
    public static readonly Material TurretCable = MaterialPool.MatFrom("Buildings/Nod/Defense/Turrets/TurretCable");

    //
    // public static readonly Texture2D DeleteX = TexButton.Delete;
    // public static readonly Texture2D Plus = TexButton.Plus;
    // public static readonly Texture2D Minus = TexButton.Minus;
    // public static readonly Texture2D Infinity = TexButton.Infinity;
    // public static readonly Texture2D Suspend = TexButton.Suspend;
    // public static readonly Texture2D Copy = TexButton.Copy;
    // public static readonly Texture2D Paste = TexButton.Paste;

    //UI - Menus

    public static readonly Texture2D Undiscovered = ContentFinder<Texture2D>.Get("UI/Menu/Undiscovered", true);
    public static readonly Texture2D Fact_Undisc = ContentFinder<Texture2D>.Get("UI/Menu/Fact_Undiscovered", true);
    public static readonly Texture2D Des_Undisc = ContentFinder<Texture2D>.Get("UI/Menu/Des_Undiscovered", true);
    public static readonly Texture2D Tab_Undisc = ContentFinder<Texture2D>.Get("UI/Menu/Tab_Undiscovered", true);
    public static readonly Texture2D InfoButton = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);
    public static readonly Texture2D SideBarArrow = ContentFinder<Texture2D>.Get("UI/Menu/Arrow", true);

    public static readonly Texture2D HighlightAtlas =
        ContentFinder<Texture2D>.Get("UI/Widgets/TutorHighlightAtlas", true);

    public static readonly Texture2D HightlightInMenu = ContentFinder<Texture2D>.Get("UI/Icons/HighLight", true);

    //UI - Icons
    //--Controls

    //----SuperWeapon
    public static readonly Texture2D NodNukeIcon =
        ContentFinder<Texture2D>.Get("UI/Icons/SuperWeapon/Launch_Nuke", true);

    public static readonly Texture2D IonCannonIcon =
        ContentFinder<Texture2D>.Get("UI/Icons/SuperWeapon/Launch_IonCannon", true);

    public static readonly Texture2D FireStorm_On =
        ContentFinder<Texture2D>.Get("UI/Icons/SuperWeapon/Firestorm_On", true);

    public static readonly Texture2D FireStorm_Off =
        ContentFinder<Texture2D>.Get("UI/Icons/SuperWeapon/Firestorm_Off", true);

    //--Faction Icons
    public static readonly Texture2D CommonIcon = ContentFinder<Texture2D>.Get("UI/Icons/Factions/Common", true);
    public static readonly Texture2D ForgottenIcon = ContentFinder<Texture2D>.Get("UI/Icons/Factions/Forgotten", true);
    public static readonly Texture2D GDIIcon = ContentFinder<Texture2D>.Get("UI/Icons/Factions/GDI", true);
    public static readonly Texture2D NodIcon = ContentFinder<Texture2D>.Get("UI/Icons/Factions/Nod", true);
    public static readonly Texture2D ScrinIcon = ContentFinder<Texture2D>.Get("UI/Icons/Factions/Scrin", true);

    public static readonly Texture2D BlackMarketIcon =
        ContentFinder<Texture2D>.Get("UI/Icons/Factions/BlackMarket", true);

    //--World

    //--Research
    public static readonly Texture2D Research_Active = ContentFinder<Texture2D>.Get("UI/Icons/Research/Active");

    public static readonly Texture2D Research_Available =
        ContentFinder<Texture2D>.Get("UI/Icons/Research/Available", true);

    //ThingCategories
    public static readonly Texture2D
        TiberiumIcon = ContentFinder<Texture2D>.Get("UI/Icons/Tiberium/Tiberium_RGB", true);

    //Targeter Mats
    public static readonly Material IonCannonTargeter =
        MaterialPool.MatFrom("UI/Targeters/Target_IonCannon", ShaderDatabase.Transparent);

    public static readonly Material NodNukeTargeter =
        MaterialPool.MatFrom("UI/Targeters/Target_Nuke", ShaderDatabase.Transparent);

    public static readonly Material ScrinLandingTargeter =
        MaterialPool.MatFrom("UI/Targeters/Target_IonCannon", ShaderDatabase.Transparent);

    public static readonly Material IonLightningMat =
        MaterialPool.MatFrom("VisualFX/LightningBoltIon", ShaderDatabase.MoteGlow);

    public static readonly Material ForcedTargetLineMat =
        MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, new Color(1f, 0.5f, 0.5f));
    
    
    public static readonly Texture2D GreenBar = SolidColorMaterials.NewSolidColorTexture(TColor.Green);
    public static readonly Texture2D BlueBar = SolidColorMaterials.NewSolidColorTexture(TColor.NiceBlue);
    public static readonly Texture2D BarBG = SolidColorMaterials.NewSolidColorTexture(TColor.BGLighter);
}