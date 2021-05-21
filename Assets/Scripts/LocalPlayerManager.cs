using UnityEngine;
using Unity.FPS.Gameplay;

public class LocalPlayerManager : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject aimPoint;
    [SerializeField] SkinnedMeshRenderer thirdPersonMesh;

    void Awake()
    {
        // Activate components and objects if the character is controlled by the player
        if (GetComponent<PlayerCharacterController>().IsLocalPlayer)
        {
            if (thirdPersonMesh != null) thirdPersonMesh.enabled = false;
            if (playerCamera != null) playerCamera.SetActive(true);
            if (aimPoint != null) aimPoint.SetActive(true);
        }
    }
}