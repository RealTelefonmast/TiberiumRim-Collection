using TeleCore;
using UnityEngine;
using Verse;

namespace TR;

public abstract class Designator_Target : Designator_Extended
{
    protected Material targeterMat;
    private Material tempMaterial;
    protected float size;
    protected FloatRange opacity = new FloatRange(0.5f, 1f);
    public ISuperWeapon superWeapon; 

    public override AcceptanceReport CanDesignateCell(IntVec3 loc)
    {
        return loc.InBounds(Map) && !loc.Fogged(Map);
    }

    public override void SelectedUpdate()
    {
        if (targeterMat == null)
            return;

        tempMaterial = new Material(targeterMat);
        bool designateCheck = CanDesignateCell(UI.MouseCell()).Accepted;
        Color color = !designateCheck ? Color.red : targeterMat.color;
        if (opacity.min != opacity.max)
            color.a = TMath.Cosine2(opacity.min, opacity.max, 3f, 0, Time.realtimeSinceStartup * 6.28318548f);
        tempMaterial.color = color;
        TRUtils.DrawTargeter(UI.MouseCell(), tempMaterial, size);
    }
}