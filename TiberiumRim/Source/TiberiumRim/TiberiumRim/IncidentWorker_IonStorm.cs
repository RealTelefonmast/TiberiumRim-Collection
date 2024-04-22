using UnityEngine;
using Verse;
using RimWorld;

namespace TiberiumRim
{
    public class IncidentWorker_IonStorm : IncidentWorker_MakeGameCondition
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int count = map.listerThings.AllThings.FindAll((Thing x) => x.def.defName.Contains("Tiberium")).Count;

            if (count > 350)
            {
                int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
                GameCondition cond = GameConditionMaker.MakeCondition(this.def.gameCondition, duration, 1);
                GameCondition cond2 = GameConditionMaker.MakeCondition(GameConditionDefOf.SolarFlare, duration, 1);
                map.gameConditionManager.RegisterCondition(cond);
                map.gameConditionManager.RegisterCondition(cond2);
                map.weatherManager.TransitionTo(WeatherDef.Named("IonStormWeather"));
                base.SendStandardLetter();
                return true;
            }
            return false;
        }
    }
}
