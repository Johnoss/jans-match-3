using DG.Tweening;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Tweening
{
    public class AnchorPositionTweenView : TweenView
    {
        public void PlayTween(TweenSetting tweenSetting, Vector2 targetAnchorPosition, out float totalSeconds)
        {
            ResetTween();
            
            totalSeconds = tweenSetting.GetMaxTweenSeconds();

            CachedTween = TargetTransform.DOAnchorPos(targetAnchorPosition, tweenSetting.TweenDurationSeconds)
                .DecorateTween(tweenSetting);

            if (!tweenSetting.IsSpeedBased)
            {
                return;
            }
            
            var distance = Vector2.Distance(TargetTransform.anchoredPosition, targetAnchorPosition);
            totalSeconds = distance / totalSeconds;
        }

        public override void ResetTween()
        {
            CachedTween?.Kill();
        }
    }
}