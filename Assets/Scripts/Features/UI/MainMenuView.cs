using MVC;
using Scripts.Features.GameSession;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.UI
{
    public class MainMenuView : AbstractView
    {
        [Header("Interaction")]
        [SerializeField] private Button _playButton;
        
        [Inject] private GameUIView _gameUIView;

        [Inject] private GameSessionModel _gameSessionModel;
        [Inject] private GameSessionController _gameSessionController;
        
        [Inject] private CompositeDisposable _disposer;
        
        [Inject]
        public void Initialize()
        {
            _playButton.OnClickAsObservable().Subscribe(_ => StartGame()).AddTo(_disposer);
            _gameSessionModel.IsGameRunning.Subscribe(UpdateVisibility).AddTo(_disposer);
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