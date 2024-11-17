using DG.Tweening;
using Scripts.Features.GameSession;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Tweening;
using Scripts.Utils;
using UniRx;
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
        [Inject] private GameSessionModel _gameSessionModel;
        [Inject] private CompositeDisposable _disposer;

        private Tweener _startTweener;
        
        public RectTransform TilesParent => _tilesParent;
        public RectTransform PiecesParent => _piecesParent;
        
        public void SetupGridView()
        {
            _maskRectTransform.sizeDelta = Vector2.zero;
            
            _maskRectTransform.sizeDelta = Vector2.zero;
            
            _gameSessionModel.IsGameRunning
                .Where(isRunning => isRunning)
                .Subscribe(_ => PlayStartAnimation()).AddTo(_disposer);
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