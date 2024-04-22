﻿using System.Collections.Generic;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class Building_VeinTentacle : Building
    {
        private int progressTicks;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        private bool ShouldBeDying
        {
            get
            {
                return progressTicks > 60;
            }
        }

        private bool canAttack
        {
            get
            {
                return (progressTicks % 20 == 0);
            }
        }

        public override void Tick()
        {
            progressTicks += 1;
            if (ShouldBeDying)
            {
                this.DeSpawn();
                return;
            }

            IntVec3 c = this.Position;
            if (c.InBounds(Map))
            {
                List<Thing> thinglist = c.GetThingList(Map);
                for (int i = 0; i < thinglist.Count; i++)
                {
                    Pawn p = thinglist[i] as Pawn;
                    if (p != null)
                    {
                        hurt(p);
                    }
                }
            }
            base.Tick();
        }

        public void hurt(Pawn p)
        {
            if (canAttack)
            {
                int amt = 2;
                if (p.apparel == null)
                {
                    amt = amt * 3;
                }
                DamageInfo damage = new DamageInfo(DamageDefOf.Blunt, amt);

                if (Rand.Chance(0.1f) && !p.def.defName.Contains("TBI") && p.Position.InBounds(this.Map))
                {
                    if (!p.Downed)
                    {
                        p.TakeDamage(damage);
                    }
                }
            }
        }
    }
}
