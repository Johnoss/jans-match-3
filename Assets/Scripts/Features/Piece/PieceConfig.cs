using System;
using UnityEngine;

namespace Scripts.Features.Piece
{
    [CreateAssetMenu(menuName = "Create PieceConfig", fileName = "PieceConfig", order = 0)]
    public class PieceConfig : ScriptableObject
    {
        [Header("Piece")]
        [SerializeField] private PieceView _piecePrefab;
        [SerializeField] private PieceSetting[] _pieceSettings;
        
        public PieceView PiecePrefab => _piecePrefab;
        public int PieceTypesCount => _pieceSettings.Length;
        
        public PieceSetting GetPieceSetting(int pieceType)
        {
            return _pieceSettings[pieceType];
        }
    }

    [Serializable]
    public class PieceSetting
    {
        public Sprite BodySprite;
        public Color EyesTintColor;
    }
}