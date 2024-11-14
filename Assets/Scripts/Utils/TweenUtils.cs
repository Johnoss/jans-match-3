using Scripts.Features.Grid.Moving;
using UnityEngine;

namespace Scripts.Utils
{
    public static class TweenUtils
    {
        public static float GetTweenDuration(this TweenSetting tweenSetting, float? distance = null)
        {
            var baseDuration = tweenSetting.TweenDelaySeconds;
            
            if (!tweenSetting.IsSpeedBased)
            {
                return (baseDuration + tweenSetting.TweenDurationSeconds) * tweenSetting.LoopCount;
            }
            
            if (distance.HasValue)
            {
                return baseDuration + distance.Value / tweenSetting.TweenDurationSeconds;
            }
                
            Debug.LogError("Distance is not set for speed based tween");
            return baseDuration;
        }
    }
}