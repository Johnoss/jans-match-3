using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Grid;
using UniRx;
using UniRx.Triggers;
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
        
        private EntityViewPool<PieceEntityView> _entityViewPool;
        private PieceConfig _pieceConfig;
        private GridConfig _gridConfig;
        private EcsWorld _world;
        
        private PieceSetting _pieceSetting;

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

        public class ViewFactory : PlaceholderFactory<int, PieceEntityView>
        {
        }
    }
}