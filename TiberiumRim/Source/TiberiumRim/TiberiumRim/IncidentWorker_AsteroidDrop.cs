using RimWorld;
using System;
using Verse;

namespace TiberiumRim
{
    public class IncidentWorker_AsteroidDrop : IncidentWorker
    {
        protected override bool CanFireNowSub(IIncidentTarget target)
        {
            Map map = (Map)target;
            IntVec3 intVec;
            return this.TryFindCell(out intVec, map);
        }

        private bool TryFindCell(out IntVec3 cell, Map map)
        {
            AsteroidDef Localdef = this.def as AsteroidDef;
            return CellFinderLoose.TryFindSkyfallerCell(Localdef.skyfallerDef, map, out cell, 10, default(IntVec3), -1, true, false, false, false, delegate (IntVec3 x)
            {
                if (TiberiumRimSettings.settings.UseProducerCap)
                {
                    if (CheckLists.producerAmt >= TiberiumRimSettings.settings.TiberiumProducersAmt)
                    {
                        if (Localdef.asteroidType != null)
                        {
                            if (!Localdef.asteroidType.defName.Contains("TBNS"))
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
                Plant plant = x.GetPlant(map);
                TerrainDef terrain = x.GetTerrain(map);

                if (plant != null && x.InBounds(map))
                {
                    if (plant.def.defName.Contains("Tiberium"))
                    {
                        return false;
                    }
                }

                if (terrain != null && x.InBounds(map))
                {
                    if (terrain.defName.Contains("Water") || terrain.defName.Contains("Marsh"))
                    {
                        return false;
                    }
                }

                if (x.Fogged(map))
                {
                    return false;
                }

                foreach (IntVec3 current in GenAdj.CellsOccupiedBy(x, Rot4.North, new IntVec2(4,4)))
                {
                    if (!current.Standable(map))
                    {
                        return false;
                    }
                    if (map.roofGrid.Roofed(current))
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            AsteroidDef Localdef = this.def as AsteroidDef;
            Map map = (Map)parms.target;
            IntVec3 intVec;
            bool result;
            if (!this.TryFindCell(out intVec, map))
            {
                result = false;
            }
            else
            {
                SkyfallerMaker.SpawnSkyfaller(Localdef.skyfallerDef, Localdef.asteroidType, intVec, map);
                Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, new TargetInfo(intVec, map, false), null);
                result = true;
            }
            return result;
        }
    }
}
