using Verse;

namespace TiberiumRim
{
    public class HediffUtils
    {
        public static Hediff getMutation(Pawn p)
        {
            if (p.health.hediffSet.HasHediff(TiberiumHediffDefOf.TiberiumMutationBad))
            {
                return p.health.hediffSet.hediffs.Find((Hediff x) => x.def.defName == "TiberiumMutationBad");
            }
            else if (p.health.hediffSet.HasHediff(TiberiumHediffDefOf.TiberiumMutationGood))
            {
                return p.health.hediffSet.hediffs.Find((Hediff x) => x.def.defName == "TiberiumMutationGood");
            }
            else
            {
                return null;
            }
        }

        public static Hediff tibSickness(Pawn p)
        {
            if (p.health.hediffSet.HasHediff(TiberiumHediffDefOf.TiberiumBuildupHediff))
            {
                return p.health.hediffSet.hediffs.Find((Hediff x) => x.def.defName == "TiberiumBuildupHediff");
            }
            else if (p.health.hediffSet.HasHediff(TiberiumHediffDefOf.TiberiumStage1))
            {
                return p.health.hediffSet.hediffs.Find((Hediff x) => x.def.defName == "TiberiumStage1");
            }
            else if (p.health.hediffSet.HasHediff(TiberiumHediffDefOf.TiberiumStage2))
            {
                return p.health.hediffSet.hediffs.Find((Hediff x) => x.def.defName == "TiberiumStage2");
            }
            else if (p.health.hediffSet.HasHediff(TiberiumHediffDefOf.TiberiumContactPoison))
            {
                return p.health.hediffSet.hediffs.Find((Hediff x) => x.def.defName == "TiberiumContactPoison");
            }
            else
            {
                return null;
            }
        }
    }
}