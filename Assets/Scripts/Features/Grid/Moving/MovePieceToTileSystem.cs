using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Grid.Moving
{
    public class MovePieceToTileSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceViewLinkComponent, PieceTileLinkComponent, MoveToTileComponent>> _moveToTileFilter;
        
        private EcsPoolInject<TileViewLinkComponent> _tileViewLinkPool;
        private EcsPoolInject<MoveCompleteComponent> _moveCompletePool;
        private EcsPoolInject<IsFallingComponent> _isFallingPool;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _moveToTileFilter.Value)
            {
                //TODO move piece to tile
                var targetTileEntity = _moveToTileFilter.Pools.Inc2.Get(entity).LinkedEntity;
                var pieceView = _moveToTileFilter.Pools.Inc1.Get(entity).View;
                
                MoveView(targetTileEntity, pieceView);
                
                //TO DOTween

                ConcludeMove(entity);
            }
        }

        private void ConcludeMove(int entity)
        {
            _moveCompletePool.Value.Add(entity) = new MoveCompleteComponent();
            if (_isFallingPool.Value.Has(entity))
            {
                _isFallingPool.Value.Del(entity);
            }
            _moveToTileFilter.Pools.Inc3.Del(entity);
        }

        private void MoveView(int targetTileEntity, PieceEntityView pieceView)
        {
            var tileViewLinkComponent = _tileViewLinkPool.Value.Get(targetTileEntity);
            pieceView.transform.SetParent(tileViewLinkComponent.View.PieceAnchor);
            pieceView.RectTransform.anchoredPosition = Vector2.zero;
        }
    }
}