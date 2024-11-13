using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Grid.Moving
{
    public class MovePieceToTileSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceViewLinkComponent, PieceTileLinkComponent, MoveToTileComponent>> _moveToTileFilter;
        
        private EcsPoolInject<TileViewLinkComponent> _tileViewLinkPool;
        private EcsPoolInject<MoveCompleteCommand> _moveCompletePool;
        private EcsPoolInject<IsFallingComponent> _isFallingPool;

        private EcsCustomInject<MatchingService> _matchService;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _moveToTileFilter.Value)
            {
                var targetTileEntity = _moveToTileFilter.Pools.Inc2.Get(entity).LinkedEntity;
                var pieceView = _moveToTileFilter.Pools.Inc1.Get(entity).View;
                
                MoveView(targetTileEntity, pieceView);
                
                //TO DOTween

                ConcludeMove(entity);
            }
        }

        private void ConcludeMove(int entity)
        {
            if(!_moveCompletePool.Value.Has(entity))
            {
                _moveCompletePool.Value.Add(entity) = new MoveCompleteCommand();
            }
            if (_isFallingPool.Value.Has(entity))
            {
                _isFallingPool.Value.Del(entity);
            }
            _moveToTileFilter.Pools.Inc3.Del(entity);

            _matchService.Value.OnBoardUpdated();
        }

        private void MoveView(int targetTileEntity, PieceEntityView pieceView)
        {
            var tileViewLinkComponent = _tileViewLinkPool.Value.Get(targetTileEntity);
            pieceView.transform.SetParent(tileViewLinkComponent.View.PieceAnchor);
            pieceView.RectTransform.anchoredPosition = Vector2.zero;
        }
    }
}