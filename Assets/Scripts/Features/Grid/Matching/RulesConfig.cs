using UnityEngine;

namespace Scripts.Features.Grid.Matching
{
    public class RulesConfig : ScriptableObject
    {
        [Header("Matches")]
        [SerializeField] private int _minMatchLength = 3;
        [SerializeField] private bool _allowSquareMatches = true;
        public int MinMatchLength => _minMatchLength;
        public bool AllowSquareMatches => _allowSquareMatches;
        
        [Header("Score")]
        [SerializeField] private float _baseScore = 45;
        [Tooltip("Bonus for combo. BaseScore + (Combo * ComboAdditiveBonus)")]
        [SerializeField] private float _comboAdditiveBonus = 15;
        
        public float BaseScore => _baseScore;
        public float ComboAdditiveBonus => _comboAdditiveBonus;
        
        [Header("Possible Moves")]
        [SerializeField] private int _maxIterationsPerFrame = 10;
        [SerializeField] private float _possibleMoveCheckDelaySeconds = 3f;
        [SerializeField] private float _shuffleDelaySeconds = 1f;
        
        public int MaxIterationsPerFrame => _maxIterationsPerFrame;
        public float PossibleMoveCheckDelaySeconds => _possibleMoveCheckDelaySeconds;
        public float ShuffleDelaySeconds => _shuffleDelaySeconds;
        
        [Header("Game Session")]
        [SerializeField] private float _gameDurationSeconds = 3 * 60f;
        [SerializeField] private bool _showSeconds = true;
        
        public float GameDurationSeconds => _gameDurationSeconds;
        public bool ShowSeconds => _showSeconds;
        
        [Header("Serialization")]
        [SerializeField] private string _highScoreKey = "Score";
        
        public string HighScoreKey => _highScoreKey;
        
        [Header("Performance")]
        [SerializeField] private int _targetFrameRate = 120;
        
        public int TargetFrameRate => _targetFrameRate;
    }
}
