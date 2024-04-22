using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    [StaticConstructorOnStartup]
    public class TiberiumCrystal : Plant
    { 
        public TerrainDef corruptsTo = null;

        public object cachedLabelMouseover { get; private set; }

        public int DyingDamagePerTick { get; private set; }

        public List<IntVec3> affectedTiles = new List<IntVec3>();

        private int bledTimes = 0;

        protected override bool Resting
        {
            get
            {
                return Current.Game.VisibleMap.mapTemperature.OutdoorTemp < -50f;
            }
        }

        public override float GrowthRate
        {
            get
            {
                return 1;
            }
        }

        public override bool IngestibleNow
        {
            get
            {
                return false;
            }
        }

        public override float CurrentDyingDamagePerTick
        {
            get
            {
                return 0f;
            }
        }

        public void SetTiles()
        {
            for (int i = 0; i < GenAdjFast.AdjacentCells8Way(this.Position).Count; i++)
            {
                IntVec3 v = GenAdjFast.AdjacentCells8Way(this.Position)[i];
                if (v.InBounds(this.Map))
                {
                    if (!this.Map.GetComponent<MapComponent_Tiberium>().affectedTiles.Contains(v))
                    {
                        this.Map.GetComponent<MapComponent_Tiberium>().affectedTiles.Add(v);
                    }
                }
                this.affectedTiles.Add(v);
            }
        }

        public void RemoveTiles()
        {
            HashSet<IntVec3> sourceAffectedTiles = new HashSet<IntVec3>();
            for (int i = 0; i < this.affectedTiles.Count; i++)
            {
                IntVec3 v = this.affectedTiles[i];
                if (v.InBounds(this.Map))
                {
                    List<TiberiumCrystal> sources = MapComponent_Tiberium.Sources(v, this.Map);
                    for (int ii = 0; ii < sources.Count; ii++)
                    {
                        TiberiumCrystal source2 = sources[ii];
                        if (source2 != null)
                        {
                            if (source2.affectedTiles.Contains(v))
                            {
                                sourceAffectedTiles.Add(v);
                            }
                        }
                    }
                    if (this.Map.GetComponent<MapComponent_Tiberium>().affectedTiles.Contains(v) && !sourceAffectedTiles.Contains(v))
                    {
                        this.Map.GetComponent<MapComponent_Tiberium>().affectedTiles.Remove(v);
                    }
                }
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            SetTiles();
            Plant c = this.Position.GetPlant(map);
            if (c != null && c != this)
            {
                if (c.def.defName.Contains("Tiberium"))
                {
                    this.Destroy(DestroyMode.Vanish);
                    return;
                }
                else
                {
                    c.Destroy(DestroyMode.Vanish);
                }
            }
            
            if (!this.Map.GetComponent<MapComponent_TiberiumBiomeChecker>().tiberCount.Contains(this.thingIDNumber))
            {
                this.Map.GetComponent<MapComponent_TiberiumBiomeChecker>().tiberCount.Add(this.thingIDNumber);
            } 
        }

        public override void DeSpawn()
        {
            RemoveTiles();
            TiberiumCrystalDef Localdef = this.def as TiberiumCrystalDef;

            if (this.Map.GetComponent<MapComponent_TiberiumBiomeChecker>().tiberCount.Contains(this.thingIDNumber))
            {
                this.Map.GetComponent<MapComponent_TiberiumBiomeChecker>().tiberCount.Remove(this.thingIDNumber);
            }
            base.DeSpawn();
        }

        public override void TickLong()
        {
            Bleed(Map);
            if (Destroyed)
            {
                return;
            }

            if (def.plant.LimitedLifespan)
            {
                ageInt += GenTicks.TickLongInterval;
            }         

            TiberiumCrystalDef Localdef = this.def as TiberiumCrystalDef;

            corruptsTo = Localdef.corruptsInto;

            List<ThingDef> friendlyTo = Localdef.friendlyTo;

            if (def.plant.reproduces && growthInt >= SeedShootMinGrowthPercent)
            {
                if (Rand.MTBEventOccurs(def.plant.reproduceMtbDays, GenDate.TicksPerDay, GenTicks.TickLongInterval))
                {
                    if (!GenPlant.SnowAllowsPlanting(Position, Map))
                    {
                        return;
                    }
                    GenPlantReproduction.TryReproduceFrom(Position, def, SeedTargFindMode.Reproduce, Map, corruptsTo, friendlyTo);
                }
            }

            TerrainDef t = this.Position.GetTerrain(Map);
            if (t != null && t != this.corruptsTo)
            {
                Map.terrainGrid.SetTerrain(this.Position, this.corruptsTo);
            }

            float prevGrowth = growthInt;
            bool wasMature = LifeStage == PlantLifeStage.Mature;
            growthInt += GrowthPerTick * GenTicks.TickLongInterval;

            if (growthInt > 1f)
            {
                growthInt = 1f;
            }

            if ((!wasMature && LifeStage == PlantLifeStage.Mature) || (int)(prevGrowth * 10f) != (int)(growthInt * 10f))
            {
                if (CurrentlyCultivated())
                {
                    Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
                }
            }
            cachedLabelMouseover = null;
        }

        public void Bleed(Map map)
        {
            if (this.HitPoints < this.MaxHitPoints && this.bledTimes < 5)
            {
                IntVec3 pos = this.PositionHeld;
                FilthMaker.MakeFilth(pos, map, ThingDefOf.FilthBlood, 1);
                bledTimes = bledTimes + 1;
            }
            else if(this.HitPoints >= this.MaxHitPoints)
            {
                this.bledTimes = 0;
            }
        }
    }

    public static class GenPlantReproduction
    {
        public static Plant TryReproduceFrom(IntVec3 source, ThingDef plantDef, SeedTargFindMode mode, Map map, TerrainDef setTerrain, List<ThingDef> friendlyTo)
        {
            IntVec3 dest;
            if (!TryFindReproductionDestination(source, plantDef, mode, map, friendlyTo, out dest))
                return null;

            return TryReproduceInto(dest, plantDef, map, setTerrain, friendlyTo);
        }

        public static Plant TryReproduceInto(IntVec3 dest, ThingDef plantDef, Map map, TerrainDef setTerrain, List<ThingDef> friendlyTo)
        {
            if (!plantDef.CanEverPlantAt(dest, map))
                return null;

            if (!GenPlant.SnowAllowsPlanting(dest, map))
                return null;

            if (TiberiumRimSettings.settings.UseSpreadRadius)
            {
                if (!CheckLists.AllowedCells.Contains(dest))
                {
                    return null;
                }
            }

            if (CheckLists.ProtectedCells.Contains(dest) || CheckLists.SuppressedCells.Contains(dest))
            {
                return null;
            }

            var t = dest.GetTerrain(map);

            if (t.defName.Contains("Water") | t.defName.Contains("Marsh") && !t.defName.Contains("Marshy"))
            {
                if (plantDef.defName.Contains("TiberiumVein"))
                {
                    return null;
                }
                plantDef = ThingDef.Named("TiberiumGlacier");
                setTerrain = TerrainDef.Named("TiberiumWater");
            }
            else 
            if (plantDef.defName.Contains("TiberiumGlacier"))
            {
                return null;
            }

            if(plantDef.defName.Contains("Pod"))
            {
                if(!t.defName.Contains("Soil"))
                {
                    return null;
                }
            }
            
            if (t.defName.Contains("Sand") && !t.defName.Contains("Sandstone") && Rand.Chance(0.3f))
            {
                if (!plantDef.defName.Contains("Desert"))
                {
                    switch (plantDef.defName)
                    {
                        case "TiberiumGreen":
                            plantDef = ThingDef.Named("TiberiumDesertGreen");
                            setTerrain = TerrainDef.Named("TiberiumSandGreen");
                            break;

                        case "TiberiumBlue":
                            plantDef = ThingDef.Named("TiberiumDesertBlue");
                            setTerrain = TerrainDef.Named("TiberiumSandBlue");
                            break;

                        case "TiberiumRed":
                            plantDef = ThingDef.Named("TiberiumDesertRed");
                            setTerrain = TerrainDef.Named("TiberiumSandRed");
                            break;

                        case "TiberiumVein":
                            plantDef = ThingDef.Named("TiberiumVein");
                            setTerrain = TerrainDef.Named("TiberiumSandRed");
                            break;

                        case "TiberiumPod":
                            return null;

                        case "TiberiumMossGreen":
                            return null;

                        case "TiberiumMossBlue":
                            return null;

                    }
                    ChangeTerrain(dest, map, setTerrain);
                    return (Plant)GenSpawn.Spawn(plantDef, dest, map);
                }
                else
                {
                    ChangeTerrain(dest, map, setTerrain);
                    return (Plant)GenSpawn.Spawn(plantDef, dest, map);
                }
            }
            else if (plantDef.defName.Contains("Desert"))
            {
                return null;
            }
            
            if (t.defName.Contains("Ice"))
            {
                switch (plantDef.defName)
                {
                    case "TiberiumGreen":
                        setTerrain = TerrainDef.Named("TiberiumIce");
                        break;

                    case "TiberiumBlue":
                        setTerrain = TerrainDef.Named("TiberiumIce");
                        break;

                    case "TiberiumRed":
                        return null;

                    case "TiberiumPod":
                        return null;

                    case "TiberiumVein":
                        plantDef = ThingDef.Named("TiberiumVein");
                        setTerrain = TerrainDef.Named("TiberiumIce");
                        break;

                    case "TiberiumDesertGreen":
                        plantDef = ThingDef.Named("TiberiumMossGreen");
                        setTerrain = TerrainDef.Named("TiberiumIce");
                        break;

                    case "TiberiumDesertBlue":
                        plantDef = ThingDef.Named("TiberiumMossBlue");
                        setTerrain = TerrainDef.Named("TiberiumIce");
                        break;

                    case "TiberiumMossGreen":
                        setTerrain = TerrainDef.Named("TiberiumIce");
                        break;

                    case "TiberiumMossBlue":
                        setTerrain = TerrainDef.Named("TiberiumIce");
                        break;

                    case "TiberiumDesertRed":
                        return null;
                }
            }

            if (t.fertility < 0.01)
            {
                if (!t.defName.Contains("Tiberium") || t.defName.Contains("moss"))
                {
                    if (plantDef.defName.Contains("Moss"))
                    {
                        ChangeTerrain(dest, map, setTerrain);
                        return (Plant)GenSpawn.Spawn(plantDef, dest, map);
                    }
                    else
                    {
                        switch (plantDef.defName)
                        {
                            case "TiberiumGreen":
                                plantDef = TiberiumCrystalDefOf.TiberiumMossGreen;
                                setTerrain = TerrainDef.Named("TiberiumRockGreen");
                                break;

                            case "TiberiumBlue":
                                plantDef = TiberiumCrystalDefOf.TiberiumMossBlue;
                                setTerrain = TerrainDef.Named("TiberiumRockBlue");
                                break;

                            case "TiberiumRed":
                                return null;

                            case "TiberiumPod":
                                return null;

                            case "TiberiumVein":
                                plantDef = TiberiumCrystalDefOf.TiberiumVein;
                                setTerrain = TerrainDef.Named("TiberiumRockGreen");
                                break;

                            case "TiberiumDesertGreen":
                                plantDef = TiberiumCrystalDefOf.TiberiumMossGreen;
                                setTerrain = TerrainDef.Named("TiberiumRockGreen");
                                break;

                            case "TiberiumDesertBlue":
                                plantDef = TiberiumCrystalDefOf.TiberiumMossBlue;
                                setTerrain = TerrainDef.Named("TiberiumRockBlue");
                                break;

                            case "TiberiumDesertRed":
                                return null;
                        }
                    }
                }
            }
            else if (t.defName.Contains("Mossy") || t.defName.Contains("Mud"))
            {
                if (plantDef.defName.Contains("Moss"))
                {
                    switch (plantDef.defName)
                    {
                        case "TiberiumMossGreen":
                            setTerrain = TerrainDef.Named("TiberiumMossyTerrainGreen");
                            break;

                        case "TiberiumMossBlue":
                            setTerrain = TerrainDef.Named("TiberiumMossyTerrainBlue");
                            break;
                    }
                }
                else
                {
                    switch (plantDef.defName)
                    {
                        case "TiberiumGreen":
                            plantDef = TiberiumCrystalDefOf.TiberiumMossGreen;
                            setTerrain = TerrainDef.Named("TiberiumMossyTerrainGreen");
                            break;

                        case "TiberiumBlue":
                            plantDef = TiberiumCrystalDefOf.TiberiumMossBlue;
                            setTerrain = TerrainDef.Named("TiberiumMossyTerrainBlue");
                            break;
                    }
                }
            }
            else if (plantDef.defName.Contains("Moss"))
            {
                return null;
            }

            if (Rand.Chance(0.8f))
            {
                ChangeTerrain(dest, map, setTerrain);
                return (Plant)GenSpawn.Spawn(plantDef, dest, map);
            }
            return null;
        }

        public static void ChangeTerrain(IntVec3 c, Map map, TerrainDef setTerrain)
        {
            if (c.InBounds(map))
            {
                TerrainDef targetTerrain = c.GetTerrain(map);
                map.terrainGrid.SetTerrain(c, setTerrain);
            }
        }

        public static ThingDef GetAnyPlant(String name)
        {
            ThingDef plant = null;
            if (name.Contains("Tree"))
            {
                plant = TiberiumPlantDefOf.TiberiumTree;
            }
            else if (name.Contains("Bush") || Rand.Chance(0.65f))
            {
                if (Rand.Chance(0.45f))
                {
                    plant = TiberiumPlantDefOf.TiberiumBush;
                }
                else
                {
                    plant = TiberiumPlantDefOf.TiberiumShroom;
                }
            }
            else
            {
                plant = TiberiumPlantDefOf.TiberiumGrass;
            }
            return plant;
        }

        public static bool TryFindReproductionDestination(IntVec3 source, ThingDef plantDef, SeedTargFindMode mode, Map map, List<ThingDef> friendlyTo, out IntVec3 foundCell)
        {
            float radius = -1;
            if (mode == SeedTargFindMode.Reproduce)
                radius = plantDef.plant.reproduceRadius;
            else if (mode == SeedTargFindMode.MapGenCluster)
                radius = plantDef.plant.WildClusterRadiusActual;
            else if (mode == SeedTargFindMode.MapEdge)
                radius = 40;

            var rect = CellRect.CenteredOn(source, Mathf.RoundToInt(radius));
            rect.ClipInsideMap(map);

            for (int z = rect.minZ; z <= rect.maxZ; z++)
            {
                for (int x = rect.minX; x <= rect.maxX; x++)
                {
                    var c = new IntVec3(x, 0, z);
                    var p = c.GetPlant(map);

                    if (p != null)
                    {
                        if (!friendlyTo.Contains(p.def))
                        {
                            if (Rand.Chance(0.75f))
                            {                              
                                String name = p.def.defName;
                                ThingDef flora = GetAnyPlant(name);
                                TerrainDef setTerrain = TerrainDef.Named("TiberiumSoilGreen");
                                IntVec3 loc = p.Position;

                                p.Destroy(DestroyMode.Vanish);

                                GenSpawn.Spawn(flora, loc, map);
                                ChangeTerrain(loc, map, setTerrain);

                            }
                            else
                            {
                                p.Destroy(DestroyMode.Vanish);
                            }
                        }
                    }
                }
            }

            Predicate<IntVec3> destValidator = c =>
            {
                if (!plantDef.CanEverPlantAt(c, map))
                    return false;

                if (!GenPlant.SnowAllowsPlanting(c, map))
                    return false;

                if (!source.InHorDistOf(c, radius))
                    return false;

                if (!GenSight.LineOfSight(source, c, map, skipFirstCell: true))
                    return false;

                return true;
            };
            return CellFinder.TryFindRandomCellNear(source, map, Mathf.CeilToInt(radius), destValidator, out foundCell);
        }
    }
}
