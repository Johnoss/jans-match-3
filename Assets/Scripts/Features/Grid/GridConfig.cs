using UnityEngine;

namespace Scripts.Features.Grid
{
    [CreateAssetMenu(menuName = "Create GridConfig", fileName = "GridConfig", order = 0)]
    public class GridConfig : ScriptableObject
    {
        [Header("Grid")]
        [SerializeField] private Vector2Int _gridSize;
        public Vector2Int GridSize => _gridSize;
        
        [Header("Tile")]
        [SerializeField] private Vector2 _tileSize;
        [SerializeField] private TileView _tilePrefab;
        
        public Vector2 TileSize => _tileSize;
        public TileView TilePrefab => _tilePrefab;
    }
}