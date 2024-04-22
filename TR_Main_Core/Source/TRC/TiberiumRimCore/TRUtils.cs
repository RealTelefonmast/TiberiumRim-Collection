using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using TeleCore;
using UnityEngine;
using Verse;

namespace TR;

public static class TRUtils
{
    public static WorldComponent_TR Tiberium()
    {
        return Find.World.GetComponent<WorldComponent_TR>();
    }
    
    public static GameComponent_TR TiberiumRim()
    {
        return Current.Game.GetComponent<GameComponent_TR>();
    }
    
    public static ResearchDiscoveryTable ResearchDiscoveryTable()
    {
        return TiberiumRim().ResearchDiscoveryTable;
    }

    public static EventManager EventManager()
    {
        return Find.World.GetComponent<EventManager>();
    }

    public static ResearchCreationTable ResearchCreationTable()
    {
        return Find.World.GetComponent<TResearchManager>().creationTable;
    }

    public static ResearchTargetTable ResearchTargetTable()
    {
        return Find.World.GetComponent<TResearchManager>().researchTargets;
    }

    public static TResearchManager ResearchManager()
    {
        return Find.World.GetComponent<TResearchManager>();
    }

    public static MainTabWindow WindowFor(MainButtonDef def)
    {
        return def.TabWindow;
    }

    public static EventLetter SendEventLetter(this LetterStack stack, TaggedString eventLabel, TaggedString eventDesc,
        EventDef eventDef, LookTargets targets = null)
    {
        EventLetter letter =
            (EventLetter)LetterMaker.MakeLetter(eventLabel, eventDesc, TRCDefOf.EventLetter, targets);
        letter.AddEvent(eventDef);
        stack.ReceiveLetter(letter);
        return letter;
    }

    public static string GetTextureDirectory()
    {
        return GetModRootDirectory() + Path.DirectorySeparatorChar + "Textures" + Path.DirectorySeparatorChar;
    }

    public static string GetModRootDirectory()
    {
        TiberiumCoreMod mod = LoadedModManager.GetMod<TiberiumCoreMod>();
        if (mod == null)
        {
            TRLog.Error("LoadedModManager.GetMod<TiberiumRimMod>() failed");
            return "";
        }

        return mod.Content.RootDir;
    }

    public static string SizeTo(this string label, float newSize)
    {
        while (Text.CalcSize(label).x < newSize)
        {
            label += " ";
        }

        return label;
    }
    
    public static bool IsConductive(this Thing thing)
    {
        if (thing.IsMetallic()) return true;
        if (thing.def.IsConductive()) return true;
        return false;
    }

    public static bool IsConductive(this ThingDef def)
    {
        return def.race != null;
    }

    public static Material GetColoredVersion(this Material mat, Color color)
    {
        Material material = new Material(mat);
        material.color = color;
        return material;
    }

