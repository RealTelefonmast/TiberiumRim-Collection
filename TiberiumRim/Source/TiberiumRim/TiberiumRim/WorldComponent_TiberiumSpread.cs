using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace TiberiumRim
{
    public class WorldComponent_TiberiumSpread : WorldComponent
    {
        private List<int> tmpNeighbors = new List<int>();

        public List<int> TileID = new List<int>();

        public HashSet<int> TileHashList = new HashSet<int>();

        public Dictionary<int, float> tiberiumPcts = new Dictionary<int, float>();

        public WorldComponent_TiberiumSpread(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<int, float>(ref this.tiberiumPcts, "tiberiumPcts", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look<int>(ref this.TileID, "TileID", LookMode.Value);
            Scribe_Collections.Look<int>(ref this.TileHashList, "TileHashList", LookMode.Value);
            Scribe_Collections.Look<int>(ref this.tmpNeighbors, "tmpNeighbors", LookMode.Value);
            base.ExposeData();
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            foreach (int hash in TileHashList)
            {
                setBiome(hash);
            }
        }

        public bool isFullyInfected(int tile)
        {
            if (tiberiumPcts.ContainsKey(tile))
            {
                return tiberiumPcts[tile] >= 1f;
            }
            else
            {
                return false;
            }
        }

        public bool isTiberiumTile(int tileInt)
        {
            Tile tile = Find.WorldGrid.tiles[tileInt];
            return tile.biome.defName.Contains("Tiberium");
        }

        public void setBiome(int tileID)
        {
            Tile tile = Find.WorldGrid.tiles[tileID];

            if (Find.WorldGrid.tiles[tileID].biome.canBuildBase)
            {
                tile.biome = DefDatabase<BiomeDef>.GetNamed("TiberiumRedZone");
            }
            else
            {
                tile.biome = DefDatabase<BiomeDef>.GetNamed("TiberiumGlacierSea");
            }
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (Find.TickManager.TicksGame % 500 == 0)
            {
                Spread();
            }
            if (Find.TickManager.TicksGame % GenDate.TicksPerDay == 0)
            {
                Grow();
            }
        }

        public void Spread()
        {
            for (int i = 0; i < TileID.Count; i++)
            {
                int tileID = TileID[i];
                if (!TileHashList.Contains(tileID))
                {
                    if (!isFullyInfected(tileID))
                    {
                        if (isTiberiumTile(tileID))
                        {
                            Find.WorldGrid.GetTileNeighbors(tileID, tmpNeighbors);
                            for (int ii = 0; ii < tmpNeighbors.Count; ii++)
                            {
                                if (!TileHashList.Contains(tmpNeighbors[ii]))
                                {
                                    if (!TileID.Contains(tmpNeighbors[ii]))
                                    {
                                        if (Find.WorldGrid.tiles[tmpNeighbors[ii]].biome.defName.Contains("Ice"))
                                        {
                                            tmpNeighbors.Remove(tmpNeighbors[ii]);
                                        }
                                        else
                                        {
                                            TileID.Add(tmpNeighbors[ii]);
                                            tmpNeighbors.Remove(tmpNeighbors[ii]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (tiberiumPcts.ContainsKey(tileID))
                            {
                                if (tiberiumPcts[tileID] > 0.55 && tiberiumPcts[TileID[i]] < 1.0f)
                                {
                                    setBiome(tileID);
                                }
                            }
                        }
                    }
                    else
                    {
                        TileHashList.Add(tileID);
                    }
                }
                else
                {
                    TileID.Remove(tileID);
                }
            }
        }

        public void Grow()
        {
            for (int i = 0; i < TileID.Count; i++)
            {
                int tileID = TileID[i];
                if (!TileHashList.Contains(tileID))
                {
                    Tile tile = Find.WorldGrid.tiles[tileID];
                    if (tile != Current.Game.AnyPlayerHomeMap.TileInfo)
                    {
                        if (!tiberiumPcts.ContainsKey(tileID))
                        {
                            tiberiumPcts.Add(tileID, Rand.Range(0f, 0.025f));
                        }
                        else
                        {
                            if (tile.hilliness == Hilliness.Mountainous)
                            {
                                tiberiumPcts[tileID] += Rand.Range(0f, 0.015f);

                            }
                            else
                            if (tile.temperature > 10)
                            {
                                tiberiumPcts[tileID] += Rand.Range(0f, 0.007f);

                            }
                            else
                            {
                                tiberiumPcts[tileID] += Rand.Range(0f, 0.025f);
                            }

                            if (tiberiumPcts[tileID] >= 1f)
                            {
                                tiberiumPcts[tileID] = 1f;
                            }
                        }
                    }
                }
            }
        }
    }
}