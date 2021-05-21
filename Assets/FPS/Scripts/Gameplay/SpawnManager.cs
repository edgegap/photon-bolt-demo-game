using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Gameplay {
    public class SpawnManager : MonoBehaviour
    {
        public static List<SpawnPoint> SpawnPoints = null;
        void Start()
        {
            RefreshSpawnPoints();
        }

        static void RefreshSpawnPoints()
        {
            GameObject[] g_SpawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
            SpawnPoints = new List<SpawnPoint>();
            for (int i = 0; i < g_SpawnPoints.Length; i++)
            {
                SpawnPoint test = g_SpawnPoints[i].AddComponent<SpawnPoint>();
                SpawnPoints.Add(test);
            }
        }

        public static List<SpawnPoint> GetNewSpawnPoints()
        {
            RefreshSpawnPoints();
            return SpawnPoints;
        }
    }
}