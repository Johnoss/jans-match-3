using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Piece;

namespace Scripts.Features.Spawning
{
    public class FillEmptyTilesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<TileComponent>, Exc<SpawnTargetComponent, PieceTileLinkComponent>> _spawnTargetsFilter;

        private EcsPoolInject<SpawnTargetComponent> _spawnTargetComponent;
        
        public void Run(EcsSystems systems)
        {
            foreach (var spawnTileEntity in _spawnTargetsFilter.Value)
            {
                _spawnTargetComponent.Value.Add(spawnTileEntity) = new SpawnTargetComponent();
            }
        }
    }
}