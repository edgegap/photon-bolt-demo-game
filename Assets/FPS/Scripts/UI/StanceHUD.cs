using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class StanceHUD : MonoBehaviour
    {
        [Tooltip("Image component for the stance sprites")]
        public Image StanceImage;

        [Tooltip("Sprite to display when standing")]
        public Sprite StandingSprite;

        [Tooltip("Sprite to display when crouching")]
        public Sprite CrouchingSprite;

        void Awake() => PlayerEvents.OnLocalPlayerReady += OnLocalPlayerReady;

        void OnLocalPlayerReady(GameObject player)
        {
            if (PlayerCharacterController.LocalPlayer == null) return;
            if (!player.GetComponent<PlayerCharacterController>().IsLocalPlayer) return;

            PlayerCharacterController.LocalPlayer.OnStanceChanged += OnStanceChanged;
            OnStanceChanged(PlayerCharacterController.LocalPlayer.IsCrouching);
        }

        void OnStanceChanged(bool crouched)
        {
            StanceImage.sprite = crouched ? CrouchingSprite : StandingSprite;
        }
    }
}