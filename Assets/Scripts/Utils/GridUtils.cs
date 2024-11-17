using Scripts.Features.Grid;
using UnityEngine;

namespace Scripts.Utils
{
    public static class GridUtils
    {
        
        public static Vector2 GetTileAnchorPosition(this GridConfig gridConfig, int x, int y)
        {
            return gridConfig.GetTileAnchorPosition(new Vector2Int(x, y));
        }
        
        public static Vector2 GetTileAnchorPosition(this GridConfig gridConfig, Vector2Int coordinates)
        {
            var resolution = gridConfig.GridResolution;
            var tileSize = gridConfig.TileSize;
            var boardSize = new Vector2(resolution.x * tileSize.x, resolution.y * tileSize.y);
            
            return new Vector2(
                coordinates.x * tileSize.x - boardSize.x / 2 + tileSize.x / 2,
                coordinates.y * tileSize.y - boardSize.y / 2 + tileSize.y / 2
            );
        }
        
        public static Vector2 GetGridDimensions(this GridConfig gridConfig)
        {
            return gridConfig.GridResolution * gridConfig.TileSize;
        }
    }
}