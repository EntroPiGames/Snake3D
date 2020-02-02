using EntroPi;
using Snake3D.Player;
using Snake3D.WorldGrid;
using System.Collections;
using UnityEngine;

namespace Snake3D.Game
{
    public class GameController : EntroPiBehaviour
    {
        [SerializeField, RequiredReference]
        private CameraController m_CameraController = default;

        [SerializeField, RequiredReference]
        private GameScoreManager m_ScoreManager = default;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float m_UpdateInterval = 0.5f;

        [Header("World Grid")]
        [SerializeField, RequiredReference]
        private GameObject m_WorldGridPrefab = default;

        [SerializeField, RequiredReference]
        private WorldGridGenerator m_WorldGridGenerator = default;

        [Header("Player")]
        [SerializeField, RequiredReference]
        private GameObject m_PlayerPrefab = default;

        [Header("UI")]
        [SerializeField, RequiredReference]
        private GameMenuUI m_MenuUI = default;

        [SerializeField, RequiredReference]
        private GameScoreUI m_ScoreUI = default;

        private WorldGridController m_WorldGridController = default;
        private PlayerController m_PlayerController = default;

        private void OnEnable()
        {
            m_MenuUI.StartButtonClicked += OnMenuStartButtonClicked;
            m_MenuUI.ExitButtonClicked += OnMenuExitButtonClicked;
        }

        private void Start()
        {
            m_MenuUI.SetIsVisible(true);
            m_ScoreUI.SetIsVisible(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) == true)
            {
                RestartGame();
            }
        }

        private void OnDisable()
        {
            if (m_MenuUI != null)
            {
                m_MenuUI.StartButtonClicked -= OnMenuStartButtonClicked;
                m_MenuUI.ExitButtonClicked -= OnMenuExitButtonClicked;
            }
        }

        private void OnMenuStartButtonClicked()
        {
            RestartGame();
        }

        private void OnMenuExitButtonClicked()
        {
            Application.Quit();
        }

        private void StartNewGame()
        {
            Debug.Assert(m_WorldGridController == null && m_PlayerController == null, "Game already started!", this);

            m_PlayerController = CreatePlayer();
            if (m_PlayerController != null)
            {
                m_WorldGridController = CreateWorldGrid();
                if (m_WorldGridController != null && m_WorldGridController.GenerateWorldGrid(m_WorldGridGenerator) == true)
                {
                    m_ScoreManager.InitializeScore(m_WorldGridGenerator.PlayerPrefKey);
                    m_ScoreUI.SetIsVisible(true);

                    m_PlayerController.Initialize(m_WorldGridController.PlayerStartNode, m_WorldGridGenerator.PlayerStartDirection);
                    m_WorldGridController.UpdatePlayerTarget(m_PlayerController.PathNodes);

                    m_CameraController.Initialize(m_PlayerController, m_WorldGridController);

                    StartCoroutine(UpdateGameRoutine());
                }
            }
        }

        private void EndGame()
        {
            StopAllCoroutines();

            if (m_PlayerController != null)
            {
                Destroy(m_PlayerController.gameObject);
                m_PlayerController = null;
            }

            if (m_WorldGridController != null)
            {
                Destroy(m_WorldGridController.gameObject);
                m_WorldGridController = null;
            }

            m_MenuUI.SetIsVisible(false);
        }

        private void RestartGame()
        {
            EndGame();
            StartNewGame();
        }

        private WorldGridController CreateWorldGrid()
        {
            GameObject worldGridObject = Instantiate(m_WorldGridPrefab);
            WorldGridController worldGridController = worldGridObject.GetAndAssertComponent<WorldGridController>();

            return worldGridController;
        }

        private PlayerController CreatePlayer()
        {
            GameObject playerObject = Instantiate(m_PlayerPrefab);
            PlayerController playerController = playerObject.GetAndAssertComponent<PlayerController>();

            return playerController;
        }

        private IEnumerator UpdateGameRoutine()
        {
            float timer = 0;

            while (m_PlayerController != null && m_PlayerController.IsAlive == true)
            {
                while (timer < m_UpdateInterval)
                {
                    timer += Time.deltaTime;

                    yield return null;
                }

                timer -= m_UpdateInterval;

                UpdateGame();
            }

            OnPlayerDied();
        }

        private void UpdateGame()
        {
            m_PlayerController.MoveToNextPosition(m_WorldGridController);
            m_ScoreManager.OnPlayerMoved();

            if (m_PlayerController.ActiveNode == m_WorldGridController.PlayerTargetNode)
            {
                m_WorldGridController.UpdatePlayerTarget(m_PlayerController.PathNodes);
                m_PlayerController.IncreaseLength();

                m_CameraController.ShakeCamera(m_CameraController.LightShakeConfiguration);
                m_ScoreManager.OnPlayerReachedTarget();
            }

            m_CameraController.UpdateCameraRotation(m_PlayerController, m_WorldGridController.transform, m_UpdateInterval);
        }

        private void OnPlayerDied()
        {
            m_MenuUI.TitleText = m_MenuUI.GameOverTitle;
            m_MenuUI.MessageText = m_ScoreManager.IsNewHighScore == true ? m_MenuUI.NewHighScoreMessage : m_MenuUI.GameOverMessage;
            m_MenuUI.SetIsVisible(true);

            m_CameraController.ShakeCamera(m_CameraController.HeavyShakeConfiguration);
            m_ScoreManager.FinalizeScore(m_WorldGridGenerator.PlayerPrefKey);
        }
    }
}