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
        
        [Header("Possible Moves")]
        [SerializeField] private int _maxIterationsPerFrame = 10;
        [SerializeField] private float _possibleMoveCheckDelaySeconds = 3f;
        [SerializeField] private float _shuffleDelaySeconds = 1f;
        
        public int MaxIterationsPerFrame => _maxIterationsPerFrame;
        public float PossibleMoveCheckDelaySeconds => _possibleMoveCheckDelaySeconds;
        public float ShuffleDelaySeconds => _shuffleDelaySeconds;
        
        [Header("Game Session")]
        [SerializeField] private float _gameDurationSeconds = 3 * 60f;
        
        public float GameDurationSeconds => _gameDurationSeconds;
        
        [Header("Serialization")]
        [SerializeField] private string _highScoreKey = "Score";
        
        public string HighScoreKey => _highScoreKey;
        
        [Header("Performance")]
        [SerializeField] private int _targetFrameRate = 120;
        
        public int TargetFrameRate => _targetFrameRate;
    }
}
