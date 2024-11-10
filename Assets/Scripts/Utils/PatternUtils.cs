using System.Linq;
using UnityEngine;

namespace Scripts.Utils
{
    public static class PatternUtils
    {
        public static Vector2Int GetOffset(this Vector2Int[] pattern)
        {
            var minX = pattern.Min(coordinates => coordinates.x);
            var minY = pattern.Min(coordinates => coordinates.y);
            return new Vector2Int(minX, minY);
        }
        
        public static Vector2Int[] NormalizePattern(this Vector2Int[] inputPattern, Vector2Int offset)
        {
            return inputPattern
                .Select(coordinates => new Vector2Int(coordinates.x - offset.x, coordinates.y - offset.y))
                .OrderBy(coordinates=> coordinates.x)
                .ThenBy(coordinates=> coordinates.y)
                .ToArray();
        }
    }
}