using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Comp_TiberiumProducer : ThingComp
    {
        private CompProperties_TiberiumProducer def;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.def = (CompProperties_TiberiumProducer)this.props;

            setGrowthRadius();
            removeGrass();
            spawnTerrain(def.corruptsInto);
            CheckLists.producerAmt += 1;
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            CheckLists.producerAmt -= 1;
            base.PostDeSpawn(map);
        }

        public override void PostExposeData()
        {
            CheckLists.producerAmt = 0;
            CheckLists.AllowedCells.Clear();
            base.PostExposeData();
        }

        public override void CompTickRare()
        {
            SnowUtility.AddSnowRadial(this.parent.OccupiedRect().RandomCell, this.parent.Map, 14f, -0.08f);
            DestroyWalls();
            base.CompTickRare();
        }

        public void setGrowthRadius()
        {
            int num = GenRadial.NumCellsInRadius(this.def.radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 positionToCheck = this.parent.Position + GenRadial.RadialPattern[i];
                CheckLists.AllowedCells.Add(positionToCheck);
            }
        }

        public void DestroyWalls()
        {
            var c = this.parent.CellsAdjacent8WayAndInside();
            foreach (IntVec3 d in c)
            {
                if (d.InBounds(this.parent.Map))
                {
                    var p = d.GetFirstBuilding(this.parent.Map);

                    if (p != null)
                    {
                        int amt = 150;

                        DamageInfo damage = new DamageInfo(DamageDefOf.Mining, amt);

                        if (!p.def.defName.Contains("TBNS"))
                        {
                            p.TakeDamage(damage);
                        }
                    }
                }
            }
        }

        public void spawnTerrain(TerrainDef setTerrain)
        {
            foreach (IntVec3 inside in this.parent.CellsAdjacent8WayAndInside())
            {
                if (inside.InBounds(this.parent.Map))
                {
                    this.parent.Map.terrainGrid.SetTerrain(inside, setTerrain);
                }
            }
        }

        public void removeGrass()
        {
            foreach (IntVec3 inside in this.parent.CellsAdjacent8WayAndInside())
            {
                if (inside.InBounds(this.parent.Map))
                {
                    Plant plant = inside.GetPlant(this.parent.Map);
                    if (plant != null)
                    {
                        plant.Destroy();
                    }
                }
            }
        }
    }

    class CompProperties_TiberiumProducer : CompProperties
    {
        public int radius;

        public TerrainDef corruptsInto;

        public CompProperties_TiberiumProducer()
        {
            this.compClass = typeof(Comp_TiberiumProducer);
        }
    }
}
