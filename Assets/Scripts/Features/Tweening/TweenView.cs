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
        
        public void ResetTween()
        {
            CachedTween?.Kill();
        }
    }
}