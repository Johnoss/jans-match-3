using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using Scripts.Features.Time;
using Scripts.Features.Tweening;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Input
{
    public class ValidateSwapSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<SwapPieceComponent, PieceComponent, MoveCompleteComponent>> _completedSwapFilter;
        private EcsFilterInject<Inc<SwapPieceComponent, PieceComponent, PendingMatchValidatorComponent>> _pendingMatchValidatorFilter;

        private EcsFilterInject<Inc<IsMovingComponent>> _isMovingFilter;
        
        private EcsPoolInject<SwapPieceComponent> _swapPiecePool;
        private EcsPoolInject<InvalidSwapComponent> _invalidSwapPool;
        private EcsPoolInject<IsMatchComponent> _isMatchPool;
        private EcsPoolInject<IsTweeningComponent> _isTweeningComponent;
        private EcsPoolInject<PieceViewLinkComponent> _pieceViewLinkPool;
        private EcsPoolInject<PendingMatchValidatorComponent> _pendingMatchValidatorPool;
        
        private EcsCustomInject<TweenConfig> _tweenConfig;
        
        public void Run(EcsSystems systems)
        {
            ValidateCompletedSwaps();
        }

        private void ValidateCompletedSwaps()
        {
            var isAnyPieceMoving = _isMovingFilter.Value.GetEntitiesCount() > 0;
            
            foreach (var sourcePieceEntity in _completedSwapFilter.Value)
            {
                var swapComponent = _swapPiecePool.Value.Get(sourcePieceEntity);
                var targetPieceEntity = swapComponent.TargetPieceEntity;
                _pendingMatchValidatorPool.Value.AddOrSkip(targetPieceEntity);
            }
            
            if (isAnyPieceMoving)
            {
                return;
            }
            
            foreach (var sourcePieceEntity in _pendingMatchValidatorFilter.Value)
            {
                var swapComponent = _swapPiecePool.Value.Get(sourcePieceEntity);
                var targetPieceEntity = swapComponent.TargetPieceEntity;
                ValidateSwap(sourcePieceEntity, targetPieceEntity);
                
                _pendingMatchValidatorPool.Value.Del(targetPieceEntity);
            }
        }

        private void ValidateSwap(int sourcePieceEntity, int targetPieceEntity)
        {
            if (_invalidSwapPool.Value.Has(sourcePieceEntity) || _isMatchPool.Value.Has(targetPieceEntity) || _isMatchPool.Value.Has(sourcePieceEntity))
            {
                _swapPiecePool.Value.Del(sourcePieceEntity);
                _invalidSwapPool.Value.DelOrSkip(sourcePieceEntity);
            }
            else
            {
                SetupInvalidMove(sourcePieceEntity);
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
            
            viewLinkComponent.View.StartInvalidMoveTween(tweenSetting, out var durationSeconds);

            ref var isTweeningComponent = ref _isTweeningComponent.Value.GetOrAddComponent(entity);
            isTweeningComponent.RemainingSeconds = durationSeconds;
        }
    }
}