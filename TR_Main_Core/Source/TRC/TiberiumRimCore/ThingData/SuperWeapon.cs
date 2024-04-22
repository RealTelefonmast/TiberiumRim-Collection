﻿using System;
using System.Collections.Generic;
using RimWorld;
using TeleCore;
using Verse;

namespace TR;

public interface ISuperWeapon
{
    void Notify_Fired();
}

public class SuperWeapon : IExposable, ICoolDownHolder, ISuperWeapon
{
    private Designator_Target resolvedDesignator;
    
    private TRBuildingPrototype _building;
    
    private int ticksUntilReady = 0;

    public virtual bool Active => _building.DestroyedOrNull() && IsPowered;

    public virtual bool CanFire => !CoolDownActive;
    public bool CoolDownActive => ticksUntilReady > 0;
    public float DisabledPct
    {
        get
        {
            float total = Props.chargeTime.SecondsToTicks();
            return ticksUntilReady / total;
        }
    }

    public bool IsPowered => ((CompPowerTrader)_building.PowerComp).PowerOn;


    public SuperWeaponProperties Props => _building.def.superWeapon;
    
    public SuperWeapon(){}
    
    public SuperWeapon(TRBuildingPrototype building)
    {
        _building = building;
        //ticksUntilReady = Props.chargeTime.SecondsToTicks();
    }
    
    public void ExposeData()
    {
        Scribe_References.Look(ref _building, "building");
        Scribe_Values.Look(ref ticksUntilReady, "ticksUntilReady");
    }
    
    public virtual void Tick()
    {
        if (ticksUntilReady > 0)
        {
            ticksUntilReady--;
        }
    }

    public void Notify_Fired()
    {
        if (CoolDownActive)
        {
            TRLog.Warning("Attempted to fire a super weapon while it was on cooldown.");
            return;
        }
        ticksUntilReady = Props.chargeTime.SecondsToTicks();
    }

    private Designator ResolvedDesignator
    {
        get
        {
            if (resolvedDesignator == null)
            {
                resolvedDesignator = (Designator_Target)Activator.CreateInstance(Props.designator);
                resolvedDesignator.coolDown = this;
                resolvedDesignator.superWeapon = this;
            }
            return this.resolvedDesignator;
        }
    }
    
    public IEnumerable<Gizmo> GetGizmos()
    {
        if (DebugSettings.godMode)
        {
            yield return new Command_Action()
            {
                defaultLabel = "Reset Cooldown",
                action = delegate
                {
                    ticksUntilReady = 0;
                }
            };
        }
        
        yield return ResolvedDesignator;
    }
}