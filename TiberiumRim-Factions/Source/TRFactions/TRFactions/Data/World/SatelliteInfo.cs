

namespace TR;

public class SatelliteInfo : WorldInformation
{
    public ASATNetwork AttackSatelliteNetwork;

    public SatelliteInfo(RimWorld.Planet.World world) : base(world)
    {
        AttackSatelliteNetwork = new ASATNetwork();
    }
}