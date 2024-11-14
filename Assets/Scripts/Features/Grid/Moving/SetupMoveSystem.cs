using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;

namespace Scripts.Features.Grid.Moving
{
    public class SetupMoveSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceTileLinkComponent, StartMovePieceCommand>> _startMoveFilter;
        
        private EcsCustomInject<GridService> _gridService;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _startMoveFilter.Value)
            {
                _gridService.Value.UnlinkPieceFromTile(entity);
            }
        }
    }
}