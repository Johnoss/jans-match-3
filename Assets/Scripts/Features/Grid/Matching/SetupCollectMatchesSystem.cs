using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using Scripts.Features.Time;
using Scripts.Features.Tweening;
using Scripts.Utils;

namespace Scripts.Features.Grid.Matching
{
    public class SetupCollectMatchesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceComponent, IsMatchComponent, PieceViewLinkComponent>> _isMatchFilter;

        private EcsPoolInject<CollectPieceComponent> _collectPiecePool; 
        private EcsPoolInject<IsTweeningComponent> _isTweeningPool; 

        private EcsCustomInject<TweenConfig> _tweenConfig;
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _isMatchFilter.Value)
            {
                 var view = _isMatchFilter.Pools.Inc3.Get(pieceEntity).View;
                 
                 view.StartCollectTween(_tweenConfig.Value.CollectTweenSetting, out var totalSeconds);
                 
                 ref var tweeningComponent = ref _isTweeningPool.Value.GetOrAddComponent(pieceEntity);
                 tweeningComponent.RemainingSeconds = totalSeconds;
                 
                 _collectPiecePool.Value.AddOrSkip(pieceEntity);
            }
        }
    }
}