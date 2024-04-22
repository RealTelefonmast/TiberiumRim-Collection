using Verse;

namespace TR;

public static class HediffUtils
{
    public static bool IsMechanoid(this Pawn pawn)
    {
        return pawn?.kindDef?.RaceProps?.IsMechanoid ?? false;
    }
}