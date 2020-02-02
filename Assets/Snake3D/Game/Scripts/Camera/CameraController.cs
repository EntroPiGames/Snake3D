using EntroPi;
using Snake3D.Player;
using Snake3D.WorldGrid;
using System.Collections;
using UnityEngine;

namespace Snake3D.Game
{
    public class CameraController : EntroPiBehaviour
    {
        [SerializeField, RequiredReference]
        private Transform m_CameraTransform = default;

        [SerializeField]
        [Range(1, 10)]
        private float m_DistanceMultiplier = 2f;

        [SerializeField]
        [Range(0, 1)]
        private float m_CameraSpeed = 0.5f;

        [Header("Player Feedback")]
        [SerializeField]
        private CameraShakeConfiguration m_HeavyShakeConfiguration = default;

        [SerializeField]
        private CameraShakeConfiguration m_LightShakeConfiguration = default;

        private Coroutine m_CameraRotationRoutine;
        private Coroutine m_CameraShakeRoutine;

        public CameraShakeConfiguration HeavyShakeConfiguration { get { return m_HeavyShakeConfiguration; } }
        public CameraShakeConfiguration LightShakeConfiguration { get { return m_LightShakeConfiguration; } }

        public void Initialize(PlayerController playerController, WorldGridController worldGridController)
        {
            if (AreReferencesAssigned == true)
            {
                StopAllCoroutines();
                m_CameraRotationRoutine = null;
                m_CameraShakeRoutine = null;

                float cameraDistance = worldGridController.GridSize / 2f;
                cameraDistance *= m_DistanceMultiplier;
                m_CameraTransform.localPosition = new Vector3(0, 0, -cameraDistance);

                Quaternion lookAtRotation = CalculateCameraLookAtRotation(playerController, worldGridController.transform, Vector3.up);
                transform.rotation = lookAtRotation;
            }
        }

        public void UpdateCameraRotation(PlayerController playerController, Transform worldGridTransform, float intervalDuration)
        {
            if (AreReferencesAssigned == true)
            {
                Quaternion lookAtRotation = CalculateCameraLookAtRotation(playerController, worldGridTransform, m_CameraTransform.up);

                if (m_CameraRotationRoutine != null)
                {
                    StopCoroutine(m_CameraRotationRoutine);
                }

                m_CameraRotationRoutine = StartCoroutine(LerpCameraRotationRoutine(transform.rotation, lookAtRotation, intervalDuration));
            }
        }

        public void ShakeCamera(CameraShakeConfiguration shakeConfiguration)
        {
            if (AreReferencesAssigned == true)
            {
                if (m_CameraShakeRoutine != null)
                {
                    StopCoroutine(m_CameraShakeRoutine);
                }
                m_CameraShakeRoutine = StartCoroutine(ShakeCameraRoutine(shakeConfiguration));
            }
        }

        private IEnumerator LerpCameraRotationRoutine(Quaternion fromRotation, Quaternion toRotation, float duration)
        {
            float timer = 0;

            while (timer < duration)
            {
                float lerpValue = timer / duration;
                transform.rotation = Quaternion.Lerp(fromRotation, toRotation, lerpValue);

                timer += Time.deltaTime * m_CameraSpeed;

                yield return null;
            }

            m_CameraRotationRoutine = null;
        }

        private IEnumerator ShakeCameraRoutine(CameraShakeConfiguration shakeConfiguration)
        {
            if (shakeConfiguration != null)
            {
                float timer = 0;

                while (timer < shakeConfiguration.Duration)
                {
                    float shakeOffset = shakeConfiguration.CalculateCameraShakeOffsetForTime(timer);
                    SetCameraShakeOffset(shakeOffset);

                    timer += Time.deltaTime * m_CameraSpeed;
                    yield return null;
                }

                SetCameraShakeOffset(0);
            }

            m_CameraShakeRoutine = null;
        }

        private void SetCameraShakeOffset(float shakeOffset)
        {
            Vector3 cameraLocalPosition = m_CameraTransform.localPosition;
            cameraLocalPosition.y = shakeOffset;
            m_CameraTransform.localPosition = cameraLocalPosition;
        }

        private static Quaternion CalculateCameraLookAtRotation(PlayerController playerController, Transform worldGridTransform, Vector3 upVector)
        {
            Quaternion lookAtRotation = Quaternion.identity;

            if (playerController != null && playerController.ActiveNode != null && worldGridTransform != null)
            {
                Vector3 worldGridCenter = worldGridTransform.position;
                Vector3 playerPosition = playerController.ActiveNode.ViewPosition;

                lookAtRotation = Quaternion.LookRotation(worldGridCenter - playerPosition, upVector);
            }

            return lookAtRotation;
        }
    }
}