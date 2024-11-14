using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Features.Grid.Moving
{
    [CreateAssetMenu(menuName = "Create TweenConfig", fileName = "TweenConfig", order = 0)]
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
        [SerializeField] private Vector2 _collectPunchScale;
        
        public TweenSetting CollectTweenSetting => _collectTweenSetting;
        public Vector2 CollectPunchScale => _collectPunchScale;
        
        [Header("Visual Feedback Tweens")]
        [Tooltip("DoPunchScale")]
        [SerializeField] private TweenSetting _hintTweenSetting;
        [Tooltip("DoShakeRotation")]
        [SerializeField] private TweenSetting _invalidSwapTweenSetting;
        
        public TweenSetting HintTweenSetting => _hintTweenSetting;
        public TweenSetting InvalidSwapTweenSetting => _invalidSwapTweenSetting;
    }

    [Serializable]
    public class TweenSetting
    {
        public float TweenDurationSeconds = 1f;
        public float TweenDelaySeconds;
        public Ease Ease = Ease.InSine;
        public bool IsSpeedBased;
        
        public int LoopCount = 1;
        public LoopType LoopType = LoopType.Yoyo;
        
        [Tooltip("Anchor position, target punch scale, etc.")]
        public Vector3 TargetVector;
        
        public int VibrateCount;
    }
}