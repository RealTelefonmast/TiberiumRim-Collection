using UnityEngine;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class Comp_GroundFreezer : CompTerrainPump
    {
        private CompPowerTrader powerComp;

        private int progressTicks;

        private CompProperties_GroundFreezer Props
        {
            get
            {
                return (CompProperties_GroundFreezer)this.props;
            }
        }

        private float ProgressDays
        {
            get
            {
                return (float)this.progressTicks / 60000f;
            }
        }

        private float CurrentRadius
        {
            get
            {
                return Mathf.Min(this.Props.radius, this.ProgressDays / this.Props.daysToRadius * this.Props.radius);
            }
        }

        private bool Working
        {
            get
            {
                if (this.Props.requiresElectricity)
                {
                    return this.powerComp == null || this.powerComp.PowerOn;
                }
                return true;
            }
        }

        private int TicksUntilRadiusInteger
        {
            get
            {
                float num = Mathf.Ceil(this.CurrentRadius) - this.CurrentRadius;
                if (num < 1E-05f)
                {
                    num = 1f;
                }
                float num2 = this.Props.radius / this.Props.daysToRadius;
                float num3 = num / num2;
                return (int)(num3 * 60000f);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.powerComp = this.parent.TryGetComp<CompPowerTrader>();
        }

        public override void PostDeSpawn(Map map)
        {
            this.progressTicks = 0;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.progressTicks, "progressTicks", 0, false);
        }

        public override void CompTickRare()
        {
            if (this.Working)
            {
                this.progressTicks += 250;
                int num = GenRadial.NumCellsInRadius(this.CurrentRadius);

                for (int i = 0; i < num; i++)
                {
                    IntVec3 positionToCheck = this.parent.Position + GenRadial.RadialPattern[i];
                    this.AffectCell(positionToCheck);
                }
            }
        }

        protected override void AffectCell(IntVec3 c)
        {
            if (c.InBounds(this.parent.Map))
            {
                TerrainDef terrain = c.GetTerrain(this.parent.Map);
                if (terrain != null)
                {
                    if (!terrain.Removable)
                    {
                        this.parent.Map.snowGrid.AddDepth(c, 0.095f);
                        TerrainDef Postterrain = null;
                        if (terrain.defName.Contains("Water") | terrain.defName.Contains("Marsh"))
                        {
                            Postterrain = TerrainDef.Named("Ice");
                            this.parent.Map.terrainGrid.SetTerrain(c, Postterrain);
                        }
                        Plant p = c.GetPlant(this.parent.Map);
                        if (p != null && !p.def.defName.Contains("Tiberium"))
                        {
                            p.Destroy(DestroyMode.Vanish);
                        }
                        return;
                    }
                }
            }
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            if (this.CurrentRadius < this.Props.radius - 0.0001f)
            {
                GenDraw.DrawRadiusRing(this.parent.Position, this.CurrentRadius);
            }
        }

        public override string CompInspectStringExtra()
        {
            string text = string.Concat(new string[]
            {
                "TimePassed".Translate().CapitalizeFirst(),
                ": ",
                this.progressTicks.ToStringTicksToPeriod(true),
                "\n",
                "CurrentRadius".Translate().CapitalizeFirst(),
                ": ",
                this.CurrentRadius.ToString("F1")
            });
            if (this.ProgressDays < this.Props.daysToRadius && this.Working)
            {
                string text2 = text;
                text = string.Concat(new string[]
                {
                    text2,
                    "\n",
                    "RadiusExpandsIn".Translate().CapitalizeFirst(),
                    ": ",
                    this.TicksUntilRadiusInteger.ToStringTicksToPeriod(true)
                });
            }
            return text;
        }
    }

    public class CompProperties_GroundFreezer : CompProperties
    {
        public float radius = 9.9f;

        public float daysToRadius = 1f;

        public bool requiresElectricity;

        public CompProperties_GroundFreezer()
        {
            this.compClass = typeof(Comp_GroundFreezer);
        }
    }
}
