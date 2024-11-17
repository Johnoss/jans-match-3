using UnityEngine;

namespace Scripts.Features.Tweening
{
    public class TweenConfig : ScriptableObject
    { 
        [Header("Move Piece Tweens")]
        [Tooltip("DOAnchorPos with target [0,0].")]
        [SerializeField] private TweenSetting _fallTweenSetting;
        [SerializeField] private TweenSetting _swapTweenSetting;
        [SerializeField] private TweenSetting _revertTweenSetting;
        
        public TweenSetting FallTweenSetting => _fallTweenSetting;
        public TweenSetting SwapTweenSetting => _swapTweenSetting;
        public TweenSetting RevertTweenSetting => _revertTweenSetting;
        
        [Header("Collect Piece Tweens")]
        [Tooltip("DoPunchScale with DoFade.")]
        [SerializeField] private TweenSetting _collectTweenSetting;
        public TweenSetting CollectTweenSetting => _collectTweenSetting;
        
        [Header("Visual Feedback Tweens")]
        [Tooltip("DoPunchScale")]
        [SerializeField] private TweenSetting _hintTweenSetting;
        [Tooltip("DoShakeRotation")]
        [SerializeField] private TweenSetting _invalidSwapTweenSetting;
        [SerializeField] private float _comboScaleMultiplier = 1.2f;
        [SerializeField] private float _comboShakeTweenSetting = 0.2f;
        
        public TweenSetting HintTweenSetting => _hintTweenSetting;
        public TweenSetting InvalidSwapTweenSetting => _invalidSwapTweenSetting;
        public float ComboScaleMultiplier => _comboScaleMultiplier;
        public float ComboShakeEndValue => _comboShakeTweenSetting;

        [Header("Launch Tweens")]
        [Tooltip("DoSizeDelta on the mask.")]
        [SerializeField] private TweenSetting _gridLaunchTweenSetting;
        
        public TweenSetting GridLaunchTweenSetting => _gridLaunchTweenSetting;
    }
}