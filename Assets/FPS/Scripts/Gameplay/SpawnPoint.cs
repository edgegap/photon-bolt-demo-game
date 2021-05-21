using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Gameplay {
    public class SpawnPoint : MonoBehaviour
    {
        public int nearbyPlayers = 0;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine("DecrementNearbyPlayers");
        }

        IEnumerator DecrementNearbyPlayers()
        {
            while (true)
            {
                if (nearbyPlayers > 0) nearbyPlayers--;
                yield return new WaitForSeconds(5f);
            }
        }
    }
}