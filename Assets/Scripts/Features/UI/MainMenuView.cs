using MVC;
using Scripts.Features.GameSession;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.UI
{
    public class MainMenuView : AbstractView
    {
         private const string HIGH_SCORE_TEXT_FORMAT = "High Score: {0}";
        
        [Header("Interaction")]
        [SerializeField] private Button _playButton;
        
        [Header("HighScore")]
        [SerializeField] private TextMeshProUGUI[] _highScoreText;
        
        [Header("Game Over")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _successObject;
        [SerializeField] private GameObject _failureObject;
        
        [Inject] private GameUIView _gameUIView;

        [Inject] private GameSessionModel _gameSessionModel;
        [Inject] private GameSessionController _gameSessionController;
        
        [Inject] private CompositeDisposable _disposer;
        
        [Inject]
        public void Initialize()
        {
            _gameSessionModel.IsHighScoreBeaten.Skip(1).Subscribe(OnGameOver).AddTo(_disposer);
            _gameSessionModel.HighScore.Subscribe(UpdateHighScore).AddTo(_disposer);
            _playButton.OnClickAsObservable().Subscribe(_ => StartGame()).AddTo(_disposer);
            _gameSessionModel.IsGameRunning.Subscribe(UpdateVisibility).AddTo(_disposer);
        }
        
        private void UpdateHighScore(float highScore)
        {
            foreach (var highScoreText in _highScoreText)
            {
                highScoreText.text = string.Format(HIGH_SCORE_TEXT_FORMAT, highScore);
            }
        }
        
        private void OnGameOver(bool isSuccess)
        {
            _scoreText.text = $"{_gameSessionModel.Score.Value}";
            _successObject.SetActive(isSuccess);
            _failureObject.SetActive(!isSuccess);
        }

        private void UpdateVisibility(bool isRunning)
        {
            gameObject.SetActive(!isRunning);
        }

        private void StartGame()
        {
            _gameSessionController.StartGame();
        }
    }
}