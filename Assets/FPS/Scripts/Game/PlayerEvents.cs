using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Game
{
    public class PlayerEvents : MonoBehaviour
    {
        public static PlayerEvents current;
        void Awake() => current = this;

        public static UnityAction<GameObject> OnLocalPlayerReady;
    }
}