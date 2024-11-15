using DG.Tweening;
using Scripts.Utils;

namespace Scripts.Features.Tweening
{
    public class ShakeRotationTweenView : TweenView
    {
        private const float DEFAULT_RANDOMNESS = 90f;
        
        public void PlayTween(TweenSetting tweenSetting, out float totalSeconds)
        {
            ResetTween();
            
            totalSeconds = tweenSetting.GetMaxTweenSeconds();

            CachedTween = TargetTransform.DOShakeRotation(tweenSetting.TweenDurationSeconds, tweenSetting.TargetVector,
                tweenSetting.VibrateCount, DEFAULT_RANDOMNESS, true, ShakeRandomnessMode.Harmonic)
                .DecorateTween(tweenSetting);
        }
    }
}