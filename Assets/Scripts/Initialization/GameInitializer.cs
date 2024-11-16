using MVC;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using UnityEngine;
using Zenject;
using static Scripts.Features.Piece.PieceEntityView;

namespace Initialization
{
    public class GameInitializer
    {
        [Inject] private ViewFactory _viewFactory;
        
        [Inject] private EntityViewPool<PieceEntityView> _pieceEntityViewPool;
        
        [Inject] private GridService _gridService;
        [Inject] private MatchingService _matchingService;
        [Inject] private InputService _inputService;
        
        [Inject] private GridConfig _gridConfig;
        
        private readonly Transform _pooledObjectsParent;
        
        [Inject]
        public GameInitializer(Transform pooledObjectsParent)
        {
            _pooledObjectsParent = pooledObjectsParent;
        }
        
        [Inject]
        public void Init()
        {
            SetupPools();
            _gridService.SetupGrid();
            _inputService.SetupInput();
            _matchingService.CreateMatchValidatorEntity();
            _matchingService.CreatePossibleMovesEntity();
        }

        private void SetupPools()
        {
            _pieceEntityViewPool.SetupPool(_gridConfig.GridSize, _viewFactory, _pooledObjectsParent);
        }
    }
}