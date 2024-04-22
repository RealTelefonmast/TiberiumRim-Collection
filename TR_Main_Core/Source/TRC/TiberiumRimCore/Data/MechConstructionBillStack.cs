using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TeleCore.Primitive;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TR;

public class MechConstructionBillStack
{
    private Building_Hangar _parent;
    private List<MechConstructionBill> _queue;
    
    public bool HasBillWaiting => CurrentItem is { IsPreparing: true };
    public MechConstructionBill CurBill => CurrentItem;
    public IReadOnlyCollection<MechConstructionBill> All => _queue;
    internal List<MechConstructionBill> Queue => _queue;
    
    public MechConstructionBill CurrentItem => _queue.Count > 0 ? _queue.First() : null;
    
    
    public MechConstructionBillStack(Building_Hangar parent)
    {
        _parent = parent;
        _queue = new List<MechConstructionBill>();
    }
    
    public void Tick()
    {
        if (_queue.Count <= 0) return;

        if (CurrentItem is { IsPreparing: false , IsFinished: false})
        {
            CurrentItem.TickProgress();
            return;
        }
        
        if(CurrentItem is {IsFinished: true})
            Finish();
    }

    public void Begin(MechConstructionBill bill)
    {

    }

    public void Finish()
    {
        var item = CurrentItem;
        _queue.Remove(item);
        //Spawn mech
        PawnKindDef kind = item.Recipe.mechDef;
        Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, _parent.Faction, PawnGenerationContext.NonPlayer, -1, false, false, true, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn, null, null, null, false));
        GenSpawn.Spawn(pawn, _parent.Position, _parent.Map);
        var freeSpot = CellFinder.RandomClosewalkCellNear(_parent.Position, _parent.Map, _parent.RotatedSize.MagnitudeManhattan);
        pawn.jobs.StartJob(new Job(JobDefOf.Goto, freeSpot), JobCondition.InterruptForced);
    }

    public void Notify_Reordered(MechRecipeDef def, int newIndex)
    {
        var bill = _queue.Find(c => c.Recipe == def);
        _queue.Remove(bill);
        _queue.Insert(newIndex, bill);
    }
    
    public void Add(MechRecipeDef recipe)
    {
        var bill = new MechConstructionBill(recipe);
        _queue.Add(bill);
    }

    public void Delete(MechConstructionBill bill)
    {
        _queue.Remove(bill);
        //TODO: Refund
    }
    
    public void Notify_AddedResources(Thing thing, int count)
    {
        if (thing.stackCount != count)
        {
            TRLog.Warning($"Something is wrong: {thing} has a stack count of {thing.stackCount} but we are trying to add {count} to the bill.");
        }
        CurrentItem.Notify_ResourceAdded(thing, thing.stackCount);
    }
}

public class MechConstructionBill
{
    private readonly MechRecipeDef _def;
    private bool isSelected;
    private DefValueStack<ThingDef, int> _inputCache;
    private int _progress;
    
    public bool IsPreparing => MissingResources.Any();
    public bool IsFinished => _progress >= _def.workCost;
    
    public float ProgressPercent => _progress / (float)_def.workCost;
    public float ItemProgress => _inputCache.TotalValue / (float)_def.costList.Sum(c => c.count);
    
    public string ProgressLabel => $"{_progress}/{_def.workCost}";
    public string ItemProgressLabel => $"{_inputCache.TotalValue}/{_def.costList.Sum(c => c.count)}";

    public MechRecipeDef Recipe => _def;
    
    public IEnumerable<ThingDefCount> MissingResources
    {
        get
        {
            foreach (var needed in _def.costList)
            {
                var left = needed.count - _inputCache[needed.thingDef].Value;
                if (left > 0)
                    yield return new ThingDefCount(needed.thingDef, left);
            }
        }
    }

    public MechConstructionBill(MechRecipeDef recipeDef)
    {
        _def = recipeDef;
    }

    public void Notify_Selected(bool selected)
    {
        isSelected = selected;
    }
    
    public void Notify_ResourceAdded(Thing thing, int count)
    {
        _inputCache += (thing.def, count);
    }

    public void TickProgress()
    {
        _progress = Mathf.Clamp(_progress + 10, 0, _def.workCost);
    }
}