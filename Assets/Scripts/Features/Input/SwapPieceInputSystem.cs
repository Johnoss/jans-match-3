using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using Scripts.Features.Time;
using Scripts.Utils;

namespace Scripts.Features.Input
{
    public class SwapPieceInputSystem : IEcsRunSystem, IEcsInitSystem
    {
        private const int MAX_INTERACTED_TILES = 2;
        
        private EcsFilterInject<Inc<SelectedPieceComponent>> _selectedPieceFilter;
        
        private readonly EcsPoolInject<SwapPieceComponent> _swapPiecePool;
        private readonly EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private readonly EcsPoolInject<BlockInputComponent> _blockInputPool;
        
        private readonly EcsCustomInject<EcsWorld> _world;
        private readonly EcsCustomInject<GridService> _gridService;
        private readonly EcsCustomInject<InputService> _inputService;
        
        
        private readonly List<int> _selectedPieceEntities = new();

        private IEcsPool[] _interactionBlacklistPool;
        
        public void Init(EcsSystems systems)
        {
            _interactionBlacklistPool = new IEcsPool[]
            {
                _world.Value.GetPool<SwapPieceComponent>(),
                _world.Value.GetPool<IsTweeningComponent>(),
                _world.Value.GetPool<IsMovingComponent>(),
                _world.Value.GetPool<IsMatchComponent>(),
            };
        }

        public void Run(EcsSystems systems)
        {
            var selectedPiecesCount = _selectedPieceFilter.Value.GetEntitiesCount();
            if (selectedPiecesCount < MAX_INTERACTED_TILES)
            {
                return;
            }
            
            _selectedPieceEntities.Clear();

            foreach (var pieceEntity in _selectedPieceFilter.Value)
            {
                if(pieceEntity.IsInAnyPool(_interactionBlacklistPool))
                {
                    continue;
                }
                
                _selectedPieceEntities.Add(pieceEntity);
                
                if(_selectedPieceEntities.Count == MAX_INTERACTED_TILES)
                {
                    SetupSwapPieces();
                }
            }
        }

        private void SetupSwapPieces()
        {
            var originPieceEntity = _selectedPieceEntities[0];
            var targetPieceEntity = _selectedPieceEntities[1];

            if (!_pieceTileLinkPool.Value.HasAllOf(originPieceEntity, targetPieceEntity))
            {
                return;
            }
            
            var originTileEntity = _pieceTileLinkPool.Value.Get(originPieceEntity).LinkedEntity;
            var targetTileEntity = _pieceTileLinkPool.Value.Get(targetPieceEntity).LinkedEntity;
            
            
            if (!_gridService.Value.AreNeighbours(originTileEntity, targetTileEntity))
            {
                return;
            }

            _swapPiecePool.Value.Add(originPieceEntity) = new SwapPieceComponent
            {
                SourceTileEntity = originTileEntity,
                TargetTileEntity = targetTileEntity,
                TargetPieceEntity = targetPieceEntity,
            };

            _swapPiecePool.Value.Add(targetPieceEntity) = new SwapPieceComponent
            {
                SourceTileEntity = targetTileEntity,
                TargetTileEntity = originTileEntity,
                TargetPieceEntity = originPieceEntity,
            };

            ClearInteraction();
        }

        private void ClearInteraction()
        {
            _blockInputPool.Value.AddOrSkip(_inputService.Value.InputEntity);
            foreach (var entity in _selectedPieceFilter.Value)
            {
                _selectedPieceFilter.Pools.Inc1.Del(entity);
            }
        }
    }
}