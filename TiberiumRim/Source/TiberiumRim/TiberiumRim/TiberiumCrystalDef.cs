using System.Collections.Generic;
using Verse;

namespace TiberiumRim
{
    public class TiberiumCrystalDef : ThingDef
    {
        public TerrainDef corruptsInto;

        public ThingDef Monolith;

        public int EntityDamage;

        public int BuildingDamage;

        public bool isFlesh;

        public List<ThingDef> friendlyTo = new List<ThingDef>();
    }
}
