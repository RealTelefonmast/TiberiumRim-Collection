using Verse;

namespace TR;

/*  ResearchWorker is going to contain valuable data such as culprits for events
 *  ResearchWorkers can vary in their completion task
 *
 */
public class ResearchWorker
{
    public TResearchTaskDef def;
    public TargetInfo Culprit;

    public ResearchWorker(){}

    public ResearchWorker(TResearchTaskDef def)
    {
        this.def = def;
    }

    public void RegisterCulprit(Thing thing)
    {
        Culprit = new TargetInfo(thing);
    }

    //The task check acts allows the task to have a goal for the player
    public virtual bool PlayerTaskCompleted()
    {
        return true;
    }

    public virtual void FinishAction()
    {
    }
}