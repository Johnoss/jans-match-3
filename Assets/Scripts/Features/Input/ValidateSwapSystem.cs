using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using Scripts.Features.Time;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Input
{
    public class ValidateSwapSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<SwapPieceComponent, PieceComponent, MoveCompleteComponent>> _completedSwapFilter;
        
        private EcsPoolInject<SwapPieceComponent> _swapPiecePool;
        private EcsPoolInject<InvalidSwapComponent> _invalidSwapPool;
        private EcsPoolInject<IsMatchComponent> _isMatchPool;
        private EcsPoolInject<IsTweeningComponent> _isTweeningComponent;
        private EcsPoolInject<PieceViewLinkComponent> _pieceViewLinkPool;
        
        private EcsCustomInject<TweenConfig> _tweenConfig;
        
        public void Run(EcsSystems systems)
        {
            ValidateCompletedSwaps();
        }

        private void ValidateCompletedSwaps()
        {
            foreach (var entity in _completedSwapFilter.Value)
            {
                var swapComponent = _swapPiecePool.Value.Get(entity);
                var targetPieceEntity = swapComponent.TargetPieceEntity;
                
                if (_invalidSwapPool.Value.Has(entity) || _isMatchPool.Value.Has(targetPieceEntity) || _isMatchPool.Value.Has(entity))
                {
                    _swapPiecePool.Value.Del(entity);
                    _invalidSwapPool.Value.DeleteComponent(entity);
                }
                else
                {
                    SetupInvalidMove(entity);
                }
            }
        }

        private void SetupInvalidMove(int entity)
        {
            ref var swapPieceComponent = ref _swapPiecePool.Value.Get(entity);
            (swapPieceComponent.SourceTileEntity, swapPieceComponent.TargetTileEntity) = (swapPieceComponent.TargetTileEntity, swapPieceComponent.SourceTileEntity);
            _invalidSwapPool.Value.AddOrSkip(entity);
            
            SetupInvalidMoveTween(entity);
        }

        private void SetupInvalidMoveTween(int entity)
        {
            if (!_pieceViewLinkPool.Value.Has(entity))
            {
                Debug.LogError($"PieceViewLinkComponent not found for entity {entity}");
                return;
            }
            
            var viewLinkComponent = _pieceViewLinkPool.Value.Get(entity);

            var tweenSetting = _tweenConfig.Value.InvalidSwapTweenSetting;
            var durationSeconds = tweenSetting.GetTweenDuration();
            
            viewLinkComponent.View.StartInvalidMoveTween(tweenSetting);

            ref var isTweeningComponent = ref _isTweeningComponent.Value.GetOrAddComponent(entity);
            isTweeningComponent.RemainingSeconds = durationSeconds;
        }
    }
}