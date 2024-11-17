using MVC;
using Scripts.Features.Grid.Matching;
using Scripts.Utils;
using UniRx;
using Zenject;

namespace Scripts.Features.GameSession
{
    public class GameSessionModel : AbstractModel
    {
        private readonly ReactiveProperty<float> _remainingSeconds = new(0);
        private readonly ReactiveProperty<float> _score = new(0);
        private readonly ReactiveProperty<bool> _isIsTimerPaused = new(false);
        private readonly ReactiveProperty<int> _currentCombo = new(0);
        private readonly ReactiveProperty<bool> _ecsSystemsOnline = new(false);
        private readonly ReactiveProperty<float> _highScore = new();
        private readonly ReactiveProperty<bool> _isHighScoreBeaten = new();
        
        public IReadOnlyReactiveProperty<bool> IsGameRunning => RemainingSeconds.Select(seconds => seconds > 0).ToReadOnlyReactiveProperty();
        public IReadOnlyReactiveProperty<float> Score => _score;
        public IReadOnlyReactiveProperty<float> RemainingSeconds => _remainingSeconds;
        public IReadOnlyReactiveProperty<bool> IsTimerPaused => _isIsTimerPaused;
        public IReadOnlyReactiveProperty<int> CurrentCombo => _currentCombo;
        public IReadOnlyReactiveProperty<bool> EcsSystemsOnline => _ecsSystemsOnline;
        public IReadOnlyReactiveProperty<float> HighScore => _highScore;
        private IReadOnlyReactiveProperty<Unit> OnGameOver => IsGameRunning.Where(isRunning => !isRunning).Select(_ => Unit.Default).ToReadOnlyReactiveProperty();
        
        public IReadOnlyReactiveProperty<bool> IsHighScoreBeaten => _isHighScoreBeaten;

        [Inject] private RulesConfig _rulesConfig;

        public void SetScore(float score)
        {
            _score.Value = score;
        }

        public void IncrementScore(float deltaScore)
        {
            var newScore = _score.Value + deltaScore;
            SetScore(newScore);
        }

        public void IncrementCombo(int deltaCombo = 1)
        {
            var newCombo = _currentCombo.Value + deltaCombo;
            SetCurrentCombo(newCombo);
        }

        public void ResetCombo()
        {
            SetCurrentCombo(0);
        }

        public void SetupHighScore()
        {
            _highScore.Value = PlayerPrefsUtils.GetFloat(_rulesConfig.HighScoreKey);
        }

        public void SetFinalScore()
        {
            var score = _score.Value;
            _isHighScoreBeaten.SetValueAndForceNotify(score > _highScore.Value);
            if (!_isHighScoreBeaten.Value)
            {
                return;
            }
            
            PlayerPrefsUtils.SetFloat(_rulesConfig.HighScoreKey, score);
            _highScore.Value = score;
        }

        public void SetTimerPaused(bool isBusy)
        {
            _isIsTimerPaused.Value = isBusy;
        }

        public void SetRemainingSeconds(float seconds)
        {
            _remainingSeconds.Value = seconds;
        }

        public void ToggleEcsSystems(bool isActive)
        {
            _ecsSystemsOnline.Value = isActive;
        }

        private void SetCurrentCombo(int combo)
        {
            _currentCombo.Value = combo;
        }
    }
}