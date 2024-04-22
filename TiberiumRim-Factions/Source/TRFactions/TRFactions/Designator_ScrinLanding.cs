using RimWorld;
using Verse;

namespace TR;

public class Designator_ScrinLanding : Designator_Target
{
    public bool activated = false;

    public Designator_ScrinLanding()
    {
        this.defaultLabel = "DEBUG: Scrin Landing";
        this.defaultDesc = "Scrin lands here now";
        this.icon = TRContent.ScrinIcon;
        this.useMouseIcon = false;
        this.soundSucceeded = SoundDefOf.Click;
        this.mustBeUsed = true;

        targeterMat = TRContent.NodNukeTargeter;
        size = 6;
    }

    public override void Selected()
    {
        base.Selected();
        GameComponent_EVA.EVAComp().ReceiveSignal(EVASignal.SelectDestination, null);
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        Skyfaller skyfaller = SkyfallerMaker.MakeSkyfaller(TRFDefOf.ScrinDronePlatformIncoming, TRFDefOf.ScrinDronePlatform);
        DronePlatform platform = (DronePlatform) ThingMaker.MakeThing(TRFDefOf.ScrinDronePlatform);
        platform.SetFactionDirect(Faction.OfPlayer);
        SkyfallerMaker.SpawnSkyfaller(TRFDefOf.ScrinDronePlatformIncoming, platform, c, Map);
        activated = true;
    }

    public override bool MustStaySelected => base.MustStaySelected && !activated;

    public override bool CanRemainSelected()
    {
        return !activated;
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 loc)
    {
        return base.CanDesignateCell(loc).Accepted && loc.Standable(Map); 
    }
}