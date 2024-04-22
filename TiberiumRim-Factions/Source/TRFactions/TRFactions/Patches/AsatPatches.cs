﻿using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace TR.Patches;

public static class AsatPatches
{
    [HarmonyPatch(typeof(WorldSelector))]
    [HarmonyPatch("HandleWorldClicks")]
    public static class HandleWorldClicksPatch
    {
        public static bool Prefix(WorldSelector __instance)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 1 && __instance.NumSelectedObjects > 0)
                {
                    WorldObject obj = __instance.FirstSelectedObject;
                    if (obj is AttackSatellite asat)
                    {
                        asat.SetDestination(GenWorld.MouseTile(false));
                        Event.current.Use();
                        return false;
                    }
                }
            }
            return true;
        }
    }
}