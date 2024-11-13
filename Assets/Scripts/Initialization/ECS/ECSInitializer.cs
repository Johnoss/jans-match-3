using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
using UniRx;
using Zenject;

namespace Initialization.ECS
{
    public class ECSInitializer
    {
        [Inject] private EcsWorld _world;
        
        [Inject] private GridService _gridService;
        [Inject] private MatchingService _matchingService;
        [Inject] private PieceService _pieceService;
        
        [Inject] private RulesConfig _rulesConfig;
        
        private EcsSystems _systems;
        
        [Inject]
        private void Init()
        {
            _systems = new EcsSystems(_world);

            _systems
                .Add(new SpawnPieceSystem())
                .Add(new InputSystem())
                .Add(new SwapPiecesSystem())
                .Add(new DetermineFallSystem())
                .Add(new SetupFallSystem())
                .Add(new ExecuteFallSystem())
                .Add(new FillEmptyTilesSystem())
                .Add(new MovePieceToTileSystem())
                .Add(new DetermineMatchesSystem())
                .Add(new ValidateSwapSystem())
                .Add(new DestroyEntitySystem())
                .Add(new CollectMatchesSystem())
                .Add(new ValidatePossibleMovesSystem())
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(null, true, entityNameFormat: "D"))
#endif
                .Inject(GetInjectables())
                .Init();
            
            Observable.EveryUpdate().Subscribe(_ => RunSystems());
        }

        //LeoECS injection doesn't pair well with Zenject, so we have to inject the dependencies manually
        private object[] GetInjectables()
        {
            return new object[]
            {
                _world,
                _gridService,
                _matchingService,
                _pieceService,
                _rulesConfig,
            };
        }
        
        private void RunSystems()
        {
            _systems.Run();
        }
    }
}