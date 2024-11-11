using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Grid.Matching
{
    public class DetermineMatchesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MoveCompleteComponent, PieceComponent, PieceTileLinkComponent>> _moveCompleteFilter;
        
        private EcsCustomInject<MatchingService> _matchingService;
        private EcsCustomInject<GridService> _gridService;

        private EcsPoolInject<IsMatchComponent> _isMatchPool;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _moveCompleteFilter.Value)
            {
                var pieceTileLinkComponent = _moveCompleteFilter.Pools.Inc3.Get(entity);
                _moveCompleteFilter.Pools.Inc1.Del(entity);
                
                var tileEntity = pieceTileLinkComponent.LinkedEntity;
                
                var matchingNeighbours = _matchingService.Value.GetMatchingCandidates(tileEntity);
                if (matchingNeighbours.Length == 0)
                {
                    continue;
                }
                
                var legalMatches = _matchingService.Value.FindMatchesCoordinates(matchingNeighbours);
                MarkMatchedPieces(_gridService.Value.GetPieceEntitiesAtCoordinates(legalMatches));
            }
        }

        private void MarkMatchedPieces(HashSet<int> getPieceEntitiesAtCoordinates)
        {
            foreach (var pieceEntity in getPieceEntitiesAtCoordinates)
            {
                _isMatchPool.Value.Add(pieceEntity) = new IsMatchComponent();
            }
        }
    }
}