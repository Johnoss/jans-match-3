using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Spawning
{
    public class SpawnPieceSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilterInject<Inc<SpawnPieceCommand>> _spawnTargetsFilter;
        
        private EcsCustomInject<PieceService> _pieceService;
        private EcsCustomInject<GridService> _gridService;
        private EcsCustomInject<MatchingService> _matchingService;
        
        private EcsCustomInject<GridConfig> _gridConfig;
        private EcsCustomInject<EcsWorld> _world;

        private int?[] _columnEmptyTilesCache;
        private int?[] _columnSpawnHeightsCache;
        
        public void Init(EcsSystems systems)
        {
            _columnEmptyTilesCache = new int?[_gridConfig.Value.GridResolution.x];
            _columnSpawnHeightsCache = new int?[_gridConfig.Value.GridResolution.x];
        }

        public void Run(EcsSystems systems)
        {
            ClearColumnsCache();
            foreach (var spawnPieceEntity in _spawnTargetsFilter.Value)
            {
                var spawnPieceCommand = _spawnTargetsFilter.Pools.Inc1.Get(spawnPieceEntity);
                
                UpdateColumnCache(spawnPieceCommand, out var spawnColumn, out var spawnRow);
                
                var spawnCoordinates = new Vector2Int(spawnColumn, spawnRow);
                
                _columnEmptyTilesCache[spawnColumn]--;
                
                _pieceService.Value.CreateRandomPieceEntity(spawnCoordinates, _columnSpawnHeightsCache[spawnColumn]!.Value, spawnPieceCommand.ForbidMatches);
                
                _matchingService.Value.SetBoardDirty();
                
                _world.Value.DelEntity(spawnPieceEntity);
            }
        }

        private void UpdateColumnCache(SpawnPieceCommand spawnPieceCommand, out int spawnColumn, out int spawnRow)
        {
            spawnColumn = spawnPieceCommand.Column;
            var emptyTilesCount = _gridService.Value.GetEmptyTilesCount(spawnColumn);
            _columnEmptyTilesCache[spawnColumn] ??= emptyTilesCount;
            _columnSpawnHeightsCache[spawnColumn] ??= emptyTilesCount;
            spawnRow = _gridConfig.Value.GridResolution.y - _columnEmptyTilesCache[spawnColumn].Value;
        }

        private void ClearColumnsCache()
        {
            for (var i = 0; i < _gridConfig.Value.GridResolution.x; i++)
            {
                _columnEmptyTilesCache[i] = null;
                _columnSpawnHeightsCache[i] = null;
            }
        }
    }
}