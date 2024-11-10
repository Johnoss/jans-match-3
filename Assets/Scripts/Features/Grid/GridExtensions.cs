using UnityEngine;

namespace Scripts.Features.Grid
{
    public static class GridExtensions
    {
        public static bool IsWithinBounds(this Vector2Int targetCoordinates, Vector2Int gridResolution)
        {
            return targetCoordinates.x >= 0 && targetCoordinates.x < gridResolution.x && targetCoordinates.y >= 0 && targetCoordinates.y < gridResolution.y;
        }
    }
}