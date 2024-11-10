using MVC;
using UnityEngine;
using Zenject;

namespace Scripts.Features.Grid
{
    [RequireComponent(typeof(RectTransform))]
    public class TileView : AbstractView
    {
        [SerializeField] private RectTransform _rectTransform;
        
        private int _entity;
        
        [Inject]
        public void Construct(int entity, GridConfig gridConfig)
        {
            _entity = entity;
        }
        
        public class ViewFactory : PlaceholderFactory<int, TileView>
        {
        }
    }
}