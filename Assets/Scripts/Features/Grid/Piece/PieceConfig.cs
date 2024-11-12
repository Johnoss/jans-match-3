using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Features.Piece
{
    [CreateAssetMenu(menuName = "Create PieceConfig", fileName = "PieceConfig", order = 0)]
    public class PieceConfig : ScriptableObject
    {
        [FormerlySerializedAs("_piecePrefab")]
        [Header("Piece")]
        [SerializeField] private PieceEntityView _pieceEntityPrefab;
        [SerializeField] private PieceSetting[] _pieceSettings;
        
        public PieceEntityView PieceEntityPrefab => _pieceEntityPrefab;
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
        public Sprite MouthSprite;
        public Sprite EyesSprite;
        public Color EyesTintColor;
    }
}