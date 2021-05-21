using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using Bolt;
using System;

namespace Unity.FPS.Gameplay
{
    public class PlayerManager : GlobalEventListener
    {
        public List<PlayerCharacterController> Players { get; private set; }
        public int NumberOfEnemiesTotal { get; private set; }
        public int TeamPoints { get; private set; } = 0;
        public int NumberOfPlayersRemaining => Players.Count;

        void Awake()
        {
            Players = new List<PlayerCharacterController>();
            PlayerEvents.OnLocalPlayerReady += OnLocalPlayerReady;
        }

        void OnLocalPlayerReady(GameObject player) => RegisterPlayer(player.GetComponent<PlayerCharacterController>());

        public void RegisterPlayer(PlayerCharacterController player)
        {
            if (!Players.Contains(player))
            {
                Players.Add(player);
                NumberOfEnemiesTotal++;
                ObjectiveKillPlayers.playerScores.Add(0);
                if (BoltNetwork.IsServer)
                {
                    Debug.LogFormat("Sent affiliation #{0}", ObjectiveKillPlayers.playerScores.Count);
                    PlayerAffiliationEvent affiliationEvent = PlayerAffiliationEvent.Create();
                    affiliationEvent.PlayerEntity = player.entity;
                    affiliationEvent.PlayerAffiliation = ObjectiveKillPlayers.playerScores.Count;
                    affiliationEvent.Send();
                }
            }
            else Debug.Log("Player already registered");
        }

        public override void OnEvent(PlayerAffiliationEvent evt)
        {
            foreach(PlayerCharacterController player in Players)
            {
                if (player == null) continue;
                else if (player.entity == evt.PlayerEntity)
                {
                    Debug.LogFormat("Received affiliation #{0}", evt.PlayerAffiliation);
                    player.SetAffiliation(evt.PlayerAffiliation);
                    return;
                }
            }
        }

        public void UnregisterPlayer(PlayerCharacterController playerKilled, GameObject killer, bool removeFromList = true)
        {
            // Send event for score calculation
            PlayerKillEvent evt = Events.PlayerKillEvent;
            evt.Player = playerKilled.gameObject;
            evt.Killer = killer;
            EventManager.Broadcast(evt);

            // removes the player from the list, so that we can keep track of how many are left on the map
            if(removeFromList) RemovePlayer(playerKilled);
        }

        public void RemovePlayer(PlayerCharacterController player)
        {
            if (Players.Contains(player))
            {
                Players.Remove(player);
                try
                {
                    ObjectiveKillPlayers.playerScores.Remove(ObjectiveKillPlayers.playerScores[player.GetComponent<Actor>().Affiliation]);
                } catch(NullReferenceException)
                {
                    Debug.LogWarning("Couldn't remove affiliate player score.");
                }
            }
        }
    }
}