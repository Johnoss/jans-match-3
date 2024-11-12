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
        
        public HashSet<Vector2Int> FindMatchesCoordinates(Vector2Int[] candidateCoordinates)
        {
            return MatchUtils.FindMatches(candidateCoordinates, _rulesConfig);
        }
        
        public Vector2Int[] GetMatchingCandidates(Vector2Int coordinates, int? simulateIndex = null)
        {
            var tileEntity = _gridService.GetTileEntity(coordinates);
            return tileEntity == ECSTypes.NULL ? Array.Empty<Vector2Int>() : GetMatchingCandidates(tileEntity, simulateIndex);
        }
        
        public Vector2Int[] GetMatchingCandidates(int tileEntity, int? simulateIndex = null)
        {
            var tileComponent = _world.GetPool<TileComponent>().Get(tileEntity);

            if (!TryGetPieceTypeIndex(tileEntity, simulateIndex, out var pieceTypeIndex))
            {
                return Array.Empty<Vector2Int>();
            }

            var coordinates = tileComponent.Coordinates;
            var visited = new HashSet<Vector2Int>() { };
            var matchingNeighboursCoordinates = new HashSet<Vector2Int> { coordinates };

            GetMatchingNeighboursRecursive(coordinates, pieceTypeIndex, visited, matchingNeighboursCoordinates, simulateIndex);

            return matchingNeighboursCoordinates.Count >= _rulesConfig.MinMatchLength ? matchingNeighboursCoordinates.ToArray() : Array.Empty<Vector2Int>();
        }

        private void GetMatchingNeighboursRecursive(Vector2Int coordinates, int lookupPieceType, HashSet<Vector2Int> visited, HashSet<Vector2Int> matchingNeighbours, int? simulateType = null)
        {
            var tileEntity = _gridService.GetTileEntity(coordinates);
            if(tileEntity == ECSTypes.NULL)
            {
                return;
            }

            if (!TryGetPieceTypeIndex(tileEntity, simulateType, out var compareTypeIndex))
            {
                return;
            }
            
            if (compareTypeIndex != lookupPieceType)
            {
                return;
            }
            
            matchingNeighbours.Add(coordinates);
            
            if (!visited.Add(coordinates))
            {
                return;
            }

            var tileComponent = _world.GetPool<TileComponent>().Get(tileEntity);
            foreach (var neighbour in tileComponent.NeighboringTileCoordinates)
            {
                GetMatchingNeighboursRecursive(neighbour, lookupPieceType, visited, matchingNeighbours);
            }
        }

        private bool TryGetPieceTypeIndex(int tileEntity, int? simulateIndex, out int pieceTypeIndex)
        {
            pieceTypeIndex = -1;

            if (simulateIndex != null)
            {
                pieceTypeIndex =  simulateIndex.Value;
                return true;
            }
            
            if(!_world.GetPool<PieceTileLinkComponent>().Has(tileEntity))
            {
                return false;
            }
            
            var pieceEntity = _world.GetPool<PieceTileLinkComponent>().Get(tileEntity).LinkedEntity;

            if (pieceEntity == ECSTypes.NULL)
            {
                return false;
            }
            
            if (!_world.GetPool<PieceTypeComponent>().Has(pieceEntity))
            {
                return false;
            }

            pieceTypeIndex = _world.GetPool<PieceTypeComponent>().Get(pieceEntity).TypeIndex;

            return true;
        }
    }
}