using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [Tooltip("Image component dispplaying current health")]
        public Image HealthFillImage;

        Health m_PlayerHealth;

        void Awake() => PlayerEvents.OnLocalPlayerReady += OnLocalPlayerReady;

        void OnLocalPlayerReady(GameObject player)
        {
            if (PlayerCharacterController.LocalPlayer == null) return;
            if (!player.GetComponent<PlayerCharacterController>().IsLocalPlayer) return;

            m_PlayerHealth = PlayerCharacterController.LocalPlayer.GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerHealthBar>(m_PlayerHealth, this,
                m_PlayerHealth.gameObject);
        }

        void Update()
        {
            // update health bar value
            if (m_PlayerHealth != null) HealthFillImage.fillAmount = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
        }
    }
}