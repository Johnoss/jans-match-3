using UnityEngine;

namespace Scripts.Features.Grid
{
    public struct TileComponent
    {
        public Vector2Int Coordinates;
        public Vector2Int[] NeighboringTileCoordinates;
    }
}