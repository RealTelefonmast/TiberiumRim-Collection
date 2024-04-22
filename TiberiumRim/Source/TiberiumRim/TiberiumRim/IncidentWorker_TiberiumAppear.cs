using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class IncidentWorker_TiberiumAppear : IncidentWorker
    {
        private static readonly IntRange CountRange = new IntRange(10, 20);

        private const int SpawnRadius = 6;

        protected override bool CanFireNowSub(IIncidentTarget target)
        {
            bool result;
            if (!base.CanFireNowSub(target))
            {
                result = false;
            }
            else
            {
                Map map = (Map)target;
                IntVec3 intVec;
                result = (this.TryFindRootCell(map, out intVec));
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            AsteroidDef LocalDef = (AsteroidDef)this.def;
            Map map = (Map)parms.target;
            IntVec3 root;
            bool result = false;
            bool flag = true;
            if (TiberiumRimSettings.settings.UseProducerCap)
            {
                if (CheckLists.producerAmt >= TiberiumRimSettings.settings.TiberiumProducersAmt)
                {
                    if (LocalDef.asteroidType != null)
                    {
                        return false;
                    }
                }
                return true;
            }
            if (!this.TryFindRootCell(map, out root))
            {
                result = false;
            }
            else
            {
                if (LocalDef.spawnCrystals)
                {
                    if(LocalDef.tiberiumType == null)
                    {
                        Log.Error("Missing TiberiumType");
                    }
                    Thing thing = null;
                    int randomInRange = IncidentWorker_TiberiumAppear.CountRange.RandomInRange;
                    for (int i = 0; i < randomInRange; i++)
                    {
                        IntVec3 intVec;
                        if (!CellFinder.TryRandomClosewalkCellNear(root, map, 6, out intVec, (IntVec3 x) => this.CanSpawnAt(x, map)))
                        {
                            break;
                        }
                        Plant plant = intVec.GetPlant(map);
                        if (plant != null)
                        {
                            plant.Destroy(DestroyMode.Vanish);
                        }
                        Thing thing2 = GenSpawn.Spawn(LocalDef.tiberiumType, intVec, map);
                        if (thing == null)
                        {
                            thing = thing2;                            
                        }
                        if(thing == null)
                        {
                            flag = false;
                        }
                    }
                }
                if (flag)
                {                  
                    Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, new TargetInfo(root, map, false), null);
                    GenSpawn.Spawn(LocalDef.asteroidType, root, map);
                    result = true;           
                }
                else { result = false; }
            }
            return result;
        }

        private bool TryFindRootCell(Map map, out IntVec3 cell)
        {
            AsteroidDef LocalDef = (AsteroidDef)this.def;
            if (LocalDef.sourceThing != null)
            {
                Thing Tree = map.listerThings.AllThings.Find((Thing x) => x.def.defName.Contains("Tree") && x.def != LocalDef.asteroidType);
                if (Tree != null)
                {
                    cell = Tree.Position;
                    if (cell.InBounds(map))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => this.CanSpawnAt(x, map) && x.GetRoom(map, RegionType.Set_Passable).CellCount >= 64, map, out cell);
            }
            cell = IntVec3.Invalid;
            return false;
        }

        private bool CanSpawnAt(IntVec3 c, Map map)
        {
            bool result;
            if (!c.Standable(map) || c.Fogged(map) || !c.GetRoom(map, RegionType.Set_Passable).PsychologicallyOutdoors || c.GetEdifice(map) != null)
            {
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
}
