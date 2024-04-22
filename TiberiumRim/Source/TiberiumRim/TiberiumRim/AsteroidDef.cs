using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class AsteroidDef : IncidentDef
    {
        public ThingDef asteroidType;

        public ThingDef skyfallerDef;

        public ThingDef tiberiumType;

        public string sourceThing;

        public bool spawnCrystals = false;
    }
}