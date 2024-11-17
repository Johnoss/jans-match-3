using MVC;
using Scripts.Features.Grid.Matching;
using Scripts.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scripts.Features.GameSession
{
    public class GameSessionModel : AbstractModel
    {
        private readonly ReactiveProperty<float> _remainingSeconds = new(0);
        private readonly ReactiveProperty<bool> _isIsTimerPaused = new(false);
        private readonly ReactiveProperty<int> _currentCombo = new(0);
        private readonly ReactiveProperty<float> _score = new(0);
        
        public IReadOnlyReactiveProperty<bool> IsGameRunning => RemainingSeconds.Select(seconds => seconds > 0).ToReadOnlyReactiveProperty();
        public IReadOnlyReactiveProperty<float> Score => _score;
        public IReadOnlyReactiveProperty<float> RemainingSeconds => _remainingSeconds;
        public IReadOnlyReactiveProperty<bool> IsTimerPaused => _isIsTimerPaused;
        public IReadOnlyReactiveProperty<int> CurrentCombo => _currentCombo;

        public float HighScore => PlayerPrefsUtils.GetFloat(_rulesConfig.HighScoreKey, 0);

        [Inject] private RulesConfig _rulesConfig;
        
        public void IncrementScore(float deltaScore)
        {
            _score.Value += deltaScore;
        }
        
        public void IncrementCombo(int deltaCombo)
        {
            var newCombo = _currentCombo.Value + deltaCombo;
            SetCurrentCombo(newCombo);
        }
        
        public void ResetCombo()
        {
            SetCurrentCombo(0);
        }

        public void SetFinalScore(float score, out bool isNewHighScore)
        {
            isNewHighScore = score > HighScore;
            if (isNewHighScore)
            {
                PlayerPrefsUtils.SetFloat(_rulesConfig.HighScoreKey, score);
            }
        }

        public void SetTimerPaused(bool isBusy)
        {
            _isIsTimerPaused.Value = isBusy;
        }

        public void SetRemainingSeconds(float seconds)
        {
            _remainingSeconds.Value = seconds;
        }

        public void SetCurrentCombo(int combo)
        {
            _currentCombo.Value = combo;
        }

        private void SetScore(float score)
        {
            _score.Value = score;
        }
    }
}