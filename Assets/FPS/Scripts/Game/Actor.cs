using UnityEngine;
using System;
using Bolt;

namespace Unity.FPS.Game
{
    // This class contains general information describing an actor (player or enemies).
    // It is mostly used for AI detection logic and determining if an actor is friend or foe
    public class Actor : MonoBehaviour
    {
        //[Tooltip("Represents the affiliation (or team) of the actor. Actors of the same affiliation are friendly to each other")]
        public int Affiliation;

        [Tooltip("Represents point where other actors will aim when they attack this actor")]
        public Transform AimPoint;

        ActorsManager m_ActorsManager;

        void Awake() => PlayerEvents.OnLocalPlayerReady += AddActor;

        void Start()
        {
            AddActor(null);
        }

        void AddActor(GameObject player)
        {
            try
            {
                m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();

                // Register as an actor
                if (!m_ActorsManager.Actors.Contains(this))
                {
                    m_ActorsManager.Actors.Add(this);
                }
            }
            catch (NullReferenceException e)
            {
                if (m_ActorsManager == null) /*Debug.Log("Couldn't find Actors Manager");*/ return;
                else Debug.LogError(e);
            }
        }

        void OnDestroy()
        {
            // Unregister as an actor
            if (m_ActorsManager)
            {
                m_ActorsManager.Actors.Remove(this);
            }
        }
    }
}