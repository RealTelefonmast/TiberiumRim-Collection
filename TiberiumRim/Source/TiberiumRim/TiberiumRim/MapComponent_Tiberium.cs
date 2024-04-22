using System.Collections.Generic;
using System.Linq;
using Verse;
using System;
using UnityEngine;
using RimWorld;

namespace TiberiumRim
{
    public class MapComponent_Tiberium : MapComponent
    {
        public HashSet<IntVec3> affectedTiles = new HashSet<IntVec3>();

        public MapComponent_Tiberium(Map map) : base(map)
        {
        }


        private bool TibExists => Current.Game.VisibleMap.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("Tiberium")) != null;

        private bool TiberiumExists
        {
            get
            {
                if (Current.Game.VisibleMap.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("Tiberium")) != null)
                {
                    return true;
                }
                return false;
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<IntVec3>(ref this.affectedTiles, "affectedTiles", LookMode.Value);
            base.ExposeData();
        }

        private bool CanSpawnMonolith(IntVec3 source, Plant parent)
        {
            if (parent.HarvestableNow)
            {
                foreach (IntVec3 intVec in GenAdjFast.AdjacentCells8Way(parent.Position, parent.Rotation, parent.RotatedSize))
                {
                    if (intVec.InBounds(map))
                    {
                        if (intVec.GetFirstThing(map, parent.def) != null && intVec.GetFirstThing(map, parent.def).def.plant.Harvestable)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void MonolithRise(Map map, ThingDef tower, Plant parent)
        {
            IntVec3 loc = parent.Position;
            GenSpawn.Spawn(tower, loc, map);
        }

        public static TiberiumCrystal GetTiberium(IntVec3 c, Map map)
        { 
            List<Thing> list = map.thingGrid.ThingsListAtFast(c);
            TiberiumCrystal result;
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].def.thingClass == typeof(TiberiumCrystal))
                {
                    result = (TiberiumCrystal)list[i];
                    return result;
                }
            }
            result = null;
            return result;
        }

        public static List<TiberiumCrystal> Sources(IntVec3 c, Map map)
        {        
            List<TiberiumCrystal> parentList = new List<TiberiumCrystal>();
            foreach (IntVec3 v in GenAdjFast.AdjacentCells8Way(c))
            {
                if (v.InBounds(map))
                {
                    TiberiumCrystal source = GetTiberium(v, map);
                    if (source != null)
                    {
                        parentList.Add(source);
                    }
                }               
            }
            return parentList;
        }

        public static Plant Source(IntVec3 c, Map map)
        {
            foreach (IntVec3 v in GenAdjFast.AdjacentCells8Way(c))
            {
                if (v.InBounds(map))
                {
                    Plant source = v.GetPlant(map);
                    if (source != null)
                    {
                        if (source.def.thingClass == typeof(TiberiumCrystal))
                        {
                            return source;
                        }
                    }
                }
            }
            return null;
        }

        public override void MapComponentTick()
        {
            if (Find.TickManager.TicksGame % GenTicks.TickLongInterval == 0)
            {
                if (TiberiumExists)
                {
                    AffectTile(affectedTiles);
                }
            }
            base.MapComponentTick();
        }

        public void AffectTile(HashSet<IntVec3> affectedTiles)
        {
            HashSet<IntVec3> tempHashSet = new HashSet<IntVec3>();
            tempHashSet = affectedTiles;

            foreach(IntVec3 v in tempHashSet)
            {
                if (affectedTiles.Contains(v))
                {
                    if (v.InBounds(this.map))
                    {
                        Plant parent = Source(v, map);
                        if (parent != null)
                        {
                            TiberiumCrystalDef def = (TiberiumCrystalDef)parent.def;
                            DamageInfo damageEntity = new DamageInfo(DamageDefOf.Deterioration, def.EntityDamage);
                            DamageInfo damageBuilding = new DamageInfo(DamageDefOf.Deterioration, def.BuildingDamage);
                            if (def.Monolith != null && CanSpawnMonolith(parent.Position, parent) && Rand.Chance(0.05f) && Find.TickManager.TicksGame % GenDate.TicksPerDay == 0)
                            {
                                MonolithRise(map, def.Monolith, parent);
                                break;
                            }
                            List<Thing> thingList = v.GetThingList(this.map);
                            for (int i = 0; i < thingList.Count; i++)
                            {
                                Thing thing = thingList[i];
                                if (thing != null)
                                {
                                    if (thing is Pawn)
                                    {
                                        Pawn pawn = thing as Pawn;
                                        if (pawn != null)
                                        {
                                            TiberiumUtility.AffectPawn(pawn, def, parent, map);
                                        }
                                    }
                                    else if (thing.def.EverHaulable)
                                    {
                                        DamageEntities(thing, thing.Position, damageEntity, parent);
                                    }
                                    else
                                    if (!def.friendlyTo.Contains(thing.def))
                                    {
                                        DamageBuildings(thing, thing.Position, damageBuilding, parent);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }     

        public void DamageEntities(Thing thing, IntVec3 c, DamageInfo damage, Plant parent)
        {
            Thing t = thing;
            if (t.def.EverHaulable)
            {
                if (!t.def.defName.Contains("Tiberium"))
                {
                    t.TakeDamage(damage);
                    if (t != null)
                    {
                        DamageChunks(t, damage, parent);
                    }
                }
            }
            if (t.def.IsCorpse)
            {
                Corpse body = (Corpse)t;
                if (Rand.Chance(0.75f) && !body.InnerPawn.def.defName.Contains("_TBI") && !body.InnerPawn.RaceProps.IsMechanoid)
                {
                    if (body.AnythingToStrip())
                    {
                        body.Strip();
                    }
                    SpawnFiendOrVisceroid(c, body.InnerPawn);
                    t.Destroy(DestroyMode.Vanish);
                    return;
                }
            }
        }

        public void DamageChunks(Thing p, DamageInfo damage, Plant parent)
        {
            if (p.def.defName.Contains("Chunk") && Rand.Chance(0.05f))
            {
                ThingDef rock = null;
                switch (parent.def.defName)
                {
                    case "TiberiumGreen":
                        rock = ThingDef.Named("GreenTiberiumChunk");
                        break;

                    case "TiberiumBlue":
                        rock = ThingDef.Named("BlueTiberiumChunk");
                        break;

                    case "TiberiumRed":
                        rock = ThingDef.Named("RedTiberiumChunk");
                        break;

                    case "TiberiumDesertGreen":
                        rock = ThingDef.Named("GreenTiberiumChunk");
                        break;

                    case "TiberiumDesertBlue":
                        rock = ThingDef.Named("BlueTiberiumChunk");
                        break;

                    case "TiberiumDesertRed":
                        rock = ThingDef.Named("RedTiberiumChunk");
                        break;

                    case "TiberiumPod":
                        rock = ThingDef.Named("GreenTiberiumChunk");
                        break;

                    case "TiberiumVein":
                        return;

                    case "TiberiumMossGreen":
                        return;

                    case "TiberiumMossBlue":
                        return;

                    case "TiberiumGlacier":
                        return;
                }
                IntVec3 loc = p.Position;
                if (loc.InBounds(map))
                {
                    p.Destroy(DestroyMode.Vanish);
                    GenSpawn.Spawn(rock, loc, map);
                    TiberiumCrystalDef def = (TiberiumCrystalDef)parent.def;
                    map.terrainGrid.SetTerrain(loc, def.corruptsInto);
                    return;
                }
            }
            p.TakeDamage(damage);
        }

        public virtual void SpawnFiendOrVisceroid(IntVec3 pos, Pawn p)
        {
            Pawn pawn = null;
            if (Rand.Chance(0.25f))
            {
                PawnKindDef creature = null;
                switch (p.def.defName)
                {
                    case "Thrumbo":
                        if (Rand.Chance(0.2f))
                        {
                            creature = PawnKindDef.Named("TiberiumTerror_TBI");
                            break;
                        }
                        creature = PawnKindDef.Named("Thrimbo_TBI");
                        break;

                    case "GrizzlyBear":
                        creature = PawnKindDef.Named("BigTiberiumFiend_TBI");
                        break;

                    case "PolarBear":
                        creature = PawnKindDef.Named("BigTiberiumFiend_TBI");
                        break;

                    case "Megascarab":
                        creature = PawnKindDef.Named("Tibscarab_TBI");
                        break;

                    case "WolfTimber":
                        creature = PawnKindDef.Named("TiberiumFiend_TBI");
                        break;

                    case "WolfArctic":
                        creature = PawnKindDef.Named("TiberiumFiend_TBI");
                        break;

                    case "FoxFennec":
                        creature = PawnKindDef.Named("SmallTiberiumFiend_TBI");
                        break;

                    case "FoxRed":
                        creature = PawnKindDef.Named("SmallTiberiumFiend_TBI");
                        break;

                    case "FoxArctic":
                        creature = PawnKindDef.Named("SmallTiberiumFiend_TBI");
                        break;

                    case "Cobra":
                        creature = PawnKindDef.Named("Crawler_TBI");
                        break;

                    case "Boomrat":
                        creature = PawnKindDef.Named("Boomfiend_TBI");
                        break;

                    case "Muffalo":
                        creature = PawnKindDef.Named("Tiffalo_TBI");
                        break;

                    case "Warg":
                        creature = PawnKindDef.Named("Spiner_TBI");
                        break;

                    default:
                        creature = PawnKindDef.Named("Visceroid_TBI");
                        break;
                }
                PawnGenerationRequest request = new PawnGenerationRequest(creature);
                pawn = PawnGenerator.GeneratePawn(request);
            }
            else
            {
                PawnKindDef Visceroid = PawnKindDef.Named("Visceroid_TBI");
                PawnGenerationRequest request = new PawnGenerationRequest(Visceroid);
                pawn = PawnGenerator.GeneratePawn(request);
            }
            GenSpawn.Spawn(pawn, pos, map, p.Rotation);
        }

        public void DamageBuildings(Thing thing, IntVec3 c, DamageInfo damage, Plant parent)
        {
            if (!thing.def.defName.Contains("TBNS"))
            {
                if (thing.def.mineable && Rand.Chance(0.02f))
                {
                    CorruptWall(thing, parent);
                    return;
                }
                thing.TakeDamage(damage);
            }

            if (thing.def == ThingDefOf.Grave)
            {
                thing.Destroy(DestroyMode.Deconstruct);
            }

            if (thing.def == ThingDefOf.SteamGeyser)
            {
                ThingDef tibG = ThingDef.Named("TiberiumGeyser");
                IntVec3 loc = thing.Position;
                thing.DeSpawn();
                GenSpawn.Spawn(tibG, loc, map);
            }
        }

        public virtual void CorruptWall(Thing thing, Plant parent)
        {
            IntVec3 loc = thing.Position;
            if (thing != null)
            {
                ThingDef wall = null;
                switch (parent.def.defName)
                {
                    case "TiberiumGreen":
                        wall = ThingDef.Named("GreenTiberiumRock_TBNS");
                        break;

                    case "TiberiumBlue":
                        wall = ThingDef.Named("BlueTiberiumRock_TBNS");
                        break;

                    case "TiberiumRed":
                        wall = ThingDef.Named("RedTiberiumRock_TBNS");
                        break;

                    case "TiberiumPod":
                        return;

                    case "TiberiumDesertGreen":
                        wall = ThingDef.Named("GreenTiberiumRock_TBNS");
                        break;

                    case "TiberiumDesertBlue":
                        wall = ThingDef.Named("BlueTiberiumRock_TBNS");
                        break;

                    case "TiberiumDesertRed":
                        wall = ThingDef.Named("RedTiberiumRock_TBNS");
                        break;

                    case "TiberiumMossGreen":
                        wall = ThingDef.Named("GreenTiberiumRock_TBNS");
                        break;

                    case "TiberiumMossBlue":
                        wall = ThingDef.Named("BlueTiberiumRock_TBNS");
                        break;

                    case "TiberiumVein":
                        wall = ThingDef.Named("VeinTiberiumRock_TBNS");
                        break;

                    case "TiberiumGlacier":
                        return;
                }

                if (loc.InBounds(map))
                {
                    if (thing.def.mineable && !thing.def.defName.Contains("TBNS"))
                    {
                        thing.Destroy(DestroyMode.Vanish);
                        GenSpawn.Spawn(wall, loc, map);
                    }
                }
            }
            return;
        }
    }
}