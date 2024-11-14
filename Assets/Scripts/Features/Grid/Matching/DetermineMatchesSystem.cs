using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Input;
using Scripts.Features.Piece;

namespace Scripts.Features.Grid.Matching
{
    public class DetermineMatchesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MoveCompleteComponent, PieceComponent, PieceTileLinkComponent>> _moveCompleteFilter;
        
        private EcsCustomInject<MatchingService> _matchingService;
        private EcsCustomInject<GridService> _gridService;

        private EcsPoolInject<SwapPieceComponent> _swapPiecePool;
        private EcsPoolInject<IsMatchComponent> _isMatchPool;
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _moveCompleteFilter.Value)
            {
                var pieceTileLinkComponent = _moveCompleteFilter.Pools.Inc3.Get(pieceEntity);
                
                var tileEntity = pieceTileLinkComponent.LinkedEntity;
                
                var matchingNeighbours = _matchingService.Value.GetMatchingCandidates(tileEntity);
                var legalMatches = _matchingService.Value.FindMatchesCoordinates(matchingNeighbours);
                
                if (legalMatches.Count == 0)
                {
                    continue;
                }

                MarkMatchedPieces(_gridService.Value.GetPieceEntitiesAtCoordinates(legalMatches));
            }
        }

        private void MarkMatchedPieces(HashSet<int> getPieceEntitiesAtCoordinates)
        {
            foreach (var pieceEntity in getPieceEntitiesAtCoordinates.Where(pieceEntity => !_isMatchPool.Value.Has(pieceEntity)))
            {
                _isMatchPool.Value.Add(pieceEntity) = new IsMatchComponent();
            }
        }
    }
}