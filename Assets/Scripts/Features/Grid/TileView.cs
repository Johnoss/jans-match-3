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
        
        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI _debugText;
        
        private int _entity;

        public RectTransform RectTransform => _rectTransform;
        
        [Inject]
        public void Construct(int entity, GridConfig gridConfig, EcsWorld world)
        {
            _entity = entity;

#if UNITY_EDITOR || DEBUG
            var tilePool = world.GetPool<TileComponent>();
            var tileComponent = tilePool.Get(_entity);
            _debugText.text = $"[{tileComponent.Coordinates.x}, {tileComponent.Coordinates.y}]";
#else
            _debugText.gameObject.SetActive(false);
#endif
        }
        
        public class ViewFactory : PlaceholderFactory<int, TileView>
        {
        }
    }
}