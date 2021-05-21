using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

public class LocalObjectEnabler : MonoBehaviour
{
    [SerializeField] GameObject[] ObjectsToEnable;
    [SerializeField] GameObject[] ObjectsToDisable;

    void Awake()
    {
        PlayerEvents.OnLocalPlayerReady += OnLocalPlayerReady;
    }

    void OnLocalPlayerReady(GameObject player)
    {
        if (PlayerCharacterController.LocalPlayer == null) return;
        if (PlayerCharacterController.LocalPlayer.gameObject == gameObject)
        {
            foreach(GameObject Object in ObjectsToEnable)
            {
                Object.SetActive(true);
            }

            foreach (GameObject Object in ObjectsToDisable)
            {
                Object.SetActive(false);
            }
        }
    }
}
