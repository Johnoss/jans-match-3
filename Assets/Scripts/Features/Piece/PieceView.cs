using MVC;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.Piece
{
    [RequireComponent(typeof(RectTransform))]
    public class PieceView : AbstractView, IPoolableView
    {
        [Header("References")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _bodyImage;
        [SerializeField] private Image _eyesImage;
        [SerializeField] private Image _mouthImage;
        
        private int _entity;
        private ViewPool<PieceView> _viewPool;
        private PieceConfig _pieceConfig;
        
        private PieceSetting _pieceSetting;
        
        [Inject]
        public void Construct(int entity, PieceConfig pieceConfig, ViewPool<PieceView> viewPool)
        {
            _entity = entity;
            _pieceConfig = pieceConfig;
            _viewPool = viewPool;
            
            SetupVisuals();
        }
        
        private void SetupVisuals()
        {
            //TODO implement
            _pieceSetting = _pieceConfig.GetPieceSetting(0);
            
            _bodyImage.sprite = _pieceSetting.BodySprite;
            _eyesImage.color = _pieceSetting.EyesTintColor;
        }

        public void ResetView()
        {
            _mouthImage.overrideSprite = null;
            _eyesImage.overrideSprite = null;
            _bodyImage.overrideSprite = null;
            
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        public void DisableView()
        {
        }

        public class ViewFactory : PlaceholderFactory<int, PieceView>
        {
        }
    }
}