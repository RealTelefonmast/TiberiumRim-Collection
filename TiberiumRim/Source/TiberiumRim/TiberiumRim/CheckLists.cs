using System.Collections.Generic;
using Verse;

namespace TiberiumRim
{
    public static class CheckLists
    {

        public static HashSet<IntVec3> ProtectedCells = new HashSet<IntVec3>();

        public static HashSet<IntVec3> SuppressedCells = new HashSet<IntVec3>();

        public static HashSet<IntVec3> AllowedCells = new HashSet<IntVec3>();

        public static int producerAmt = new int();

    }
}
