using EntroPi;
using UnityEngine;

namespace Snake3D.Game
{
    [CreateAssetMenu(fileName = "GameScoreManager", menuName = "Snake 3D/Game/Score Manager")]
    public class GameScoreManager : EntroPiObject
    {
        public delegate void GameScoreManagerEventHandler(GameScoreManager gameScoreManager);

        [SerializeField]
        private int m_ScorePointsForFirstTarget = 200;

        [SerializeField]
        private int m_ScorePointsIncrementPerTarget = 100;

        [SerializeField]
        private int m_ScorePointsPerMove = 1;

        private int m_TargetScorePointIncrement;

        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }
        public bool IsNewHighScore { get { return CurrentScore > HighScore; } }

        public event GameScoreManagerEventHandler ScoreUpdated;

        public void InitializeScore(string playerPrefKey)
        {
            Debug.Assert(string.IsNullOrEmpty(playerPrefKey) == false, "Invalid player pref key");

            HighScore = PlayerPrefs.GetInt(playerPrefKey, 0);
            CurrentScore = 0;
            m_TargetScorePointIncrement = m_ScorePointsForFirstTarget;

            ScoreUpdated?.Invoke(this);
        }

        public void FinalizeScore(string playerPrefKey)
        {
            Debug.Assert(string.IsNullOrEmpty(playerPrefKey) == false, "Invalid player pref key");

            if (IsNewHighScore == true)
            {
                Debug.LogFormat("New HighScore: {0}", CurrentScore);
                PlayerPrefs.SetInt(playerPrefKey, CurrentScore);
            }
        }

        public void OnPlayerMoved()
        {
            CurrentScore += m_ScorePointsPerMove;

            ScoreUpdated?.Invoke(this);
        }

        public void OnPlayerReachedTarget()
        {
            CurrentScore += m_TargetScorePointIncrement;
            m_TargetScorePointIncrement += m_ScorePointsIncrementPerTarget;

            ScoreUpdated?.Invoke(this);
        }
    }
}