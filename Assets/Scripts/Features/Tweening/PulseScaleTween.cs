using DG.Tweening;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Tweening
{
    public class PulseScaleTween : TweenView
    {
        [SerializeField] private float _elasticity = 1f;

        public void PlayTween(TweenSetting tweenSetting, out float totalSeconds)
        {
            ResetTween();

            totalSeconds = tweenSetting.GetMaxTweenSeconds();

            CachedTween = TargetTransform.DOPunchScale(tweenSetting.TargetVector, tweenSetting.TweenDurationSeconds,
                tweenSetting.VibrateCount, _elasticity)
                .DecorateTween(tweenSetting);
        }
    }
}