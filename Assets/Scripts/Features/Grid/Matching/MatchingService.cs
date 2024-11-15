using System;
using System.Collections.Generic;
using System.Linq;
using Initialization.ECS;
using Leopotam.EcsLite;
using Scripts.Features.Piece;
using Scripts.Features.Time;
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
        
        private int _possibleMovesEntity;
        
        public void CreatePossibleMovesEntity()
        {
            _possibleMovesEntity = _world.NewEntity();
            _world.GetPool<PossibleMovesValidatorComponent>().Add(_possibleMovesEntity);
        }
        
        public void SetBoardDirty()
        {
            _world.GetPool<FoundPossibleMovesComponent>().DelOrSkip(_possibleMovesEntity);
            _world.GetPool<NoPossibleMovesComponent>().DelOrSkip(_possibleMovesEntity);
            
            RestartCheckingPossibleMoves();
        }

        private void RestartCheckingPossibleMoves()
        {
            ref var expireComponent = ref _world.GetPool<ExpireComponent>().GetOrAddComponent(_possibleMovesEntity);
            ref var checkingComponent = ref _world.GetPool<PossibleMovesValidatorComponent>().Get(_possibleMovesEntity);
            
            checkingComponent.CurrentIterationCoordinates = Vector2Int.zero;
            expireComponent.RemainingSeconds = _rulesConfig.PossibleMoveCheckDelaySeconds;
        }
        
        public bool HasPossibleMatch(Vector2Int coordinates)
        {
            ref var checkingComponent = ref _world.GetPool<PossibleMovesValidatorComponent>().Get(_possibleMovesEntity);
            checkingComponent.CurrentIterationCoordinates = coordinates;
            
            var tileEntity = _gridService.GetTileEntity(coordinates);
            if (tileEntity == ECSTypes.NULL)
            {
                return false;
            }
            
            var tilePieceLinkPool = _world.GetPool<PieceTileLinkComponent>();
            
            if (!tilePieceLinkPool.Has(tileEntity))
            {
                return false;
            }
            
            var pieceEntity = tilePieceLinkPool.Get(tileEntity).LinkedEntity;
            if (pieceEntity == ECSTypes.NULL)
            {
                return false;
            }
            
            var currentPieceTypeIndex = _world.GetPool<PieceTypeComponent>().Get(pieceEntity).TypeIndex;
            
            var tileComponent = _world.GetPool<TileComponent>().Get(tileEntity);
            var simulatedPieceTypes = new Dictionary<Vector2Int, int>();
            foreach (var neighboringCoordinate in tileComponent.NeighboringTileCoordinates)
            {
                simulatedPieceTypes.Clear();
                var neighborTileEntity = _gridService.GetTileEntity(neighboringCoordinate);
                if (neighborTileEntity == ECSTypes.NULL)
                {
                    continue;
                }

                if (!_world.GetPool<PieceTileLinkComponent>().Has(neighborTileEntity) || !tilePieceLinkPool.Has(neighborTileEntity))
                {
                    continue;
                }
                
                var neighborPieceEntity = tilePieceLinkPool.Get(neighborTileEntity).LinkedEntity;

                if (!_world.GetPool<PieceTypeComponent>().Has(neighborPieceEntity))
                {
                    continue;
                }

                var neighborPieceTypeIndex = _world.GetPool<PieceTypeComponent>().Get(neighborPieceEntity).TypeIndex;
                
                simulatedPieceTypes.Add(neighboringCoordinate, currentPieceTypeIndex);
                simulatedPieceTypes.Add(coordinates, neighborPieceTypeIndex);

                var matchingCandidates = GetMatchingCandidates(coordinates, simulatedPieceTypes);
                var matches = FindMatchesCoordinates(matchingCandidates);
                if (matches.Count > 0)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public HashSet<Vector2Int> FindMatchesCoordinates(Vector2Int[] candidateCoordinates)
        {
            return MatchUtils.FindMatches(candidateCoordinates, _rulesConfig);
        }
        
        public Vector2Int[] GetMatchingCandidates(int tileEntity, Dictionary<Vector2Int, int> simulatedOverrideTiles = null)
        {
            var coordinates = _world.GetPool<TileComponent>().Get(tileEntity).Coordinates;
            return tileEntity == ECSTypes.NULL ? Array.Empty<Vector2Int>() : GetMatchingCandidates(coordinates, simulatedOverrideTiles);
        }
        
        public Vector2Int[] GetMatchingCandidates(Vector2Int coordinates, Dictionary<Vector2Int, int> simulatedOverrideTiles = null)
        {
            if (!TryGetPieceTypeIndex(coordinates, simulatedOverrideTiles, out var pieceTypeIndex))
            {
                return Array.Empty<Vector2Int>();
            }

            var visited = new HashSet<Vector2Int>();
            var matchingNeighboursCoordinates = new HashSet<Vector2Int> { coordinates };

            GetMatchingNeighboursRecursive(coordinates, pieceTypeIndex, visited, matchingNeighboursCoordinates, simulatedOverrideTiles);

            return matchingNeighboursCoordinates.Count >= _rulesConfig.MinMatchLength ? matchingNeighboursCoordinates.ToArray() : Array.Empty<Vector2Int>();
        }

        private void GetMatchingNeighboursRecursive(Vector2Int coordinates, int lookupPieceType, HashSet<Vector2Int> visited, HashSet<Vector2Int> matchingNeighbours, Dictionary<Vector2Int, int> simulatedOverrideTiles = null)
        {
            var tileEntity = _gridService.GetTileEntity(coordinates);
            if(tileEntity == ECSTypes.NULL)
            {
                return;
            }

            if (!TryGetPieceTypeIndex(coordinates, simulatedOverrideTiles, out var compareTypeIndex))
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
                GetMatchingNeighboursRecursive(neighbour, lookupPieceType, visited, matchingNeighbours, simulatedOverrideTiles);
            }
        }

        private bool TryGetPieceTypeIndex(Vector2Int coordinates, Dictionary<Vector2Int, int> simulatedOverrideTiles, out int pieceTypeIndex)
        {
            pieceTypeIndex = -1;

            if ( simulatedOverrideTiles != null && simulatedOverrideTiles.TryGetValue(coordinates, out var simulateIndex))
            {
                pieceTypeIndex =  simulateIndex;
                return true;
            }

            var pieceEntity = _gridService.GetPieceEntity(coordinates);
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