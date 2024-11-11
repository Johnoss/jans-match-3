using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;

namespace Scripts.Features.Grid.Matching
{
    public class CollectMatchesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceComponent, IsMatchComponent, PieceTileLinkComponent>> _isMatchFilter;

        private EcsPoolInject<DestroyEntityCommand> _destroyEntityCommandPool;
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _isMatchFilter.Value)
            {
                _isMatchFilter.Pools.Inc2.Del(pieceEntity);
                
                _destroyEntityCommandPool.Value.Add(pieceEntity) = new DestroyEntityCommand();
                
                UnlinkTileAndPiece(pieceEntity);


                //TODO score and celebration
            }
        }

        private void UnlinkTileAndPiece(int pieceEntity)
        {
            var tileEntity = _pieceTileLinkPool.Value.Get(pieceEntity).LinkedEntity;

            if (_pieceTileLinkPool.Value.Has(tileEntity))
            {
                _pieceTileLinkPool.Value.Del(tileEntity);
            }
            _pieceTileLinkPool.Value.Del(pieceEntity);
        }
    }
}