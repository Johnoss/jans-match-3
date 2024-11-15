using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Tweening;
using Scripts.Features.Spawning;
using Scripts.Features.Time;
using Scripts.Utils;

namespace Scripts.Features.Grid.Matching
{
    public class ValidatePossibleMovesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PossibleMovesValidatorComponent>, Exc<FoundPossibleMovesComponent, NoPossibleMovesComponent, ExpireComponent>> _validatorFilter;
        private EcsFilterInject<Inc<MoveCompleteComponent>> _moveCompleteFilter;
        
        private EcsFilterInject<Inc<IsMovingComponent>> _moveToTileFilter;
        
        private EcsPoolInject<NoPossibleMovesComponent> _noPossibleMovesPool;
        private EcsPoolInject<FoundPossibleMovesComponent> _foundPossibleMovesPool;
        private EcsPoolInject<ExpireComponent> _expirePool;
        
        private EcsCustomInject<MatchingService> _matchingService;
        private EcsCustomInject<GridService> _gridService;
        private EcsCustomInject<RulesConfig> _rulesConfig;
        
        public void Run(EcsSystems systems)
        {
            if (ShouldPauseValidation())
            {
                return;
            }
            
            foreach (var checkEntity in _validatorFilter.Value)
            {
                RunValidator(checkEntity);
            }
        }

        private void RunValidator(int checkEntity)
        {
            ref var checkComponent = ref _validatorFilter.Pools.Inc1.Get(checkEntity);

            var maxIterations = _rulesConfig.Value.MaxIterationsPerFrame;
            for (var i = 0; i < maxIterations; i++)
            {
                if (TryFindPossibleMatch(ref checkComponent))
                {
                    _foundPossibleMovesPool.Value.Add(checkEntity) = new FoundPossibleMovesComponent();
                    return;
                }

                if (!_gridService.Value.TryGetNextCoordinates(checkComponent.CurrentIterationCoordinates, out var nextCoordinates))
                {
                    ref var expireComponent = ref _expirePool.Value.GetOrAddComponent(checkEntity);
                    expireComponent.RemainingSeconds = _rulesConfig.Value.ShuffleDelaySeconds;
                    _noPossibleMovesPool.Value.Add(checkEntity) = new NoPossibleMovesComponent();
                    return;
                }

                checkComponent.CurrentIterationCoordinates = nextCoordinates;
            }
        }

        private bool TryFindPossibleMatch(ref PossibleMovesValidatorComponent checkComponent)
        {
            var currentCoordinates = checkComponent.CurrentIterationCoordinates;

            return _matchingService.Value.HasPossibleMatch(currentCoordinates);
        }
        
        private bool ShouldPauseValidation()
        {
            return _moveToTileFilter.Value.GetEntitiesCount() > 0;
        }
    }
}