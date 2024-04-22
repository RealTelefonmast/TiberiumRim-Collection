using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace TR;

public class WorldComponent_TR : WorldComponent
{   
    private Dictionary<Type, WorldInformation> _worldInfoByType = new();
    private List<WorldInformation> _worldInfos = new();
    
    public TRGameSettingsInfo TRTibGameSettings;
    
    public WorldComponent_TR(World world) : base(world)
    {
        foreach (var info in typeof(WorldInformation).AllSubclassesNonAbstract())
        {
            try
            {
                var item = (WorldInformation) Activator.CreateInstance(info, world);
                _worldInfos.Add(item);
            }
            catch (Exception ex)
            {
                TRLog.Error($"Could not instantiate a WorldInfo of type {info}:\n{ex}");
            }
        }
        
        _worldInfoByType.Clear();
        foreach (var mapInfo in _worldInfos) 
            _worldInfoByType.Add(mapInfo.GetType(), mapInfo);
    }
    
    public T GetWorldInfo<T>() where T : WorldInformation
    {
        return (T) _worldInfoByType[typeof(T)];
    }
    
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref _worldInfos, "worldInfos", LookMode.Deep, world);
    }
    
    public override void WorldComponentTick()
    {
        foreach (var info in _worldInfos)
        {
            info.InfoTick();
        }
    }
    
    public void Notify_BuildingSpawned(TRBuildingPrototype building)
    {
        foreach (var info in _worldInfos)
        {
            info.Notify_BuildingSpawned(building);
        }
    }
    
    public void Notify_RegisterWorldObject(GlobalTargetInfo worldObjectOrThing)
    {
        foreach (var info in _worldInfos)
        {
            info.Notify_RegisterWorldObject(worldObjectOrThing);
        }
    }
    
    public void Notify_EventHappened(string tag, IIncidentTarget location = null)
    {
        foreach (var info in _worldInfos)
        {
            info.Notify_EventHappened(tag, location);
        }
    }
}