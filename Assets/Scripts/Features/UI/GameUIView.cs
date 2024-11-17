using DG.Tweening;
using MVC;
using Scripts.Features.GameSession;
using Scripts.Features.Grid.Matching;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.UI
{
    public class GameUIView : AbstractView
    {
        [Header("Startup Tweens")]
        [SerializeField] private DOTweenAnimation[] _startupTweens;

        [Header("Timer")]
        [SerializeField] private RectTransform _timerParent;
        [SerializeField] private Image _timerFillImage;
        [SerializeField] private TextMeshProUGUI _timerText;
        
        [Inject] private GameSessionModel _gameSessionModel;
        [Inject] private RulesConfig _rulesConfig;
        
        [Inject] private CompositeDisposable _disposer;

        [Inject]
        private void Initialize()
        {
            SetDisposer(_disposer);
            _gameSessionModel.IsGameRunning.Subscribe(OnGameRunningChanged).AddTo(_disposer);
            _gameSessionModel.RemainingSeconds.Subscribe(OnRemainingSecondsChanged).AddTo(_disposer);
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
            
            //TODO tween / fade
            _timerParent.gameObject.SetActive(isRunning);
        }
        
        private void OnRemainingSecondsChanged(float seconds)
        {
            var fillAmount = seconds / _rulesConfig.GameDurationSeconds;
            _timerFillImage.fillAmount = fillAmount;
            _timerText.text = $"{Mathf.CeilToInt(seconds)} s";
        }
    }
}