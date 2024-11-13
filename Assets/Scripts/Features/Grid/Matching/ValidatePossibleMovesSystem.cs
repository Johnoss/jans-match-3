using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Spawning;
using UnityEngine;

namespace Scripts.Features.Grid.Matching
{
    public class ValidatePossibleMovesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PossibleMovesValidatorComponent>, Exc<FoundPossibleMovesComponent, NoPossibleMovesComponent>> _validatorFilter;
        private EcsFilterInject<Inc<MoveCompleteCommand>> _moveCompleteFilter;
        
        private EcsFilterInject<Inc<SpawnTargetComponent>> _spawnTargetsFilter;
        private EcsFilterInject<Inc<MoveToTileComponent>> _moveToTileFilter;
        private EcsFilterInject<Inc<FoundPossibleMovesComponent>> _foundPossibleMovesFilter;
        private EcsPoolInject<NoPossibleMovesComponent> _noPossibleMovesPool;
        
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
                ref var checkComponent = ref _validatorFilter.Pools.Inc1.Get(checkEntity);

                var maxIterations = _rulesConfig.Value.MaxIterationsPerFrame;
                for (var i = 0; i < maxIterations; i++)
                {
                    if (!TryFindPossibleMatch(ref checkComponent, checkEntity))
                    {
                        continue;
                    }
                    
                    _foundPossibleMovesFilter.Pools.Inc1.Add(checkEntity);
                    return;
                }
            }
        }
        
        private bool TryFindPossibleMatch(ref PossibleMovesValidatorComponent checkComponent, int entity)
        {
            var currentCoordinates = checkComponent.CurrentIterationCoordinates;

            if (_matchingService.Value.HasPossibleMatch(currentCoordinates))
            {
                return true;
            }

            if (!_gridService.Value.TryGetNextCoordinates(currentCoordinates, out var nextCoordinates))
            {
                _noPossibleMovesPool.Value.Add(entity) = new NoPossibleMovesComponent();
                return false;
            }
                    
            checkComponent.CurrentIterationCoordinates = nextCoordinates;
            return false;
        }
        private bool ShouldPauseValidation()
        {
            if (_spawnTargetsFilter.Value.GetEntitiesCount() > 0)
            {
                return true;
            }
            
            return _moveToTileFilter.Value.GetEntitiesCount() > 0;
        }
    }
}