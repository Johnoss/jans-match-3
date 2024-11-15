using System;
using DG.Tweening;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Tweening
{
    [Serializable]
    public class TweenSetting
    {
        [SerializeField] private float _tweenDurationSeconds = 1f;
        [SerializeField] private Ease _ease = Ease.InSine;
        [SerializeField] private bool _isSpeedBased;

        [SerializeField] private int _loopCount = 1;
        [SerializeField] private LoopType _loopType = LoopType.Yoyo;

        [Tooltip("Anchor position, target punch scale, etc.")]
        [SerializeField] private Vector3 _targetVector;

        [SerializeField] private Vector2 _randomDelayRange;
        [SerializeField] private int _vibrateCount;
        
        public float TweenDurationSeconds => _tweenDurationSeconds;
        public Vector2 RandomDelayRange => _randomDelayRange;
        public float DelaySecondsFromRange => _randomDelayRange.GetRandomFloat();
        public Ease Ease => _ease;
        public bool IsSpeedBased => _isSpeedBased;
        public int LoopCount => _loopCount;
        public LoopType LoopType => _loopType;
        public Vector3 TargetVector => _targetVector;
        public int VibrateCount => _vibrateCount;
    }
}