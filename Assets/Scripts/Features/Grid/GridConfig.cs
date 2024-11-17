using UnityEngine;

namespace Scripts.Features.Grid
{
    [CreateAssetMenu(menuName = "Create GridConfig", fileName = "GridConfig", order = 0)]
    public class GridConfig : ScriptableObject
    {
        [Header("Grid")]
        [SerializeField] private Vector2Int _gridResolution;
        [SerializeField] private Vector2Int[] _neighboringOffsets = {
            new(0, 1),
            new(1, 0),
            new(0, -1),
            new(-1, 0)
        };
        
        public Vector2Int GridResolution => _gridResolution;
        public Vector2Int[] NeighboringOffsets => _neighboringOffsets;
        public int GridSize => _gridResolution.x * _gridResolution.y;
        
        [Header("Tile")]
        [SerializeField] private Vector2 _tileSize;
        [SerializeField] private TileView _tilePrefab;
        
        public Vector2 TileSize => _tileSize;
        public TileView TilePrefab => _tilePrefab;
    }
}