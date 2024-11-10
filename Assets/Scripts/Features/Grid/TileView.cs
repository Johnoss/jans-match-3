using UnityEngine;
using Zenject;

namespace Scripts.Features.Grid
{
    [RequireComponent(typeof(RectTransform))]
    public class TileView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        
        private int _entity;
        
        [Inject]
        public void Construct(int entity, GridConfig gridConfig)
        {
            _entity = entity;
        }
        
        public class TileFactory : PlaceholderFactory<int, TileView>
        {
        }
    }
}