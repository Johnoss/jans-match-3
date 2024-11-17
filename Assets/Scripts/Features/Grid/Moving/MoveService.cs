using Leopotam.EcsLite;
using Scripts.Utils;
using Zenject;

namespace Scripts.Features.Grid.Moving
{
    public class MoveService
    {
        [Inject] private EcsWorld _world;
        
        [Inject] private GridService _gridService;
        
        public void SetupMovePieceCommand(int pieceEntity, int targetTile, MoveType moveType)
        {
            ref var startMoveComponent = ref _world.GetPool<StartMovePieceCommand>().GetOrAddComponent(pieceEntity);
            startMoveComponent.TargetTileEntity = targetTile;
            startMoveComponent.MoveType = moveType;
        }
    }
}