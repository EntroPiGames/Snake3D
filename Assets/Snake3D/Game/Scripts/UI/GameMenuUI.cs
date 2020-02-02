using EntroPi;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Snake3D.Game
{
    public class GameMenuUI : EntroPiBehaviour
    {
        [Header("UI Elements")]
        [SerializeField, RequiredReference]
        private Canvas m_Canvas = default;

        [SerializeField, RequiredReference]
        private TextMeshProUGUI m_TitleText = default;

        [SerializeField, RequiredReference]
        private TextMeshProUGUI m_MessageText = default;

        [SerializeField, RequiredReference]
        private Button m_StartButton = default;

        [SerializeField, RequiredReference]
        private Button m_ExitButton = default;

        [Header("Alternate text")]
        [SerializeField]
        private string m_GameOverTitle = "Game Over";

        [SerializeField, TextArea(3, 3)]
        private string m_GameOverMessage = string.Empty;

        [SerializeField, TextArea(3, 3)]
        private string m_NewHighScoreMessage = string.Empty;

        public string TitleText { set { if (AreReferencesAssigned == true) { m_TitleText.text = value; } } }
        public string MessageText { set { if (AreReferencesAssigned == true) { m_MessageText.text = value; } } }

        public string GameOverTitle { get { return m_GameOverTitle; } }
        public string GameOverMessage { get { return m_GameOverMessage; } }
        public string NewHighScoreMessage { get { return m_NewHighScoreMessage; } }

        public event Action StartButtonClicked;

        public event Action ExitButtonClicked;

        public void SetIsVisible(bool isVisible)
        {
            if (AreReferencesAssigned == true)
            {
                m_Canvas.gameObject.SetActive(isVisible);
            }
        }

        private void OnEnable()
        {
            m_StartButton.onClick.AddListener(() => StartButtonClicked?.Invoke());
            m_ExitButton.onClick.AddListener(() => ExitButtonClicked?.Invoke());
        }

        private void OnDisable()
        {
            if (m_StartButton != null)
            {
                m_StartButton.onClick.RemoveAllListeners();
            }
            if (m_ExitButton != null)
            {
                m_ExitButton.onClick.RemoveAllListeners();
            }
        }
    }
}