using System;
using DG.Tweening;
using UnityEngine;

namespace Scripts.Features.Grid.Moving
{
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