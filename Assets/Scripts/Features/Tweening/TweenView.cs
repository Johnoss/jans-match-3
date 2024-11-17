using DG.Tweening;
using MVC;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Tweening
{
    public abstract class TweenView : AbstractView
    {
        [SerializeField] protected RectTransform TargetTransform;
        
        protected Tweener CachedTween;

        public abstract void ResetTween();

        public virtual void PlayTween(TweenSetting tweenSetting, out float totalSeconds)
        {
            totalSeconds = tweenSetting.GetMaxTweenSeconds();
        }
    }
}