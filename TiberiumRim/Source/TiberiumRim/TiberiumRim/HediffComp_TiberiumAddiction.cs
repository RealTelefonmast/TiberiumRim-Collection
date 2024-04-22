using System.Linq;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class HediffComp_TiberiumAddiction : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (this.Pawn.CarriedBy == null)
            {
                Adjust(this.Pawn);
                return;
            }
        }

        public void Adjust(Pawn pawn)
        {
            if (pawn != null && pawn.Map != null)
            {
                if (pawn.Position.InBounds(pawn.Map))
                {
                    Need_Tiberium N = (Need_Tiberium)pawn.needs.AllNeeds.Find((Need x) => x.def.defName.Contains("Need_Tiberium"));
                    HediffDef Exposure = TiberiumHediffDefOf.TiberiumBuildupHediff;
                    if (N != null)
                    {
                        HealthUtility.AdjustSeverity(pawn, this.parent.def, -this.parent.Severity);
                        HealthUtility.AdjustSeverity(pawn, this.parent.def, 1 - N.CurLevelPercentage * 0.999999f);

                        if (N.isInTiberium)
                        {
                            Hediff hediff;
                            if ((from hd in pawn.health.hediffSet.hediffs where !hd.IsOld() && hd.def.hediffClass == typeof(Hediff_Injury) && !hd.def.defName.Contains("Tiberium") select hd).TryRandomElement(out hediff))
                            {
                                hediff.Heal(0.001f);
                            }
                        }
                    }

                    if (pawn.health.hediffSet.HasHediff(Exposure))
                    {
                        HealthUtility.AdjustSeverity(pawn, Exposure, -0.5f);
                    }
                }
            }
        }

        public HediffCompProperties_TiberiumAddiction Props
        {
            get
            {
                return (HediffCompProperties_TiberiumAddiction)this.props;
            }
        }
    }

    public class HediffCompProperties_TiberiumAddiction : HediffCompProperties
    {
        public HediffCompProperties_TiberiumAddiction()
        {
            this.compClass = typeof(HediffComp_TiberiumAddiction);
        }
    }
}

