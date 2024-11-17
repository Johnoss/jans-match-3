using DG.Tweening;
using MVC;
using Scripts.Features.GameSession;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.UI
{
    public class GameUIView : AbstractView
    {
        private const string COMBO_TEXT_FORMAT = "x{0}";
        
        [Header("Startup Tweens")]
        [SerializeField] private DOTweenAnimation[] _startupTweens;

        [Header("Game Session")]
        [SerializeField] private GameObject[] _gameOnObjects;
        
        [Header("Timer")]
        [SerializeField] private Image _timerFillImage;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private GameObject[] _pauseIndicators;
        
        [Header("Combo")]
        [SerializeField] private TextMeshProUGUI _comboText;
        [SerializeField] private DOTweenAnimation _comboPunchTween;
        [SerializeField] private DOTweenAnimation _comboShakeTween;
        [SerializeField] private RectTransform _comboParent;
        
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private DOTweenAnimation _scorePunchTween;
        
        [Inject] private GameSessionModel _gameSessionModel;
        [Inject] private RulesConfig _rulesConfig;
        [Inject] private TweenConfig _tweenConfig;
        
        [Inject] private CompositeDisposable _disposer;

        [Inject]
        private void Initialize()
        {
            SetDisposer(_disposer);
            _gameSessionModel.IsGameRunning.Subscribe(OnGameRunningChanged).AddTo(_disposer);
            _gameSessionModel.RemainingSeconds.Subscribe(OnRemainingSecondsChanged).AddTo(_disposer);
            _gameSessionModel.IsTimerPaused.Subscribe(OnTimerPausedChanged).AddTo(_disposer);
            _gameSessionModel.CurrentCombo.Subscribe(OnComboChanged).AddTo(_disposer);
            _gameSessionModel.Score.Subscribe(OnScoreChanged).AddTo(_disposer);
            
            _timerText.gameObject.SetActive(_rulesConfig.ShowSeconds);
        }

        private void OnGameRunningChanged(bool isRunning)
        {
            foreach (var tween in _startupTweens)
            {
                if (isRunning)
                {
                    tween.DOPlay();
                }
                else
                {
                    tween.DORestart();
                }
            }
            
            foreach (var gameOnObject in _gameOnObjects)
            {
                gameOnObject.SetActive(isRunning);
            }
        }
        
        private void OnRemainingSecondsChanged(float seconds)
        {
            var fillAmount = seconds / _rulesConfig.GameDurationSeconds;
            _timerFillImage.fillAmount = fillAmount;

            if (_rulesConfig.ShowSeconds)
            {
                _timerText.text = $"{Mathf.CeilToInt(seconds)} s";
            }
        }
        
        private void OnTimerPausedChanged(bool isPaused)
        {
            foreach (var pauseIndicator in _pauseIndicators)
            {
                pauseIndicator.SetActive(isPaused);
            }
        }
        
        private void OnComboChanged(int combo)
        {
            var hasCombo = combo > 1;
            _comboParent.localScale = _tweenConfig.ComboScaleMultiplier * Vector3.one * combo;
            _comboParent.gameObject.SetActive(hasCombo);

            _comboShakeTween.DORestart();
            _comboShakeTween.DOPlay();
            
            if (!hasCombo)
            {
                return;
            }
            
            _comboText.text = string.Format(COMBO_TEXT_FORMAT, combo);
            _comboShakeTween.endValueV3 = _tweenConfig.ComboShakeEndValue * Vector3.one * combo;
            _comboPunchTween.endValueV3 = _tweenConfig.ComboScaleMultiplier * Vector3.one * combo;
            _comboPunchTween.DOPlay();
            _comboShakeTween.DOPlay();
        }
        
        private void OnScoreChanged(float score)
        {
            _scoreText.text = Mathf.CeilToInt(score).ToString();
            _scorePunchTween.DORestart();
            _scorePunchTween.DOPlay();
        }
    }
}