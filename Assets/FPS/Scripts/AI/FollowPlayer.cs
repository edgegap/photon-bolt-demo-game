using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    public class FollowPlayer : MonoBehaviour
    {
        Transform m_PlayerTransform;
        Vector3? m_OriginalOffset = null;

        void Awake() => PlayerEvents.OnLocalPlayerReady += OnLocalPlayerReady;

        void OnLocalPlayerReady(GameObject player)
        {
            ActorsManager actorsManager = FindObjectOfType<ActorsManager>();
            if (actorsManager != null && actorsManager.Player != null)
                m_PlayerTransform = actorsManager.Player.transform;
            else
            {
                enabled = false;
                return;
            }

            if(this != null) m_OriginalOffset = transform.position - m_PlayerTransform.position;
        }

        void LateUpdate()
        {
            if (m_PlayerTransform == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (m_OriginalOffset.HasValue) transform.position = m_PlayerTransform.position + m_OriginalOffset.Value;
        }

        private void OnDestroy()
        {
            PlayerEvents.OnLocalPlayerReady -= OnLocalPlayerReady;
        }
    }
}