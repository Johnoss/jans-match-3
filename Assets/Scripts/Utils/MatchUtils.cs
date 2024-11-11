using System.Collections.Generic;
using System.Linq;
using Scripts.Features.Grid.Matching;
using UnityEngine;

namespace Scripts.Utils
{
    public static class MatchUtils
    {
        public static HashSet<Vector2Int> FindMatches(Vector2Int[] candidateCoordinates, RulesConfig rulesConfig)
        {
            var minLineLength = rulesConfig.MinMatchLength;
            var grid = candidateCoordinates.ConvertToGrid(out var width, out var height);

            var matches = new HashSet<Vector2Int>();

            matches.UnionWith(FindHorizontalMatches(grid, width, height, minLineLength));
            matches.UnionWith(FindVerticalMatches(grid, width, height, minLineLength));
            if (rulesConfig.AllowSquareMatches)
            {
                matches.UnionWith(FindSquareMatches(grid, width, height));
            }

            return matches;
        }

        private static int[,] ConvertToGrid(this Vector2Int[] candidateCoordinates, out int width, out int height)
        {
            var minX = candidateCoordinates.Min(coord => coord.x);
            var minY = candidateCoordinates.Min(coord => coord.y);
            var maxX = candidateCoordinates.Max(coord => coord.x);
            var maxY = candidateCoordinates.Max(coord => coord.y);

            width = maxX - minX + 1;
            height = maxY - minY + 1;

            var grid = new int[width, height];

            foreach (var coord in candidateCoordinates)
            {
                grid[coord.x - minX, coord.y - minY] = 1;
            }

            return grid;
        }

        private static HashSet<Vector2Int> FindHorizontalMatches(int[,] grid, int width, int height, int minLineLength)
        {
            var matches = new HashSet<Vector2Int>();

            if (minLineLength > width)
            {
                return matches;
            }
            
            for (var y = 0; y < height; y++)
            {
                var count = 0;
                for (var x = 0; x < width; x++)
                {
                    if (grid[x, y] == 1)
                    {
                        count++;
                    }
                    else
                    {
                        if (count >= minLineLength)
                        {
                            AddHorizontalMatch(matches, x - count, y, count);
                        }
                        count = 0;
                    }
                }

                if (count >= minLineLength)
                {
                    AddHorizontalMatch(matches, width - count, y, count);
                }
            }

            return matches;
        }

        private static HashSet<Vector2Int> FindVerticalMatches(int[,] grid, int width, int height, int minLineLength)
        {
            var matches = new HashSet<Vector2Int>();

            if (minLineLength > height)
            {
                return matches;
            }
            
            for (var x = 0; x < width; x++)
            {
                var count = 0;
                for (var y = 0; y < height; y++)
                {
                    if (grid[x, y] == 1)
                    {
                        count++;
                    }
                    else
                    {
                        if (count >= minLineLength)
                        {
                            AddVerticalMatch(matches, x, y - count, count);
                        }
                        count = 0;
                    }
                }

                if (count >= minLineLength) // Check end of column
                {
                    AddVerticalMatch(matches, x, height - count, count);
                }
            }

            return matches;
        }

        private static HashSet<Vector2Int> FindSquareMatches(int[,] grid, int width, int height)
        {
            var matches = new HashSet<Vector2Int>();

            for (var x = 0; x < width - 1; x++)
            {
                for (var y = 0; y < height - 1; y++)
                {
                    // Check for a 2x2 square of 1s
                    if (grid[x, y] == 1 && grid[x + 1, y] == 1 && grid[x, y + 1] == 1 && grid[x + 1, y + 1] == 1)
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x + 1, y));
                        matches.Add(new Vector2Int(x, y + 1));
                        matches.Add(new Vector2Int(x + 1, y + 1));

                        return matches;
                    }
                }
            }

            return matches;
        }

        private static void AddHorizontalMatch(HashSet<Vector2Int> matches, int startX, int y, int count)
        {
            for (var i = 0; i < count; i++)
            {
                matches.Add(new Vector2Int(startX + i, y));
            }
        }

        private static void AddVerticalMatch(HashSet<Vector2Int> matches, int x, int startY, int count)
        {
            for (var i = 0; i < count; i++)
            {
                matches.Add(new Vector2Int(x, startY + i));
            }
        }
    }
}
