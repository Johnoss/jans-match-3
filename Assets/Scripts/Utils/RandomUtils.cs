using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Scripts.Utils
{
    public static class RandomUtils
    {
        private static Random _random;

        static RandomUtils()
        {
            _random = new Random();
            _random.InitState();
        }
        
        public static int GetRandomInt(int max)
        {
            return GetRandomInt(0, max);
        }

        public static int GetRandomInt(int min, int max)
        {
            return _random.NextInt(min, max);
        }
        
        public static IEnumerable<Vector2Int> GetShuffledCoordinates(this Vector2Int gridResolution)
        {
            var coordinates = Enumerable.Range(0, gridResolution.x)
                .SelectMany(x => Enumerable.Range(0, gridResolution.y), (x, y) => new Vector2Int(x, y))
                .ToList();

            for (var i = 0; i < coordinates.Count; i++)
            {
                var randomIndex = GetRandomInt(i, coordinates.Count);
                (coordinates[i], coordinates[randomIndex]) = (coordinates[randomIndex], coordinates[i]);
            }

            return coordinates;
        }
    }
}