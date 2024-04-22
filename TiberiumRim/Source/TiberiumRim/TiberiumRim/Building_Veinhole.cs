using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Building_Veinhole : Building
    {
        private int radius = 30;

        private int hubsInt;

        private int spawnTicks;

        private List<Thing> hubs = new List<Thing>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.hubsInt, "progressTicks", 0, false);
            Scribe_Collections.Look<Thing>(ref this.hubs, "hubs", LookMode.Reference);
        }

        public static void destroyVeins(List<IntVec3> checkCoords)
        {
            checkCoords.ForEach(i =>
            {
                if (i.InBounds(Find.VisibleMap))
                {
                    Thing thing = i.GetFirstThing(Find.VisibleMap, ThingDef.Named("TiberiumVein"));
                    if (!thing.DestroyedOrNull())
                    {
                        thing.Destroy(DestroyMode.Vanish);
                        IntVec3[] cells = GenAdj.AdjacentCells;
                        destroyVeins(new List<IntVec3>() { i + cells[0], i + cells[1], i + cells[2], i + cells[3], i + cells[4], i + cells[5], i + cells[6], i + cells[7] });
                    }
                }
            });
        }

        public void killHubs()
        {
            foreach (Thing t in hubs)
            {
                if (t != null)
                {
                    t.Destroy();
                }
            }
        }

        public bool canSpawnHubNow
        {
            get
            {
                return spawnTicks > 260;
            }
        }

        public override void TickRare()
        {
            spawnTicks += 1;
            growHub();
            spawnVeinmonster();
            base.TickRare();
        }

        private void growHub()
        {
            if (canSpawnHubNow)
            {
                spawnTicks = 0;
                if (hubsInt < 6)
                {
                    int num = GenRadial.NumCellsInRadius(this.radius);

                    int rand = Rand.Range(1, num);
                    IntVec3 pos = this.Position + GenRadial.RadialPattern[rand];
                    if (pos.InBounds(Map))
                    {
                        List<Thing> thinglist = pos.GetThingList(Map);
                        Thing hub = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("VeinholeHub_TBNS"), null);
                        foreach (Thing t in thinglist)
                        {
                            if (t.def != hub.def)
                            {
                                if (t.def.passability == Traversability.Standable)
                                {
                                    if (!pos.GetTerrain(Map).defName.Contains("Water") || !pos.GetTerrain(Map).defName.Contains("Marsh"))
                                    {
                                        GenSpawn.Spawn(hub, pos, Map);
                                        hubsInt += 1;
                                        hubs.Add(hub);
                                        Messages.Message("VeinHubSpawned".Translate(), new TargetInfo(hub.Position, Map, false), MessageTypeDefOf.CautionInput);
                                        return;
                                    }
                                }
                                growHub();
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void spawnVeinmonster()
        {
            IntVec3 pos = this.Position.RandomAdjacentCell8Way();

            int maximum = Map.listerThings.AllThings.FindAll((Thing x) => x.def.defName.Contains("Veinhole")).Count * 12;
            int Veinmonsters = Map.listerThings.AllThings.FindAll((Thing x) => x.def.defName.Contains("Veinmonster_TBI")).Count;

            if (Rand.Chance(0.001f) && Veinmonsters < maximum)
            {
                PawnKindDef Veinmonster = PawnKindDef.Named("Veinmonster_TBI");
                PawnGenerationRequest request = new PawnGenerationRequest(Veinmonster);
                Pawn pawn = PawnGenerator.GeneratePawn(request);
                GenSpawn.Spawn(pawn, pos, Map);
            }
        }

        private void EatPawn()
        {
            List<IntVec3> list = GenAdj.OccupiedRect(this).ExpandedBy(1).EdgeCells.ToList();
            IntVec3 c = list[Rand.Range(0, list.Count)];
            List<Thing> thingList = c.GetThingList(this.Map);

            foreach (Pawn p in thingList)
            {
                if (Rand.Chance(0.05f))
                {
                    Messages.Message("MessagePawnEaten".Translate(), new TargetInfo(p.Position, Map, false), MessageTypeDefOf.PawnDeath);
                    p.Destroy(DestroyMode.Vanish);
                }
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            destroyVeins(GenAdjFast.AdjacentCells8Way(this));
            killHubs();
            base.Destroy(mode);
        }
    }

    public class Building_Veinhub : Building
    {
        private int radius = 7;

        private bool floorNotSet = true;
        public override void TickRare()
        {
            int num = GenRadial.NumCellsInRadius(this.radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 positionToCheck = this.Position + GenRadial.RadialPattern[i];
                if (positionToCheck.InBounds(Map))
                {
                    if (!positionToCheck.GetTerrain(Map).Removable)
                    {
                        this.TryAttack(positionToCheck);
                        if (floorNotSet)
                        {
                            TerrainDef t = TerrainDef.Named("VeinSoilRed");
                            TerrainDef t2 = TerrainDef.Named("TiberiumSoilRed");
                            TerrainDef tt = positionToCheck.GetTerrain(Map);
                            if (tt.defName.Contains("Water") || tt.defName.Contains("Ice") || tt.defName.Contains("Marsh"))
                            {
                                Map.terrainGrid.SetTerrain(positionToCheck, tt);
                            }
                            else if (tt.defName.Contains("Sand"))
                            {
                                Map.terrainGrid.SetTerrain(positionToCheck, t2);
                            }
                            else
                            {
                                Map.terrainGrid.SetTerrain(positionToCheck, t);
                            }
                        }
                    }
                }
            }
            floorNotSet = false;
        }

        private void TryAttack(IntVec3 c)
        {
            if (c.InBounds(Map))
            {
                List<Thing> thinglist = c.GetThingList(this.Map);
                for (int i = 0; i < thinglist.Count; i++)
                {
                    if (thinglist[i] is Pawn)
                    {
                        ThingDef tentacle = ThingDef.Named("VeinTentacle");
                        if (c.GetFirstThing(Map, tentacle) == null)
                        {
                            GenSpawn.Spawn(tentacle, c, Map);
                        }
                    }
                }
            }
        }
    }
}