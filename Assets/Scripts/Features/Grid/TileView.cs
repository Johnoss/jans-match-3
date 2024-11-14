using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.Grid
{
    [RequireComponent(typeof(RectTransform))]
    public class TileView : AbstractView
    {
        [SerializeField] private RectTransform _rectTransform;
        
        [Header("Interaction")]
        [SerializeField] private Button _hitbox;
        
        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI _debugText;
        
        private int _entity;
        private EcsPool<UserInteractingComponent> _userInteractingPool;
        private EcsWorld _world;

        [Inject]
        public void Construct(int entity, GridConfig gridConfig, EcsWorld world)
        {
            _entity = entity;
            _world = world;
            _userInteractingPool = world.GetPool<UserInteractingComponent>();

            //TODO disposer
            _hitbox.OnPointerEnterAsObservable()
                .Merge(_hitbox.OnPointerDownAsObservable())
                .Subscribe(_ => OnPieceSelected());
            
            _hitbox.OnPointerUpAsObservable().Subscribe(OnPieceClicked);

#if UNITY_EDITOR || DEBUG
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
            _userInteractingPool.Add(_entity) = new UserInteractingComponent();
        }

        private void OnPieceClicked(PointerEventData pointerEventData)
        {
#if DEBUG
            if(pointerEventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }
            
            if(!_world.GetPool<PieceTileLinkComponent>().Has(_entity))
            {
                Debug.Log($"Tile Right Clicked: {_entity}, no piece linked");
                return;
            }
            
            Debug.Log($"Tile Right Clicked: {_entity}, destroying piece");
            var pieceEntity = _world.GetPool<PieceTileLinkComponent>().Get(_entity).LinkedEntity;
            
            var matchPool = _world.GetPool<IsMatchComponent>();
            if(matchPool.Has(pieceEntity))
            {
                return;
            }
            matchPool.Add(pieceEntity);
#endif
        }
        
        public class ViewFactory : PlaceholderFactory<int, TileView>
        {
        }
    }
}