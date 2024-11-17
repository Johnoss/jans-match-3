using DG.Tweening;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Tweening
{
    public class PunchScaleTween : TweenView
    {
        [SerializeField] private float _elasticity = 1f;
        
        private Vector2 _defaultScale;

        private void Awake()
        {
            _defaultScale = TargetTransform.localScale;
        }
        
        public override void PlayTween(TweenSetting tweenSetting, out float totalSeconds)
        {
            ResetTween();

            totalSeconds = tweenSetting.GetMaxTweenSeconds();

            CachedTween = TargetTransform.DOPunchScale(tweenSetting.TargetVector, tweenSetting.TweenDurationSeconds,
                tweenSetting.VibrateCount, _elasticity)
                .DecorateTween(tweenSetting);
        }
        
        public override void ResetTween()
        {
            CachedTween?.Kill();
            TargetTransform.localScale = _defaultScale;
        }
    }
}