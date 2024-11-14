using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MVC;
using Scripts.Features.Grid;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Spawning
{
    public class DestroyEntitySystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ViewComponent, DestroyEntityCommand>> _viewEntitiesFilter;
        private EcsFilterInject<Inc<DestroyEntityCommand>, Exc<ViewComponent>> _destroyEntityFilter;
        private EcsFilterInject<Inc<DestroyEntityCommand, PieceTileLinkComponent, PieceComponent>> _pieceTileLinkFilter;
        
        private EcsPoolInject<PoolableViewComponent> _poolableViewComponentPool;
        
        private EcsCustomInject<EcsWorld> _world;
        private EcsCustomInject<GridService> _gridService;
        
        public void Run(EcsSystems systems)
        {
            UnlinkPiecesFromTiles();
            DestroyEntitiesWithView();
            DestroyViewlessEntities();
        }

        private void UnlinkPiecesFromTiles()
        {
            foreach (var pieceEntity in _pieceTileLinkFilter.Value)
            {
                _gridService.Value.UnlinkPieceFromTile(pieceEntity);
            }
        }

        private void DestroyEntitiesWithView()
        {
            foreach (var entity in _viewEntitiesFilter.Value)
            {
                var viewComponent = _viewEntitiesFilter.Pools.Inc1.Get(entity);
                if (_poolableViewComponentPool.Value.Has(entity))
                {
                    ReturnViewToPool(entity);
                }
                else
                {
                    DestroyView(viewComponent.View);
                }
                _world.Value.DelEntity(entity);
            }
        }

        private void ReturnViewToPool(int entity)
        {
            var poolableViewComponent = _poolableViewComponentPool.Value.Get(entity);

            var view = poolableViewComponent.PoolableEntityView;
            view.ReturnToPool();
        }

        private static void DestroyView(AbstractView view)
        {
            Object.Destroy(view.gameObject);
        }

        private void DestroyViewlessEntities()
        {
            foreach (var entity in _destroyEntityFilter.Value)
            {
                _world.Value.DelEntity(entity);
            }
        }
    }
}