using Verse;

namespace TR;

public class ResearchModExtension : DefModExtension
{
    public Requisites requisites;
    
    public bool IsFinished => requisites.FulFilled();
}