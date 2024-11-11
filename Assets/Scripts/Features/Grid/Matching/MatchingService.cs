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
        
        public HashSet<Vector2Int> FindMatches(Vector2Int[] candidateCoordinates)
        {
            return MatchUtils.FindMatches(candidateCoordinates, _rulesConfig);
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
            if (matchingNeighboursCoordinates.Count >= _rulesConfig.MinMatchLength)
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