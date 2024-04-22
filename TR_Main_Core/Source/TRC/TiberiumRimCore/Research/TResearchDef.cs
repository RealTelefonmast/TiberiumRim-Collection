using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TR;

public class TResearchDef : Def
    {
        [Unsaved()]
        private TResearchGroupDef parentGroup;

        public Requisites requisites;

        public TargetProperties mainTarget;
        public WorkTypeDef workType;
        public List<SkillRequirement> skillRequirements;
        public StatDef relevantPawnStat;
        public StatDef relevantTargetStat;

        public List<TResearchTaskDef> tasks;
        public List<EventDef> events;
        public string researchType = "missing";
        public string projectDescription;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            //workType = DefDatabase<WorkTypeDef>.GetNamed("Research");
            //relevantPawnStat = StatDefOf.ResearchSpeed;
            //relevantTargetStat = StatDefOf.ResearchSpeedFactor;
        }

        //TODO: MULTIPLAYER
        //[SyncMethod(SyncContext.None)]
        public void TriggerEvents()
        {
            if (events.NullOrEmpty()) return;
            foreach (var @event in events)
            {
                TRUtils.EventManager().StartEvent(@event);
            }
        }
        
        
        public virtual void FinishAction()
        {
        }

        //TODO: MULTIPLAYER
        //[SyncMethod(SyncContext.None)]
        public void Debug_Finish()
        {
            TRLog.Debug($"Force Finishing TResearch '{LabelCap}'");
            tasks.ForEach(t => t.Debug_Finish());
        }

        //[SyncMethod(SyncContext.None)]
        public void Debug_Reset()
        {
            if (!tasks.NullOrEmpty())
                tasks.ForEach(t => t.Debug_Reset());
            if (!events.NullOrEmpty())
                events.ForEach(t => TRUtils.EventManager().ResetEvent(t));
        }

        public virtual bool RequisitesComplete => requisites?.FulFilled() ?? true;
        public virtual bool CanStartNow => RequisitesComplete;
        public bool IsFinished => TRUtils.ResearchManager().IsCompleted(this);
        public bool HasBeenSeen => TRUtils.ResearchDiscoveryTable().ResearchHasBeenSeen(this);

        public TResearchGroupDef ParentGroup
        {
            get
            {
                return parentGroup ??= DefDatabase<TResearchGroupDef>.AllDefsListForReading.FirstOrDefault((r) => r.researchProjects.Contains(this));
            }
        }

        public virtual TResearchTaskDef CurrentTask
        {
            get
            {
                return tasks.FirstOrDefault((task) => !task.IsFinished);
            }
        }

        public ResearchState State
        {
            get
            {
                if (Equals(TRUtils.ResearchManager().CurrentProject))
                    return ResearchState.InProgress;
                if (IsFinished)
                    return ResearchState.Finished;
                if (CanStartNow)
                    return ResearchState.Available;
                return ResearchState.Hidden;
            }
        }
    }
