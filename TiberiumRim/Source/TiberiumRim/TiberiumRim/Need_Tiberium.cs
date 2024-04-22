using System.Collections.Generic;
using System.Linq;
using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace TiberiumRim
{
    public class Need_Tiberium : Need
    {
        private const float ThreshDesire = 0.01f;

        private const float ThreshSatisfied = 0.3f;

        private float messageTick;

        public bool canDoJob
        {
            get
            {
                if (this.pawn.Downed)
                {
                    return false;
                }
                else if (this.pawn.CarriedBy != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool isInTiberium
        {
            get
            {
                IntVec3 c = this.pawn.Position;
                if (c.InBounds(this.pawn.Map))
                {
                    Plant p = c.GetPlant(this.pawn.Map);
                    if (p != null)
                    {
                        if (p.def.defName.Contains("Tiberium"))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool hasTiberAdd
        {
            get
            {
                return this.pawn.health.hediffSet.HasHediff(TiberiumHediffDefOf.TiberAddHediff);
            }
        }

        public override int GUIChangeArrow
        {
            get
            {
                if (isInTiberium || hasTiberAdd)
                {
                    return 1;
                }
                return -1;
            }
        }

        public TiberiumNeedCategory CurCategory
        {
            get
            {
                if (this.CurLevel > 0.85f)
                {
                    return TiberiumNeedCategory.Statisfied;
                }
                if (this.CurLevel > 0.45f)
                {
                    return TiberiumNeedCategory.Lacking;
                }
                if (this.CurLevel > 0f)
                {
                    return TiberiumNeedCategory.Urgent;
                }
                return 0;
            }
        }

        private bool canMessage
        {
            get
            {
                return messageTick > 125;
            }

        }

        public override float CurLevel
        {
            get
            {
                return base.CurLevel;
            }
            set
            {
                TiberiumNeedCategory curCategory = this.CurCategory;
                base.CurLevel = value;
            }
        }

        private float TiberiumNeedFallPerTick
        {
            get
            {
                return this.def.fallPerDay / 60000f;
            }
        }

        public Need_Tiberium(Pawn pawn) : base(pawn)
        {
            this.threshPercents = new List<float>();
            this.threshPercents.Add(0.45f);
            this.threshPercents.Add(0.85f);
            this.threshPercents.Add(0f);
        }

        public override void SetInitialLevel()
        {
            base.CurLevelPercentage = Rand.Range(0.2f, 0.5f);
        }

        public void GiveJob()
        {
            messageTick += 1;
            if (this.CurLevel < this.MaxLevel * 0.5)
            {
                JobDef job = DefDatabase<JobDef>.GetNamed("TiberiumBath");
                if (this.pawn.jobs.curJob.def != null)
                {
                    if (this.pawn.jobs.curJob.def != job)
                    {
                        for (int i = 0; i > this.pawn.jobs.jobQueue.Count; i++)
                        {
                            JobQueue jobQ = this.pawn.jobs.jobQueue;
                            if (jobQ[i].job.def != null)
                            {
                                if (jobQ[i].job.def == job)
                                {
                                    return;
                                }
                            }
                        }

                        Predicate<Thing> validator = delegate (Thing x)
                        {
                            bool flag = false;
                            if (x.def.defName.Contains("Tiberium") && x.def.thingClass == typeof(TiberiumCrystal))
                            {
                                flag = true;
                            }
                            return flag;
                        };

                        Thing targetA = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Plant), PathEndMode.OnCell, TraverseParms.For(TraverseMode.PassDoors), 9999f, validator);
                        if (targetA != null)
                        {
                            if (pawn.CanReach(targetA, PathEndMode.OnCell, Danger.Deadly, false))
                            {
                                this.pawn.jobs.jobQueue.EnqueueLast(new Job(job, targetA), JobTag.Misc);
                            }
                        }
                        if (canMessage && this.pawn.Faction.IsPlayer)
                        {
                            Messages.Message("CannotReachTiberium".Translate(), new TargetInfo(pawn.Position, pawn.Map, false), MessageTypeDefOf.CautionInput);
                            messageTick = 0;
                        }
                    }
                }
            }
        }

        public override void NeedInterval()
        {
            if (this.pawn?.Map != null)
            {
                if (this.pawn.Position.InBounds(this.pawn.Map))
                {
                    if (this.pawn.CarriedBy != null)
                    {
                        return;
                    }
                    if (isInTiberium)
                    {
                        this.CurLevel += 0.05f;
                    }
                    else
                    {
                        this.CurLevel -= this.TiberiumNeedFallPerTick * 350f;
                    }
                    if (canDoJob)
                    {
                        GiveJob();
                    }
                }
            }
        }
    }

    public enum TiberiumNeedCategory : byte
    {
        Statisfied,
        Lacking,
        Urgent
    }
}