    public static void DrawTargeter(IntVec3 pos, Material mat, float size)
    {
        Vector3 vector = pos.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
        Matrix4x4 matrix = default;
        matrix.SetTRS(vector, Quaternion.Euler(0f, 0f, 0f), new Vector3(size, 1f, size));
        Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0, null, 0);
    }

    public static bool IsPlayerControlledMech(this Thing thing)
    {
        return thing is MechanicalPawn p && (p.Faction?.IsPlayer ?? false);
    }

    public static float GetStatOffsetFromList(this List<ConditionalStatModifier> list, StatDef stat, Pawn pawn)
    {
        if (list == null) return 0;
        return list.Select(t => t.StatOffsetForStat(stat, pawn)).Sum();
    }

    public static Pawn NewBorn(PawnKindDef kind, Faction faction = null,
        PawnGenerationContext context = PawnGenerationContext.NonPlayer)
    {
        PawnGenerationRequest request = new PawnGenerationRequest(kind, faction, context, -1, true, true);
        return PawnGenerator.GeneratePawn(request);
    }

    public static List<IntVec3> RemoveCorners(this CellRect rect, int[] range)
    {
        List<IntVec3> cells = rect.Cells.ToList();
        for (var i = 0; i < range.Count(); i++)
        {
            var j = range[i];
            switch (j)
            {
                case 1:
                    cells.RemoveAll(c => c.x == rect.minX && c.z == rect.maxZ);
                    break;
                case 2:
                    cells.RemoveAll(c => c.x == rect.maxX && c.z == rect.maxZ);
                    break;
                case 3:
                    cells.RemoveAll(c => c.x == rect.maxX && c.z == rect.minZ);
                    break;
                default:
                    cells.RemoveAll(c => c.x == rect.minX && c.z == rect.minZ);
                    break;
            }
        }

        return cells;
    }

    public static Matrix4x4 MatrixFor(Vector3 pos, float rotation, Vector3 size)
    {
        Matrix4x4 matrix = default;
        matrix.SetTRS(pos, rotation.ToQuat(), size);
        return matrix;
    }

    public static ThingDef MakeNewBluePrint(ThingDef def, bool isInstallBlueprint, ThingDef normalBlueprint = null)
    {
        Type type = typeof(ThingDefGenerator_Buildings);
        var NewBlueprint = type.GetMethod("NewBlueprintDef_Thing", BindingFlags.NonPublic | BindingFlags.Static);
        return (ThingDef)NewBlueprint.Invoke(null, new object[] { def, isInstallBlueprint, normalBlueprint });
    }

    public static ThingDef MakeNewFrame(ThingDef def)
    {
        Type type = typeof(ThingDefGenerator_Buildings);
        var NewFrame = type.GetMethod("NewFrameDef_Thing", BindingFlags.NonPublic | BindingFlags.Static);
        return (ThingDef)NewFrame.Invoke(null, new object[] { def });
    }

    public static Rot4 FromAngleFlat2(float angle)
    {
        angle = GenMath.PositiveMod(angle, 360f);
        if (angle <= 45f)
            return Rot4.North;
        if (angle <= 135f)
            return Rot4.East;
        if (angle < 225f)
            return Rot4.South;
        if (angle <= 315f)
            return Rot4.West;
        return Rot4.North;
    }

    public static IntVec3 PositionOffset(this IntVec3 fromCenter, IntVec3 toCenter)
    {
        Rot4 rotation = FromAngleFlat2((fromCenter - toCenter).AngleFlat);
        if (rotation == Rot4.North)
            return IntVec3.North;
        if (rotation == Rot4.East)
            return IntVec3.East;
        if (rotation == Rot4.South)
            return IntVec3.South;
        if (rotation == Rot4.West)
            return IntVec3.West;
        return IntVec3.Zero;
    }

    //
    public static bool IsConstructible(this ThingDef def)
    {
        if (!def.IsBuildingArtificial) return false;
        if (def is TRThingDef trThing && trThing.isNatural) return false;
        return true;
    }

    public static bool ThingExistsAt(Map map, IntVec3 pos, ThingDef def)
    {
        return !map.thingGrid.ThingAt(pos, def).DestroyedOrNull();
    }

    public static Thing GetAnyThingIn<T>(this CellRect cells, Map map)
    {
        foreach (var c in cells)
        {
            if (!c.InBounds(map)) continue;
            var t = c.GetThingList(map).Find(x => x is T);
            if (t != null)
            {
                return t;
            }
        }

        return null;
    }
    

    public static bool ThingFitsAt(this ThingDef thing, Map map, IntVec3 cell)
    {
        foreach (var c in GenAdj.OccupiedRect(cell, Rot4.North, thing.size))
        {
            if (!c.InBounds(map) || c.Fogged(map) || !c.Standable(map) || (c.Roofed(map) && c.GetRoof(map).isThickRoof))
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsBlocked(this IntVec3 cell, Map map, out bool byPlant)
    {
        byPlant = false;
        if (!cell.Walkable(map))
        {
            return true;
        }

        List<Thing> list = map.thingGrid.ThingsListAt(cell);
        foreach (var thing in list)
        {
            if (thing.def.passability != Traversability.Standable)
            {
                byPlant = thing is Plant;
                return true;
            }
        }

        return false;
    }

    public static CellRect ToCellRect(this List<IntVec3> cells)
    {
        int minZ = cells.Min(c => c.z);
        int maxZ = cells.Max(c => c.z);
        int minX = cells.Min(c => c.x);
        int maxX = cells.Max(c => c.x);
        int width = maxX - (minX - 1);
        int height = maxZ - (minZ - 1);
        return new CellRect(minX, minZ, width, height);
    }

    public static IEnumerable<IntVec3> CellsAdjacent8Way(this IntVec3 loc, bool andInside = false)
    {
        if (andInside)
        {
            yield return loc;
        }

        IntVec3 center = loc;
        int minX = center.x - (1 - 1) / 2 - 1;
        int minZ = center.z - (1 - 1) / 2 - 1;
        int maxX = minX + 1 + 1;
        int maxZ = minZ + 1 + 1;
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minZ; j <= maxZ; j++)
            {
                yield return new IntVec3(i, 0, j);
            }
        }

        yield break;
    }

    public static string ToStringDictListing<K, V>(this Dictionary<K, V> dict)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var v in dict)
        {
            sb.AppendLine($"{v.Key}: {v.Value}");
        }

        return sb.ToString().TrimEndNewlines();
    }
    
    public static bool HasResearchExtension(this Def def, out ResearchModExtension research)
    {
        research = def.GetModExtension<ResearchModExtension>();
        return research != null;
    }
}