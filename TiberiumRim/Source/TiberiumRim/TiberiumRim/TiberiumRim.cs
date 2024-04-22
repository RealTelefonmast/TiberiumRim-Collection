using RimWorld;
using System.Reflection;
using Verse;
using System.IO;
using UnityEngine;
using RimWorld.Planet;
using System;
using System.Text;
using System.Collections.Generic;
using HarmonyLib;

namespace TiberiumRim
{
    public static class TiberiumRimSettings
    {
        public static TiberiumSettings settings;
    }

    public class TiberiumRimMod : Mod
    {
        TiberiumSettings settings;

        public TiberiumRimMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<TiberiumSettings>();
            TiberiumRimSettings.settings = this.settings;
        }

        public override string SettingsCategory() => "TiberiumRim";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float width = 230f;
            Vector2 vec = new Vector2(0f, -32f);

            Rect rowRect1 = new Rect(inRect);
            rowRect1.height = 32f;
            string BuildingDamage = "BuildingDamage".Translate();
            rowRect1.width = width;
            Widgets.CheckboxLabeled(rowRect1, BuildingDamage, ref this.settings.BuildingDamage, false);
            if (this.settings.BuildingDamage)
            {
                Rect textRect = new Rect(rowRect1);
                textRect.xMin = rowRect1.xMax;
                textRect.xMax = textRect.xMin + 800f;
                Widgets.Label(textRect, "BuildingDamageDesc".Translate());
            }

            Rect rowRect2 = new Rect(rowRect1);
            rowRect2.center = rowRect1.center - vec;
            string EntityDamage = "EntityDamage".Translate();
            Widgets.CheckboxLabeled(rowRect2, EntityDamage, ref this.settings.EntityDamage, false);
            if (this.settings.EntityDamage)
            {
                Rect textRect = new Rect(rowRect2);
                textRect.xMin = rowRect2.xMax;
                textRect.xMax = textRect.xMin + 800f;
                Widgets.Label(textRect, "EntityDamageDesc".Translate());
            }

            Rect rowRect3 = new Rect(rowRect2);
            rowRect3.center = rowRect2.center - vec;
            string UseProducerCapLabel = "UseProducerCapLabel".Translate();
            Widgets.CheckboxLabeled(rowRect3, UseProducerCapLabel, ref this.settings.UseProducerCap, false);
            if (this.settings.UseProducerCap)
            {
                Rect textRect = new Rect(rowRect3);
                textRect.xMin = rowRect3.xMax;
                textRect.xMax = textRect.xMin + 800f;
                Widgets.Label(textRect, "UseProducerCapDesc".Translate());
            }

            Rect rowRect4 = new Rect(rowRect3);
            rowRect4.center = rowRect3.center - vec;
            string UseSpreadRadiusLabel = "UseSpreadRadiusLabel".Translate();
            Widgets.CheckboxLabeled(rowRect4, UseSpreadRadiusLabel, ref this.settings.UseSpreadRadius, false);
            Widgets.CheckboxLabeled(rowRect3, UseProducerCapLabel, ref this.settings.UseProducerCap, false);
            if (this.settings.UseSpreadRadius)
            {
                Rect textRect = new Rect(rowRect4);
                textRect.xMin = rowRect4.xMax;
                textRect.xMax = textRect.xMin + 800f;
                Widgets.Label(textRect, "UseSpreadRadiusDesc".Translate());
            }

            if (this.settings.UseProducerCap)
            {
                Rect rowRect5 = new Rect(rowRect4);
                rowRect5.center = rowRect4.center - vec;
                string buffer = this.settings.TiberiumProducersAmt.ToString();
                Widgets.TextFieldNumeric(rowRect5, ref this.settings.TiberiumProducersAmt, ref buffer, 1f, 1000f);           
            }
        }
    }  

    public class TiberiumSettings : ModSettings
    {
        public bool BuildingDamage = true;
        public bool EntityDamage = true;
        public bool UseProducerCap = false;
        public bool UseSpreadRadius = false;
        public int TiberiumProducersAmt = 7;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.BuildingDamage, "BuildingDamage", true, true);
            Scribe_Values.Look(ref this.EntityDamage, "EntityDamage", true, true);
            Scribe_Values.Look(ref this.UseProducerCap, "UseProducerCap", false, true);
            Scribe_Values.Look(ref this.UseSpreadRadius, "UseSpreadRadius", false, true);
            Scribe_Values.Look(ref this.TiberiumProducersAmt, "TiberiumProducersAmt", TiberiumProducersAmt);
        }
    }

    [StaticConstructorOnStartup]
    class TiberiumRim
    {
        static TiberiumRim()
        {
            HarmonyInstance TiberiumRim = HarmonyInstance.Create("com.tiberiumrim.rimworld.mod");
            TiberiumRim.PatchAll(Assembly.GetExecutingAssembly());
        }
   
        
    [HarmonyPatch(typeof(UI_BackgroundMain)), HarmonyPatch("BackgroundOnGUI"), StaticConstructorOnStartup]
    internal static class Custom_UI_BackgroundMain
    {
            private static readonly Texture2D Custom_Background = ContentFinder<Texture2D>.Get("Icons/TiberiumBackground", true);

            internal static readonly Vector2 MainBackgroundSize = new Vector2(2048f, 1280f);

        private static bool Prefix()
        {           
            if (Custom_UI_BackgroundMain.Custom_Background)
            {
                float width = (float)UI.screenWidth;
                float num = (float)UI.screenWidth * (Custom_UI_BackgroundMain.MainBackgroundSize.y / Custom_UI_BackgroundMain.MainBackgroundSize.x);
                GUI.DrawTexture(new Rect(0f, (float)UI.screenHeight / 2f - num / 2f, width, num), Custom_UI_BackgroundMain.Custom_Background, ScaleMode.ScaleToFit, true);
            }
            return false;
        }
    }

        [HarmonyPatch(typeof(GameCondition_ToxicFallout), "DoCellSteadyEffects")]
        public static class ToxicFalloutPatch
        {
            [HarmonyPrefix]
            static bool PrefixMethod(GameCondition_ToxicFallout __instance, IntVec3 c)
            {
                var map = Traverse.Create(__instance).Property("Map").GetValue<Map>();
                if (c.InBounds(map) && !c.Roofed(map))
                {
                    List<Thing> thingList = c.GetThingList(map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        if (thingList[i] != null)
                        {
                            if (thingList[i] is Plant)
                            {
                                if (thingList[i].def.defName.Contains("Tiberium"))
                                {
                                    return false;
                                }
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(WorldInspectPane))]
        [HarmonyPatch("TileInspectString", PropertyMethod.Getter)]
        class WorldTilePatch
        {
            [HarmonyPostfix]
            static void postFix(WorldInspectPane __instance, ref String __result)
            {
                int SelectedTile = Traverse.Create(__instance).Property("SelectedTile").GetValue<int>();
                Tile tile = Find.WorldGrid[SelectedTile];

                StringBuilder stringBuilder = new StringBuilder();
                if (Find.World.GetComponent<WorldComponent_TiberiumSpread>().tiberiumPcts.ContainsKey(SelectedTile))
                {
                    stringBuilder.Append("TiberiumTileInfestation".Translate() + Find.World.GetComponent<WorldComponent_TiberiumSpread>().tiberiumPcts[SelectedTile] * 100 + "%");
                }
                __result = __result + "\n\n" + stringBuilder.ToString();

            }
        }             
    }
}


