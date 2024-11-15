using DG.Tweening;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Tweening;
using Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Scripts.Features.Grid
{
    public class GridView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _tilesParent;
        [SerializeField] private RectTransform _piecesParent;

        [Header("Masking")]
        [SerializeField] private RectTransform _maskRectTransform;

        [Inject] private GridConfig _gridConfig;
        [Inject] private TweenConfig _tweenConfig;

        private Tweener _startTweener;
        
        public Transform TilesParent => _tilesParent.transform;
        public Transform PiecesParent => _piecesParent;
        
        public void SetupGridView()
        {
            PlayStartAnimation();
            
            _maskRectTransform.sizeDelta = Vector2.zero;
        }
        
        private void PlayStartAnimation()
        {
            var tweenSetting = _tweenConfig.GridLaunchTweenSetting;
            var targetSizeDelta = _gridConfig.GetGridDimensions(); 
            _maskRectTransform.sizeDelta = Vector2.zero;
            _startTweener = _maskRectTransform.DOSizeDelta(targetSizeDelta, tweenSetting.TweenDurationSeconds)
                .SetLoops(tweenSetting.LoopCount, tweenSetting.LoopType)
                .SetDelay(tweenSetting.DelaySecondsFromRange)
                .SetEase(tweenSetting.Ease);
        }
    }
}