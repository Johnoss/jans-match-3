using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;

namespace Scripts.Features.Grid.Moving
{
    public class DetermineFallSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<TileComponent>, Exc<PieceTileLinkComponent, SpawnTargetComponent>> _emptyTilesFilter;

        private EcsCustomInject<GridService> _gridService;
        private EcsPoolInject<MoveToTileComponent> _moveToTilePool;
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private EcsPoolInject<FallOccupantComponent> _fallOccupantPool;
        
        public void Run(EcsSystems systems)
        {
            foreach (var tileEntity in _emptyTilesFilter.Value)
            {
                var tileComponent = _emptyTilesFilter.Pools.Inc1.Get(tileEntity);
                var tilesAbove = _gridService.Value.GetTilesAbove(tileComponent.Coordinates);
                
                MarkTilesForFall(tilesAbove);
            }
        }

        private void MarkTilesForFall(HashSet<int> tilesAbove)
        {
            foreach (var pieceEntity in tilesAbove.Where(tileEntity => !_fallOccupantPool.Value.Has(tileEntity)))
            {
                _fallOccupantPool.Value.Add(pieceEntity) = new FallOccupantComponent();
            }
        }
    }
}