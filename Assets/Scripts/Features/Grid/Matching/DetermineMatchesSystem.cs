using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;

namespace Scripts.Features.Grid.Matching
{
    public class DetermineMatchesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MatchValidatorComponent>> _matchValidatorFilter;
        private EcsFilterInject<Inc<MoveCompleteComponent>> _moveCompleteFilter;
        
        private EcsFilterInject<Inc<IsMovingComponent>> _ongoingMoveFilter;
        
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private EcsPoolInject<IsMatchComponent> _isMatchPool;
        
        private EcsCustomInject<MatchingService> _matchingService;
        private EcsCustomInject<GridService> _gridService;

        public void Run(EcsSystems systems)
        {
            foreach (var matchValidatorEntity in _matchValidatorFilter.Value)
            {
                ref var matchValidatorComponent = ref _matchValidatorFilter.Pools.Inc1.Get(matchValidatorEntity);
                foreach (var pieceEntity in _moveCompleteFilter.Value)
                {
                    matchValidatorComponent.PendingMatchPieceEntities.Add(pieceEntity);
                }
                
                if (_ongoingMoveFilter.Value.GetEntitiesCount() > 0)
                {
                    return;
                }

                DetermineMatches(matchValidatorComponent);
                
                matchValidatorComponent.PendingMatchPieceEntities.Clear();
            }
        }

        private void DetermineMatches(MatchValidatorComponent matchValidatorComponent)
        {
            foreach (var pieceEntity in matchValidatorComponent.PendingMatchPieceEntities)
            {
                var pieceTileLinkComponent = _pieceTileLinkPool.Value.Get(pieceEntity);
                
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