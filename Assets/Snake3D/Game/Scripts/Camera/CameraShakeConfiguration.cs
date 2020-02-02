using EntroPi;
using UnityEngine;

namespace Snake3D.Game
{
    [CreateAssetMenu(fileName = "CameraShakeConfiguration", menuName = "Snake 3D/Game/Camera/Shake Configuration")]
    public class CameraShakeConfiguration : EntroPiObject
    {
        [SerializeField]
        [Range(0.1f, 2f)]
        private float m_Duration = 0.5f;

        [SerializeField]
        [Range(30f, 120f)]
        private float m_Frequency = 90f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_Strength = 0.5f;

        public float Duration { get { return m_Duration; } }

        public float CalculateCameraShakeOffsetForTime(float time)
        {
            float sineWave = Mathf.Sin(time * m_Frequency);
            float timeFade = 1 - Mathf.Clamp01(time / m_Duration);

            float cameraOffset = m_Strength * sineWave * timeFade;

            return cameraOffset;
        }
    }
}