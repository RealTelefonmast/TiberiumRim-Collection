using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TR;

public class TResearchGroupDef : Def
{
    public int priority = 0;
    public List<TResearchDef> researchProjects;

    public List<TResearchDef> ActiveProjects => researchProjects.NullOrEmpty() ? null : researchProjects.Where(t => t.RequisitesComplete).ToList();

    public bool IsVisible => !IsFinished && !ActiveProjects.NullOrEmpty() || (IsFinished && !TResearchManager.hideGroups);
    public bool IsFinished => researchProjects.NullOrEmpty() || researchProjects.All(r => r.IsFinished);

    public bool HasUnseenProjects => ActiveProjects.Any(t => !t.HasBeenSeen);
}