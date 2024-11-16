using DG.Tweening;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Tweening
{
    public class FadeOutTweenView : TweenView
    {
        private CanvasGroup _canvasGroup;
        private float _defaultCanvasAlpha;

        private void Awake()
        {
            _canvasGroup = TargetTransform.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component not found on target transform");
                return;
            }
            
            _defaultCanvasAlpha = _canvasGroup.alpha;
        }

        public override void PlayTween(TweenSetting tweenSetting, out float totalSeconds)
        {
            ResetTween();

            totalSeconds = tweenSetting.GetMaxTweenSeconds();

            CachedTween = _canvasGroup.DOFade(0f, tweenSetting.TweenDurationSeconds)
                .SetDelay(tweenSetting.RandomDelayRange.x)
                .SetEase(Ease.Linear);
        }

        public override void ResetTween()
        {
            CachedTween?.Kill();
            _canvasGroup.alpha = _defaultCanvasAlpha;
        }
    }
}