using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Input;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.Grid
{
    [RequireComponent(typeof(RectTransform))]
    public class TileView : AbstractView
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _pieceAnchor;
        
        [Header("Interaction")]
        [SerializeField] private GraphicRaycaster _hitbox;
        
        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI _debugText;
        
        public RectTransform PieceAnchor => _pieceAnchor;
        
        private int _entity;
        private EcsPool<UserInteractingComponent> _userInteractingPool;

        [Inject]
        public void Construct(int entity, GridConfig gridConfig, EcsWorld world)
        {
            _entity = entity;
            _userInteractingPool = world.GetPool<UserInteractingComponent>();

            //TODO disposer
            _hitbox.OnPointerEnterAsObservable()
                .Subscribe(_ => OnPieceSelected());

#if UNITY_EDITOR
            var tilePool = world.GetPool<TileComponent>();
            var tileComponent = tilePool.Get(_entity);
            _debugText.text = $"[{tileComponent.Coordinates.x}, {tileComponent.Coordinates.y}]";
#else
            _debugText.gameObject.SetActive(false);
#endif
        }
        
        private void OnPieceSelected()
        {
            if (_userInteractingPool.Has(_entity))
            {
                return;
            }
            Debug.Log($"Piece {_entity} entered");
            _userInteractingPool.Add(_entity) = new UserInteractingComponent();
        }
        
        public class ViewFactory : PlaceholderFactory<int, TileView>
        {
        }
    }
}