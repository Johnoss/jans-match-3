using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using Scripts.Features.Time;
using Scripts.Utils;

namespace Scripts.Features.Grid.Moving
{
    public class CompleteMoveSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceViewLinkComponent, PieceTileLinkComponent, IsMovingComponent>, Exc<ExpireComponent, IsTweeningComponent>> _moveCompleteFilter;
        
        private EcsPoolInject<MoveCompleteComponent> _moveCompletePool;

        private EcsCustomInject<MatchingService> _matchService;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _moveCompleteFilter.Value)
            {
                ConcludeMove(entity);
                
                _moveCompleteFilter.Pools.Inc3.Del(entity);
            }
        }

        private void ConcludeMove(int entity)
        {
            _moveCompletePool.Value.AddOrSkip(entity);

            _matchService.Value.SetBoardDirty();
        }
    }
}