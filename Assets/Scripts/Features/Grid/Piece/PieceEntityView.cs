using DG.Tweening;
using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Grid;
using Scripts.Features.Tweening;
using UnityEngine;
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

        [Header("Tweens")]
        [SerializeField] private AnchorPositionTweenView _moveTweenView;
        [SerializeField] private ShakeRotationTweenView _invalidMoveTweenView;
        [SerializeField] private PunchScaleTween _hintTweenView;
        [SerializeField] private TweenView[] _collectTweenViews;
        
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
            _moveTweenView.ResetTween();
            _invalidMoveTweenView.ResetTween();
            _hintTweenView.ResetTween();
            foreach (var tweenView in _collectTweenViews)
            {
                tweenView.ResetTween();
            }
        }

        public void StartMoveTween(TweenSetting tweenSetting, Vector2 targetAnchorPosition, out float seconds)
        {
            _moveTweenView.PlayTween(tweenSetting, targetAnchorPosition, out seconds);
        }

        public void StartInvalidMoveTween(TweenSetting tweenSetting, out float totalSeconds)
        {
            _invalidMoveTweenView.PlayTween(tweenSetting, out totalSeconds);
        }
        
        public void ToggleHintTween(TweenSetting tweenSetting, bool enable)
        {
            _hintTweenView.PlayTween(tweenSetting, out _);
        }

        public void StartCollectTween(TweenSetting tweenSetting, out float totalSeconds)
        {
            totalSeconds = 0f;
            foreach (var tweenView in _collectTweenViews)
            {
                tweenView.ResetTween();
                tweenView.PlayTween(tweenSetting, out var seconds);
                totalSeconds = Mathf.Max(totalSeconds, seconds);
            }
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