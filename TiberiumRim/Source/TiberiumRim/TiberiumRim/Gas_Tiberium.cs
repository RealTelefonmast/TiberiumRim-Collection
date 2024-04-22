using System.Collections.Generic;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Gas_Tiberium : Gas
    {

        public override void Tick()
        {
            IntVec3 c = this.Position;
            if (c.InBounds(Map))
            {
                List<Thing> thinglist = c.GetThingList(Map);
                foreach (Thing t in thinglist)
                {
                    Pawn p = t as Pawn;
                    if (p != null)
                    {
                        TiberiumUtility.Infect(p, 0.0001f, Map, true);
                    }
                }
            }
            base.Tick();
        }
    }
}
