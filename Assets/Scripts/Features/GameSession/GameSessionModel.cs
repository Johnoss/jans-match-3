using MVC;
using Scripts.Features.Grid.Matching;
using UniRx;
using Zenject;

namespace Scripts.Features.GameSession
{
    public class GameSessionModel : AbstractModel
    {
        [Inject] private RulesConfig _rulesConfig;
        
        public IReadOnlyReactiveProperty<bool> IsGameRunning => RemainingSeconds.Select(seconds => seconds > 0).ToReadOnlyReactiveProperty();
        
        private readonly ReactiveProperty<float> _score = new(0);
        public IReadOnlyReactiveProperty<float> Score => _score;
        
        private readonly ReactiveProperty<float> _remainingSeconds = new(0);
        public IReadOnlyReactiveProperty<float> RemainingSeconds => _remainingSeconds;
        
        public void SetScore(float score)
        {
            _score.Value = score;
        }
        
        public void SetRemainingSeconds(float seconds)
        {
            _remainingSeconds.Value = seconds;
        }
    }
}