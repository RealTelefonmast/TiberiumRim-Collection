﻿using System;
using System.Collections.Generic;
using Verse;

namespace TR;

public class SuperWeaponInfo : WorldInformation
{
    private Dictionary<Building, SuperWeapon> _superWeapons = new Dictionary<Building, SuperWeapon>();

    public IReadOnlyCollection<SuperWeapon> SuperWeapons => _superWeapons.Values;

    public SuperWeaponInfo(RimWorld.Planet.World world) : base(world)
    {

    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref _superWeapons, "weaponsByBuilding", LookMode.Reference, LookMode.Deep);
    }
    
    public override void Notify_BuildingSpawned(TRBuildingPrototype building)
    {
        TryRegisterSuperweapon(building);
    }

    protected void TryRegisterSuperweapon(TRBuildingPrototype building)
    {
        if (building is not Building_SuperWeapon swepBuilding) return;
        var superWep = building.def.superWeapon;
        if (superWep == null) return;
        if (_superWeapons.ContainsKey(building))
        {
            TRLog.Error($"Building {building} already has a superweapon registered");
            return;
        }
        
        SuperWeapon wepWorker = (SuperWeapon)Activator.CreateInstance(superWep.worker, building);
        _superWeapons.Add(swepBuilding, wepWorker);
        swepBuilding.RegisterSWep(wepWorker);
    }

    public override void InfoTick()
    {
        foreach (var superWeapon in _superWeapons.Values)
        {
            superWeapon.Tick();
        }
    }
}