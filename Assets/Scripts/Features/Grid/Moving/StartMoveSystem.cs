using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using Scripts.Features.Time;
using Scripts.Features.Tweening;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Grid.Moving
{
    public class StartMoveSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceComponent, PieceViewLinkComponent, StartMovePieceCommand>> _startMoveFilter;

        private EcsPoolInject<ExpireComponent> _expirePool;
        private EcsPoolInject<TileViewLinkComponent> _tileViewLinkPool;
        private EcsPoolInject<IsMovingComponent> _isMovingPool;
        private EcsPoolInject<IsTweeningComponent> _isTweeningPool;

        private EcsCustomInject<GridService> _gridService;
        private EcsCustomInject<MoveService> _moveService;
        private EcsCustomInject<TweenConfig> _tweenConfig;
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _startMoveFilter.Value)
            {
                var movePieceCommand = _startMoveFilter.Pools.Inc3.Get(pieceEntity);
                
                _gridService.Value.SetTilePieceLink(movePieceCommand.TargetTileEntity, pieceEntity);
                SetupViewAndTween(pieceEntity);
                
                _isMovingPool.Value.AddOrSkip(pieceEntity);
                
                _startMoveFilter.Pools.Inc3.Del(pieceEntity);
            }
        }

        private void SetupViewAndTween(int pieceEntity)
        {
            var pieceViewLinkComponent = _startMoveFilter.Pools.Inc2.Get(pieceEntity);
            var startMovePieceCommand = _startMoveFilter.Pools.Inc3.Get(pieceEntity);

            var targetAnchorPosition = _gridService.Value.GetTileAnchorPosition(startMovePieceCommand.TargetTileEntity);
            StartMoveTween(pieceViewLinkComponent.View, targetAnchorPosition, startMovePieceCommand.MoveType, out var tweenDurationSeconds);
            
            ref var tweeningComponent = ref _isTweeningPool.Value.GetOrAddComponent(pieceEntity);
            tweeningComponent.RemainingSeconds = tweenDurationSeconds;
        }

        private void StartMoveTween(PieceEntityView view, Vector2 target, MoveType moveType, out float tweenDurationSeconds)
        {
            var tweenSetting = moveType switch
            {
                MoveType.Swap => _tweenConfig.Value.SwapTweenSetting,
                MoveType.RevertSwap => _tweenConfig.Value.RevertTweenSetting,
                MoveType.Fall => _tweenConfig.Value.FallTweenSetting,
                _ => throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null)
            };
            view.StartMoveTween(tweenSetting, target, out tweenDurationSeconds);
        }
    }
}