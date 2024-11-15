using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
using Scripts.Features.Tweening;

namespace Scripts.Features.Grid.Matching
{
    public class CollectMatchesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceComponent, IsMatchComponent, PieceTileLinkComponent>> _isMatchFilter;

        private EcsPoolInject<DestroyEntityCommand> _destroyEntityCommandPool;
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;

        private EcsCustomInject<GridService> _gridService;
        
        private EcsCustomInject<TweenConfig> _tweenConfig;
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _isMatchFilter.Value)
            {
                _destroyEntityCommandPool.Value.Add(pieceEntity) = new DestroyEntityCommand();
                
                //TODO score and celebration
            }
        }
    }
}