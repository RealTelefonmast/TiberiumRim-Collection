﻿using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using TeleCore;
using TeleCore.Data.Events;
using Verse;

namespace TR;

public class Building_IonCannon : Building_SuperWeapon
{
    private AttackSatellite_Ion _asat;
        
    //FX
    public override bool? FX_ShouldDraw(FXLayerArgs args)
    {
        return args.index switch
        {
            0 => true,
            1 => CentralLight
        };
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        _asat = (AttackSatellite_Ion) WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("ASat_Ion"));
        _asat.Tile = Tile;
        Find.WorldObjects.Add(_asat);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        _asat.Destroy();
        Find.WorldObjects.Remove(_asat);
        base.DeSpawn(mode);
    }

    public bool CentralLight => true;

    public override string GetInspectString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(base.GetInspectString());
        sb.AppendLine("Current ASATS: " + TRUtils.Tiberium().GetWorldInfo<SatelliteInfo>().AttackSatelliteNetwork.ASatsIon.Count);
        return sb.ToString().TrimEndNewlines();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (DebugSettings.godMode)
        {

        }
    }
}