using EntroPi;
using TMPro;
using UnityEngine;

namespace Snake3D.Game
{
    public class GameScoreUI : EntroPiBehaviour
    {
        [SerializeField, RequiredReference]
        private Canvas m_Canvas = default;

        [SerializeField, RequiredReference]
        private GameScoreManager m_ScoreManager = default;

        [SerializeField, RequiredReference]
        private TextMeshProUGUI m_ScoreValueText = default;

        [SerializeField, RequiredReference]
        private TextMeshProUGUI m_HighScoreValueText = default;

        public void SetIsVisible(bool isVisible)
        {
            if (AreReferencesAssigned == true)
            {
                m_Canvas.gameObject.SetActive(isVisible);
            }
        }

        private void OnEnable()
        {
            OnScoreUpdated(m_ScoreManager);

            m_ScoreManager.ScoreUpdated += OnScoreUpdated;
        }

        private void OnDisable()
        {
            if (m_ScoreManager != null)
            {
                m_ScoreManager.ScoreUpdated -= OnScoreUpdated;
            }
        }

        private void OnScoreUpdated(GameScoreManager gameScoreManager)
        {
            m_ScoreValueText.text = gameScoreManager.CurrentScore.ToString();
            m_HighScoreValueText.text = gameScoreManager.HighScore.ToString();
        }
    }
}