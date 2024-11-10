using System;
using System.Collections.Generic;
using System.Linq;
using Initialization.ECS;
using Leopotam.EcsLite;
using Scripts.Features.Piece;
using Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Scripts.Features.Grid.Matching
{
    public class MatchingService
    {
        [Inject] private GridService _gridService;
        [Inject] private RulesConfig _rulesConfig;
        [Inject] private EcsWorld _world;
        
        public HashSet<Vector2Int> GetMatchingPatternCoordinates(Vector2Int[] pattern)
        {
            var legalMatchesCoordinates = new HashSet<Vector2Int>();

            foreach (var legalPattern in _rulesConfig.LegalPatternSettings)
            {
                var matchedCoordinates = FindPatternMatches(pattern, legalPattern);
                foreach (var coordinates in matchedCoordinates)
                {
                    legalMatchesCoordinates.Add(coordinates);
                }
            }

            return legalMatchesCoordinates;
        }

        private IEnumerable<Vector2Int> FindPatternMatches(Vector2Int[] pattern, PatternSetting legalPattern)
        {
            for (var i = 0; i <= pattern.Length - legalPattern.Pattern.Length; i++)
            {
                
                var subPattern = pattern.Skip(i).Take(legalPattern.Pattern.Length).ToArray();
                var offset = subPattern.GetOffset();
                subPattern = subPattern.NormalizePattern(offset);

                if (!subPattern.SequenceEqual(legalPattern.Pattern))
                {
                    continue;
                }
                foreach (var coordinates in subPattern)
                {
                    yield return coordinates;
                }
            }
        }
        
        public Vector2Int[] GetMatchingCandidates(int tileEntity)
        {
            var tileComponent = _world.GetPool<TileComponent>().Get(tileEntity);
            if(!_world.GetPool<PieceTileLinkComponent>().Has(tileEntity))
            {
                return Array.Empty<Vector2Int>();
            }
            
            var pieceEntity = _world.GetPool<PieceTileLinkComponent>().Get(tileEntity).LinkedEntity;
            
            if(pieceEntity == ECSTypes.NULL)
            {
                return Array.Empty<Vector2Int>();
            }
            
            var coordinates = tileComponent.Coordinates;
            var pieceType = _world.GetPool<PieceTypeComponent>().Get(pieceEntity).TypeIndex;

            var matchingNeighboursCoordinates = new HashSet<Vector2Int>() {coordinates};
            var visited = new HashSet<Vector2Int>();
            GetMatchingNeighboursRecursive(coordinates, pieceType, visited, matchingNeighboursCoordinates);

            //TODO remove
            if (matchingNeighboursCoordinates.Count > _rulesConfig.MinPatternLength)
            {
                Debug.Log($"Matching neighbours coordinates: {string.Join(", ", matchingNeighboursCoordinates)}");
            }
            else
            {
                return Array.Empty<Vector2Int>();
            }
            
            return matchingNeighboursCoordinates.ToArray();
        }

        private void GetMatchingNeighboursRecursive(Vector2Int coordinates, int pieceType, HashSet<Vector2Int> visited, HashSet<Vector2Int> matchingNeighbours)
        {
            var tileEntity = _gridService.GetTileEntity(coordinates);
            if(tileEntity == ECSTypes.NULL)
            {
                return;
            }
            
            if(!_world.GetPool<PieceTileLinkComponent>().Has(tileEntity))
            {
                return;
            }
            
            var pieceEntity = _world.GetPool<PieceTileLinkComponent>().Get(tileEntity).LinkedEntity;
            if(pieceEntity == ECSTypes.NULL)
            {
                return;
            }
            
            var pieceTypeComponent = _world.GetPool<PieceTypeComponent>().Get(pieceEntity);
            if(pieceTypeComponent.TypeIndex != pieceType)
            {
                return;
            }

            if (!visited.Add(coordinates))
            {
                return;
            }
            
            matchingNeighbours.Add(coordinates);

            var tileComponent = _world.GetPool<TileComponent>().Get(tileEntity);
            foreach (var neighbour in tileComponent.NeighboringTileCoordinates)
            {
                GetMatchingNeighboursRecursive(neighbour, pieceType, visited, matchingNeighbours);
            }
        }
    }
}