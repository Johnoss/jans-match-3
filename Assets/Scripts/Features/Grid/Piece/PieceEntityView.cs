using DG.Tweening;
using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Moving;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.Piece
{
    [RequireComponent(typeof(RectTransform))]
    public class PieceEntityView : PoolableEntityView
    {
        [Header("References")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _bodyImage;
        [SerializeField] private Image _eyesImage;
        [SerializeField] private Image _mouthImage;

        [Header("Tween Target")]
        [SerializeField] private RectTransform _tweenTarget;
        
        
        private EntityViewPool<PieceEntityView> _entityViewPool;
        private PieceConfig _pieceConfig;
        private GridConfig _gridConfig;
        private EcsWorld _world;
        
        private PieceSetting _pieceSetting;

        private Tweener _moveTweenCache;
        private Tweener _shakeTweenCache;

        public RectTransform RectTransform => _rectTransform;

        [Inject]
        public void Construct(int entity, PieceConfig pieceConfig, GridConfig gridConfig, EntityViewPool<PieceEntityView> entityViewPool, EcsWorld world)
        {
            SetEntity(entity);
            _pieceConfig = pieceConfig;
            _gridConfig = gridConfig;
            _entityViewPool = entityViewPool;
            _world = world;
        }

        public override void ReturnToPool()
        {
            _entityViewPool.AddView(this);
        }

        public override void ResetView()
        {
            _mouthImage.overrideSprite = null;
            _eyesImage.overrideSprite = null;
            _bodyImage.overrideSprite = null;
            
            _rectTransform.anchoredPosition = Vector2.zero;
            
            SetupVisuals();
        }

        public override void DisableView()
        {
        }

        public void StartMoveTween(TweenSetting tweenSetting, Vector2 targetAnchorPosition)
        {
            _moveTweenCache?.Kill();

            _moveTweenCache = _tweenTarget.DOAnchorPos(targetAnchorPosition, tweenSetting.TweenDurationSeconds)
                .SetDelay(tweenSetting.TweenDelaySeconds)
                .SetEase(tweenSetting.Ease)
                .SetLoops(tweenSetting.LoopCount, tweenSetting.LoopType);
        }

        public void StartInvalidMoveTween(TweenSetting tweenSetting)
        {
            _moveTweenCache?.Restart();
            _shakeTweenCache?.Kill();

            _shakeTweenCache = _rectTransform.DOShakeRotation(tweenSetting.TweenDurationSeconds, tweenSetting.TargetVector, tweenSetting.VibrateCount, 90f,
                    true, ShakeRandomnessMode.Harmonic)
                .SetDelay(tweenSetting.TweenDelaySeconds)
                .SetEase(tweenSetting.Ease)
                .SetLoops(tweenSetting.LoopCount, tweenSetting.LoopType);
        }

        private void SetupVisuals()
        {
            var pieceTypeComponent = _world.GetPool<PieceTypeComponent>().Get(Entity);
            _rectTransform.sizeDelta = _gridConfig.TileSize;
            _pieceSetting = _pieceConfig.GetPieceSetting(pieceTypeComponent.TypeIndex);
            
            _bodyImage.sprite = _pieceSetting.BodySprite;
            _eyesImage.sprite = _pieceSetting.EyesSprite;
            _mouthImage.sprite = _pieceSetting.MouthSprite;
            _eyesImage.color = _pieceSetting.EyesTintColor;
        }

        public class ViewFactory : PlaceholderFactory<int, PieceEntityView>
        {
        }
    }
}