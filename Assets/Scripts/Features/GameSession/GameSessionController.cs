using Initialization.ECS;
using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Time;
using Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scripts.Features.GameSession
{
    public class GameSessionController : AbstractController
    {
        [Inject] private GameSessionModel _gameSessionModel;
        
        [Inject] private EcsWorld _world;
        
        [Inject] private RulesConfig _rulesConfig;
        
        [Inject] private ECSInitializer _ecsInitializer;
        
        [Inject] private GridService _gridService;
        
        private int _gameTimerEntity;
        private int _gameSessionEntity;
        
        [Inject]
        public void Initialize()
        {
            _gameSessionModel.IsGameRunning
                .Where(hasTime => !hasTime)
                .Skip(1)
                .Subscribe(_ => OnGameOver())
                .AddTo(Disposer);
            
            _gameTimerEntity = _world.NewEntity();
            _world.GetPool<GameTimerComponent>().Add(_gameTimerEntity);
            
            _gameSessionEntity = _world.NewEntity();
            _world.GetPool<GameSessionComponent>().Add(_gameSessionEntity);
        }

        public void StartGame()
        {
            _gridService.CreateTiles();
            
            _gameSessionModel.ToggleEcsSystems(true);
            
            _world.GetPool<GameInProgressComponent>().Add(_gameTimerEntity);
            
            var defaultSeconds = _rulesConfig.GameDurationSeconds;
            ref var gameExpireComponent = ref _world.GetPool<ExpireComponent>().GetOrAddComponent(_gameTimerEntity);
            gameExpireComponent.RemainingSeconds = defaultSeconds;
        }

        private void OnGameOver()
        {
            _world.GetPool<GameOverCommand>().Add(_gameSessionEntity);
            //Toggle systems off?
            //TODO ditch all pieces and show score
        }
    }
}