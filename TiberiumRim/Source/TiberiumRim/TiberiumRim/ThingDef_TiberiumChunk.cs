using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class ThingDef_TiberiumChunk : ThingWithComps
    {
        public override void TickLong()
        {
            damageChunks(15);
            if (Rand.Chance(0.01f))
            {
                int i = Rand.Range(0, 50);
                DamageInfo d = new DamageInfo(DamageDefOf.Deterioration, i);
                this.TakeDamage(d);
                return;
            }
        }

        public void damageChunks(int amt)
        {
            var c = this.RandomAdjacentCell8Way();
            if (c.InBounds(this.Map))
            {
                var p = c.GetFirstHaulable(this.Map);

                DamageInfo damage = new DamageInfo(DamageDefOf.Deterioration, amt);

                if (p != null)
                {
                    if (!p.def.defName.Contains("Tiberium"))
                    {
                        if (p.def.defName.Contains("Chunk") && Rand.Chance(0.05f))
                        {
                            ThingDef rock = null;

                            switch (this.def.defName)
                            {
                                case "GreenTiberiumChunk":
                                    rock = ThingDef.Named("GreenTiberiumChunk");
                                    break;

                                case "BlueTiberiumChunk":
                                    rock = ThingDef.Named("BlueTiberiumChunk");
                                    break;

                                case "RedTiberiumChunk":
                                    rock = ThingDef.Named("RedTiberiumChunk");
                                    break;
                            }

                            IntVec3 loc = p.Position;

                            if (loc.InBounds(Map))
                            {
                                p.Destroy(DestroyMode.Vanish);
                                GenSpawn.Spawn(rock, loc, Map);
                                return;
                            }
                        }
                        p.TakeDamage(damage);
                    }
                }
            }
        }
    }
}
