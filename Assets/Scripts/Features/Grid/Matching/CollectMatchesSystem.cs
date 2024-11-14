using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
using Scripts.Features.Time;

namespace Scripts.Features.Grid.Matching
{
    public class CollectMatchesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceComponent, IsMatchComponent, PieceTileLinkComponent>, Exc<IsMovingComponent, ExpireComponent>> _isMatchFilter;

        private EcsPoolInject<DestroyEntityCommand> _destroyEntityCommandPool;
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;

        private EcsCustomInject<GridService> _gridService;
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _isMatchFilter.Value)
            {
                _destroyEntityCommandPool.Value.Add(pieceEntity) = new DestroyEntityCommand();
                
                _gridService.Value.UnlinkPieceFromTile(pieceEntity);


                //TODO score and celebration
            }
        }
    }
}