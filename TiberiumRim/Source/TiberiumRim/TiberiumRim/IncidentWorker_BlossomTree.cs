using RimWorld;
using Verse;
using Verse.AI;

namespace TiberiumRim
{
    public class IncidentWorker_BlossomTree : IncidentWorker
    {
        IntVec3 cell = IntVec3.Invalid;

        protected override bool CanFireNowSub(IIncidentTarget target)
        {
            Map map = (Map)target;
            Thing Tree = map.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("BlossomTree"));
            if (Tree != null)
            {
                return true;
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal + 0.2f, null))
            {
                return false;
            }
            Pawn pawn = null;

            for (int i = 0; i < 12; i++)
            {
                PawnKindDef creature = Selectcreature();
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
                pawn = PawnGenerator.GeneratePawn(creature, null);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random, false);

                IntVec3 pos = map.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("BlossomTree")).Position;
                IntVec3 pos2 = pos.RandomAdjacentCell8Way();

                if (Rand.Chance(0.15f))
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true, false, null);
                }
                else
                    pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.GotoWander, pos2));
            }
            Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, new TargetInfo(cell, map, false), null);
            return true;
        }

        private Pawn FindPawnTarget(Pawn pawn)
        {
            return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedThreat, (Thing x) => x is Pawn && x.def.race.intelligence >= Intelligence.ToolUser, 0f, 9999f, default(IntVec3), 3.40282347E+38f, true);
        }

        public PawnKindDef Selectcreature()
        {
            PawnKindDef creature = null;

            switch (Rand.Range(1, 10))
            {
                case 1:
                    if (Rand.Chance(0.3f))
                    {
                        creature = PawnKindDef.Named("TiberiumTerror_TBI");
                    }
                    break;

                case 2:
                    if (Rand.Chance(0.5f))
                    creature = PawnKindDef.Named("Thrimbo_TBI");
                    break;

                case 3:
                    creature = PawnKindDef.Named("BigTiberiumFiend_TBI");
                    break;

                case 4:
                    creature = PawnKindDef.Named("Tibscarab_TBI");
                    break;

                case 5:
                    creature = PawnKindDef.Named("TiberiumFiend_TBI");
                    break;


                case 6:
                    creature = PawnKindDef.Named("SmallTiberiumFiend_TBI");
                    break;


                case 7:
                    creature = PawnKindDef.Named("Crawler_TBI");
                    break;

                case 8:
                    creature = PawnKindDef.Named("Boomfiend_TBI");
                    break;

                case 9:
                    creature = PawnKindDef.Named("Tiffalo_TBI");
                    break;

                case 10:
                    creature = PawnKindDef.Named("Spiner_TBI");
                    break;

            }
            return creature;
        }
    }
}