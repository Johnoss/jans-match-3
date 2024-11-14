using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Input
{
    public class ValidateSwapSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<SwapPieceComponent, PieceComponent, MoveCompleteComponent>> _invalidSwapFilter;
        
        private EcsPoolInject<SwapPieceComponent> _swapPiecePool;
        private EcsPoolInject<IsMatchComponent> _isMatchPool;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _invalidSwapFilter.Value)
            {
                var swapComponent = _swapPiecePool.Value.Get(entity);
                var targetPieceEntity = swapComponent.TargetPieceEntity;
                
                if (_isMatchPool.Value.Has(targetPieceEntity) || _isMatchPool.Value.Has(entity))
                {
                    _swapPiecePool.Value.Del(entity);
                }
                else
                {
                    RevertInvalidMove(entity);
                }
            }
        }
        

        private void RevertInvalidMove(int entity)
        {
            if (!_swapPiecePool.Value.Has(entity))
            {
                Debug.LogError("Trying to revert invalid move, but swap piece component is missing");
                return;
            }
            
            if(_swapPiecePool.Value.Get(entity).IsReverting)
            {
                _swapPiecePool.Value.Del(entity);
                return;
            }
            
            ref var swapPieceComponent = ref _swapPiecePool.Value.Get(entity);
            
            (swapPieceComponent.SourceTileEntity, swapPieceComponent.TargetTileEntity) = (swapPieceComponent.TargetTileEntity, swapPieceComponent.SourceTileEntity);
            swapPieceComponent.IsReverting = true;
        }
    }
}