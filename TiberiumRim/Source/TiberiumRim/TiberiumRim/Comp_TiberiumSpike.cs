using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Comp_TiberiumSpike : ThingComp
    {
        private Building_TibGeyser geyser;

        private CompPowerTrader powerComp;

        private int spawnTicks;

        private bool spawnForbidden = false;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.powerComp = this.parent.GetComp<CompPowerTrader>();
        }

        private bool canSpawn
        {
            get
            {
                return this.spawnTicks % 60 == 0;
            }
        }

        public override void CompTickRare()
        {
            spawnTicks += 1;
            if (this.geyser == null)
            {
                ThingDef tibgeyser = ThingDef.Named("TiberiumGeyser");
                this.geyser = (Building_TibGeyser)this.parent.Map.thingGrid.ThingAt(this.parent.Position, tibgeyser);
            }
            if (this.geyser != null)
            {
                this.geyser.harvester = (Building)this.parent;

                if (powerComp.PowerOn)
                {
                    if (canSpawn)
                    {
                        SpawnCrystals();
                    }
                }
            }
        }

        private void SpawnCrystals()
        {
            List<IntVec3> list = GenAdj.OccupiedRect(this.parent).ExpandedBy(1).EdgeCells.ToList();
            Thing crystalGreen = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("GreenTiberiumRaw"), null);
            Thing crystalBlue = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("BlueTiberiumRaw"), null);
            IntVec3 c = list[Rand.Range(0, list.Count)];
            List<Thing> thingList = c.GetThingList(this.parent.Map);

            if (Rand.Chance(0.75f))
            {
                TrySpawn(thingList, crystalGreen.def, c);
            }
            else
            {
                TrySpawn(thingList, crystalBlue.def, c);
            }
        }

        private void TrySpawn(List<Thing> list, ThingDef t, IntVec3 c)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].def == t && list[i].stackCount >= 24)
                {
                    return;
                }
            }
            if (c.GetFirstHaulable(this.parent.Map) == null || c.GetFirstHaulable(this.parent.Map).def == t)
            {
                Thing thing = ThingMaker.MakeThing(t, null);
                thing.stackCount = 25;
                Thing d;
                GenPlace.TryPlaceThing(thing, c, this.parent.Map, ThingPlaceMode.Direct, out d, null);
                if (spawnForbidden)
                {
                    thing.SetForbidden(true, true);
                }
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            if (this.geyser != null)
            {
                this.geyser.harvester = null;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
                yield return g;

            if (true)
            {
                Command_Action Forbid = new Command_Action()
                {
                    defaultLabel = "CommandForbid".Translate(),
                    icon = TexCommand.Forbidden,
                    hotKey = KeyBindingDefOf.CommandItemForbid,
                    activateSound = SoundDefOf.Click,
                    defaultDesc = (spawnForbidden ? "CommandForbiddenDesc".Translate() : "CommandNotForbiddenDesc".Translate())
                };
                Forbid.action = delegate
                {
                    this.spawnForbidden = !spawnForbidden;
                };
                yield return Forbid;
            }
        }
    }

    public class CompProperties_TiberiumSpike : CompProperties
    {
        public CompProperties_TiberiumSpike()
        {
            this.compClass = typeof(Comp_TiberiumSpike);
        }
    }
}