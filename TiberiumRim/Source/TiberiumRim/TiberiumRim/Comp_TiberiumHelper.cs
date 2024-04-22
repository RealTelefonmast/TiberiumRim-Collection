using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace TiberiumRim
{
    public class Comp_TiberiumHelper : ThingComp
    {
        Map map;
        public bool drawAffected = false;
        public bool drawProtected = false;
        public bool drawSuppressed = false;
        public bool drawAllowed = false;

        public List<IntVec3> intList1 = new List<IntVec3>();
        public List<IntVec3> intList2 = new List<IntVec3>();
        public List<IntVec3> intList3 = new List<IntVec3>();
        public List<IntVec3> intList4 = new List<IntVec3>();

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            map = this.parent.Map;
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            if (this.parent.TrueCenter().InBounds(map))
            {
                if (this.drawAffected)
                {
                    if (intList1.Count > 0)
                    {
                        foreach (IntVec3 c in intList1)
                        {
                            if (!map.GetComponent<MapComponent_Tiberium>().affectedTiles.Contains(c))
                            {
                                intList1.Remove(c);
                            }
                        }
                    }
                    foreach (IntVec3 c in map.GetComponent<MapComponent_Tiberium>().affectedTiles)
                    {
                        if (!intList1.Contains(c))
                        {
                            intList1.Add(c);
                        }

                    }
                    GenDraw.DrawFieldEdges(intList1, Color.white);                 
                }

                if (this.drawProtected)
                {
                    foreach (IntVec3 c in CheckLists.ProtectedCells)
                    {
                        intList2.Add(c);
                    }
                    GenDraw.DrawFieldEdges(intList2, Color.cyan);
                }

                if (this.drawAllowed)
                {
                    foreach (IntVec3 c in CheckLists.AllowedCells)
                    {
                        intList3.Add(c);
                    }
                    GenDraw.DrawFieldEdges(intList3, Color.green);
                }

                if (this.drawSuppressed)
                {
                    foreach (IntVec3 c in CheckLists.SuppressedCells)
                    {
                        intList4.Add(c);
                    }
                    GenDraw.DrawFieldEdges(intList4, Color.red);
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
            {
                yield return g;
            }

            if (true)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Show affected tiles",
                    activateSound = SoundDefOf.Click,
                    action = delegate
                    {
                        this.drawAffected = !drawAffected;
                        Log.Message("Show affected: " + drawAffected);
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Show protected tiles",
                    action = delegate
                    {
                        this.drawProtected = !drawProtected;
                        Log.Message("Show protected: " + drawProtected);
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Show allowed tiles",
                    action = delegate
                    {
                        this.drawAllowed = !drawAllowed;
                        Log.Message("Show allowed: " + drawAllowed);
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Show suppresed tiles",
                    action = delegate
                    {
                        this.drawSuppressed = !drawSuppressed;
                        Log.Message("Show suppressed: " + drawSuppressed);
                    }
                };
            }
        }
    }

    public class CompProperties_TiberiumHelper : CompProperties
    {
        public CompProperties_TiberiumHelper()
        {
            this.compClass = typeof(Comp_TiberiumHelper);
        }
    }
}