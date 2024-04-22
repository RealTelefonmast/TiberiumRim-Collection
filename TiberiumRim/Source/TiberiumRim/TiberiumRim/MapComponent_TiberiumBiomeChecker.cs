using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TiberiumRim
{
    public class MapComponent_TiberiumBiomeChecker : MapComponent
    {
        public WorldComponent_TiberiumSpread worldComp = Find.World.GetComponent<WorldComponent_TiberiumSpread>();

        public HashSet<int> tiberCount = new HashSet<int>();

        private int mapTiles;

        public MapComponent_TiberiumBiomeChecker(Map map) : base(map)
        {
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<int>(ref this.tiberCount, "tiberCount", LookMode.Value);
            base.ExposeData();
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (Find.TickManager.TicksGame % 500 == 0)
            {
                SetPct();
                Check();
            }
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            if (tiberCount.Count < 1)
            {
                tiberCount = new HashSet<int>();
            }
            mapTiles = (this.map.Size.ToIntVec2.x * this.map.Size.ToIntVec2.z);
        }

        private int mapTile
        {
            get
            {
                return this.map.Tile;
            }
        }

        public float TileCount
        {
            get
            {
                int tilesCovered = 0;
                tilesCovered = this.map.listerThings.AllThings.Where((Thing x) => x.def.coversFloor == true).Sum(x => x.def.size.x * x.def.size.z);
                return (mapTiles - tilesCovered);
            }
        }

        public bool IsRedZone
        {
            get
            {
                return map.Biome.defName.Contains("TiberiumRedZone");
            }
        }

        public void SetPct()
        {
            if (!worldComp.tiberiumPcts.ContainsKey(mapTile))
            {
                worldComp.tiberiumPcts.Add(mapTile, ((float)tiberCount.Count / TileCount));
            }
            else
            {
                worldComp.tiberiumPcts[mapTile] = ((float)tiberCount.Count / TileCount);
            }

        }

        public void Check()
        {
            if (IsRedZone != true)
            {
                if (worldComp.tiberiumPcts[mapTile] > 0f)
                {
                    if (!worldComp.TileID.Contains(mapTile))
                    {
                        worldComp.TileID.Add(mapTile);
                    }
                }
            }
        }
    }
}