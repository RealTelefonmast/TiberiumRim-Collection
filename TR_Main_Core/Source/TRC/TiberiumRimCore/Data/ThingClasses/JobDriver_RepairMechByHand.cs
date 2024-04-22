using System.Collections.Generic;
using Verse.AI;

namespace TR.ThingClasses;

public class JobDriver_RepairMechByHand : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        throw new System.NotImplementedException();
    }
}