using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
using Scripts.Features.Time;
using Scripts.Features.Tweening;
using Scripts.Utils;
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
        [Inject] private MoveService _moveService;
        [Inject] private InputService _inputService;
        
        [Inject] private RulesConfig _rulesConfig;
        [Inject] private TweenConfig _tweenConfig;
        [Inject] private GridConfig _gridConfig;
        
        [Inject] private GridView _gridView;
        
        private EcsSystems _systems;
        
        [Inject]
        private void Init()
        {
            _systems = new EcsSystems(_world)
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(null, true, entityNameFormat: "D"))
#endif
                .Add(new ExpireSystem())
                .Add(new GridInputSystem(), new SwapPieceInputSystem())
                .Add(new DetermineFallSystem(), new SetupFallSystem(), new ExecuteFallSystem())
                .Add(new SetupMoveSystem(), new StartMoveSystem(), new CompleteMoveSystem())
                .Add(new SpawnPieceSystem())
                .Add(new DetermineMatchesSystem(), new SetupCollectMatchesSystem(), new CompleteCollectMatchesSystem())
                .Add(new SwapPiecesSystem(), new ValidateSwapSystem())

                .Add(new DestroyEntitySystem())

                .Add(new ValidatePossibleMovesSystem())
                .Add(new ShuffleBoardSystem())

                .DelHere<MoveCompleteComponent>()
                .DelHere<FallPieceCommand>()
                .DelHere<DestroyEntityCommand>()
                .DelHere<FallOccupantComponent>()
                .DelHere<IsMatchComponent>();
                
                _systems
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
                _moveService,
                _inputService,
                
                _rulesConfig,
                _tweenConfig,
                _gridConfig,
                
                _gridView,
            };
        }
        
        private void RunSystems()
        {
            _systems.Run();
        }
    }
}