using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class HediffComp_TiberAdd : HediffComp
    {
        public HediffCompProperties_TiberAdd Props
        {
            get
            {
                return (HediffCompProperties_TiberAdd)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            Need_Tiberium N = (Need_Tiberium)this.Pawn.needs.AllNeeds.Find((Need x) => x.def.defName.Contains("Need_Tiberium"));
            if (N != null)
            {
                N.CurLevelPercentage += 0.05f;
            }
        }
    }

    public class HediffCompProperties_TiberAdd : HediffCompProperties
    {
        public HediffCompProperties_TiberAdd()
        {
            this.compClass = typeof(HediffComp_TiberAdd);
        }
    }
}
