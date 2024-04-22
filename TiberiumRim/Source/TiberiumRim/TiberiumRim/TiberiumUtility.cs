using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TiberiumRim
{
    public class TiberiumUtility
    {
        public static void AffectPawn(Pawn pawn, TiberiumCrystalDef def, Plant parent, Map map)
        {
            if (!def.isFlesh)
            {
                Infect(pawn, 0.25f, map, false);
            }
            else
            {
                Hurt(pawn, parent, map);
            }
        }

        public static void Infect(Pawn pawn, float amt, Map map, bool isGas)
        {
            HediffDef tiberium = TiberiumHediffDefOf.TiberiumBuildupHediff;
            HediffDef addiction = TiberiumHediffDefOf.TiberiumAddiction;
            HediffDef Stage1 = TiberiumHediffDefOf.TiberiumStage1;
            HediffDef Stage2 = TiberiumHediffDefOf.TiberiumStage2;
            HediffDef Stage3 = TiberiumHediffDefOf.TiberiumContactPoison;
            HediffDef Immunity = TiberiumHediffDefOf.TiberiumInfusionImmunity;            

            if (Immunity != null)
            {
                if (pawn.health.hediffSet.HasHediff(Immunity))
                {
                    return;
                }
            }

            HediffSet hediffs = pawn.health.hediffSet;

            if (hediffs.HasHediff(Stage1) | hediffs.HasHediff(Stage2) | hediffs.HasHediff(addiction))
            {
                return;
            }

            if (pawn.RaceProps.IsMechanoid)
            {
                return;
            }

            if (pawn.def.defName.Contains("_TBI"))
            {
                return;
            }

            if (pawn.Position.InBounds(map))
            {
                if (pawn.AnimalOrWildMan())
                {
                    HealthUtility.AdjustSeverity(pawn, tiberium, +amt * 2);
                    return;
                }
                else
                if (pawn.apparel == null)
                {
                    HealthUtility.AdjustSeverity(pawn, tiberium, +amt * 3);
                    return;
                }

                List<Apparel> Clothing = pawn.apparel.WornApparel;

                float protection = 0;

                int parts = 0;

                Apparel apparel = null;

                foreach (Apparel a in Clothing)
                {
                    if (a.def.defName.Contains("_TBP"))
                    {
                        if (a.HitPoints < a.MaxHitPoints * 0.15)
                        {
                            HealthUtility.AdjustSeverity(pawn, tiberium, +amt/2);
                            if (Current.Game.tickManager.TicksGame % 1250 == 0)
                            {
                                Messages.Message("MessageTiberiumSuitLeak".Translate(), new TargetInfo(pawn.Position, map, false), MessageTypeDefOf.CautionInput);
                            }
                            return;
                        }
                        parts = parts + 1;
                        apparel = a;
                    }
                    else if (protection < a.GetStatValue(StatDefOf.ArmorRating_Sharp))
                    {
                        protection = protection + a.GetStatValue(StatDefOf.ArmorRating_Sharp);
                    }
                }

                if (protection >= 0.6)
                {
                    return;
                }

                if (parts < 2)
                {
                    if(isGas)
                    {
                        if (apparel != null)
                        {
                            if (apparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead))
                            {
                                HealthUtility.AdjustSeverity(pawn, tiberium, +amt);
                                return;
                            }
                        }
                    }
                    HealthUtility.AdjustSeverity(pawn, tiberium, +amt);
                    return;
                }
            }
        }

        public static void Hurt(Pawn pawn, Plant parent, Map map)
        {
            if (Rand.Chance(0.75f))
            {
                int amt = 2;
                if (pawn.apparel == null)
                {
                    amt = amt * 3;
                }
                DamageInfo damage = new DamageInfo(DamageDefOf.Blunt, amt);

                if (!pawn.def.defName.Contains("TBI") && pawn.Position.InBounds(map))
                {
                    if (!pawn.Downed)
                    {
                        Log.Message("Hurt: 2 - Should Take Damage");
                        pawn.TakeDamage(damage);
                    }
                }
                if (parent.Position.InBounds(map))
                {
                    Building b = parent.Position.RandomAdjacentCell8Way().GetFirstBuilding(map);
                    if (b != null && b.def.defName.Contains("Veinhole"))
                    {
                        Infect(pawn, 0.25f, map, true);
                    }
                }
            }
        }
    }
}
