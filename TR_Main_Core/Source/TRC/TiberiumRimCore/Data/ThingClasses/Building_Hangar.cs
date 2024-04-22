using RimWorld;
using Verse;

namespace TR;

public class Building_Hangar : TRBuildingPrototype
{
    private MechConstructionBillStack _billStack;
    
    public MechConstructionBillStack Bills => _billStack;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        _billStack = new MechConstructionBillStack(this);
    }

    public override void Tick()
    {
        if(PowerComp is CompPowerTrader { PowerOn: false })
            return;
        _billStack.Tick();
        base.Tick();
    }
    
    public void AddMechConstructionBill(MechRecipeDef recipe)
    {
        _billStack.Add(recipe);
    }

    internal void MechConstructionFinished(MechConstructionBill bill)
    {
        
    }
}