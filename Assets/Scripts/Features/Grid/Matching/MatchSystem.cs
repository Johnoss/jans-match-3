using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Grid.Matching
{
    public class MatchSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MoveCompleteComponent, PieceComponent, PieceTileLinkComponent>> _moveCompleteFilter;
        private EcsCustomInject<MatchingService> _matchingService;
        
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
                
                var legalMatches = _matchingService.Value.FindMatches(matchingNeighbours);
                Debug.Log($"Legal matches for tile {tileEntity}: {legalMatches.Count}");
            }
            
        }
    }
}