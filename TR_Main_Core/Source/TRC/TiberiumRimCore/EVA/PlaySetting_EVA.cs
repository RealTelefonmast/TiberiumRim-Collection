using TeleCore;
using UnityEngine;

namespace TR;

public class PlaySetting_EVA : PlaySettingsWorker
{
    //TODO: Reintroduce EVA
    public override bool Visible => false;
    public override Texture2D Icon => TRContent.Icon_EVA;
    public override string Description => "TR.PlaySettings.EVA";
    
    public override void OnToggled(bool isActive)
    {
        GameComponent_EVA.EVAComp().EVAActive = isActive;
    }
}