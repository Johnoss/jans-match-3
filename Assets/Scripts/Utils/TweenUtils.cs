using DG.Tweening;
using Scripts.Features.Tweening;
using UnityEngine;

namespace Scripts.Utils
{
    public static class TweenUtils
    {
        public static float GetMaxTweenSeconds(this TweenSetting tweenSetting)
        {
            if (tweenSetting.IsSpeedBased)
            {
                Debug.LogWarning("Speed based tweens are not supported for calculating max duration.");
            }
            
            return tweenSetting.TweenDurationSeconds * tweenSetting.LoopCount + tweenSetting.RandomDelayRange.y;
        }
        
        public static Tweener DecorateTween(this Tweener tweener, TweenSetting tweenSetting)
        {
            return tweener
                .SetEase(tweenSetting.Ease)
                .SetLoops(tweenSetting.LoopCount, tweenSetting.LoopType)
                .SetSpeedBased(tweenSetting.IsSpeedBased)
                .SetDelay(tweenSetting.DelaySecondsFromRange);
        }
    }
}