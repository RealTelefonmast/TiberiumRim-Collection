using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace TiberiumRim
{
    public class JobGiver_Tiberium : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Thing targetA = pawn.Map.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("Tiberium") && x is Plant);
            JobDef job = DefDatabase<JobDef>.GetNamed("TiberiumBath");
            return new Job(job, targetA);
        }
    }

    public abstract class JobDriver_TiberiumBath : JobDriver
    {
        private Toil bath;

        public override PawnPosture Posture
        {
            get
            {
                return (base.CurToil != this.bath) ? PawnPosture.Standing : PawnPosture.LayingAny;
            }
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            this.bath = new Toil();
            this.bath.tickAction = delegate
            {
                if (this.pawn.needs.joy != null)
                {
                    JoyUtility.JoyTickCheckEnd(this.pawn, JoyTickFullJoyAction.EndJob, 1f);
                }
            };
            this.bath.defaultCompleteMode = ToilCompleteMode.Delay;
            this.bath.defaultDuration = base.job.def.joyDuration;
            this.bath.FailOn(() => !this.pawn.Position.GetPlant(Map).def.defName.Contains("Tiberium"));
            yield return this.bath;

        }
    }
}
