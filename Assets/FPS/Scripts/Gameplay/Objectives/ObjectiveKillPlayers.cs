using Unity.FPS.Game;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Unity.FPS.Gameplay
{
    [RequireComponent(typeof(ObjectiveKillPlayersEventListener))]
    public class ObjectiveKillPlayers : Objective
    {
        [Tooltip("If MustKillAllEnemies is false, this is the amount of enemy kills required")]
        public int KillsToCompleteObjective = 5;

        [Tooltip("Start sending notification about remaining enemies when this amount of enemies is left")]
        public int NotificationEnemiesRemainingThreshold = 3;

        public static List<int> playerScores = new List<int>();
        public int localPlayerIndex;

        ObjectiveCounter objectiveCounter;
        private ObjectiveKillPlayersEventListener killPlayersEventListener;
        private bool playerReady = false;

        protected override void Start()
        {
            base.Start();

            objectiveCounter = GameObject.FindObjectOfType<ObjectiveCounter>();
            if (objectiveCounter != null) objectiveCounter.SetString("");

            EventManager.AddListener<PlayerKillEvent>(OnPlayerKilled);

            // set a title and description specific for this type of objective, if it hasn't one
            if (string.IsNullOrEmpty(Title))
                Title = "Eliminate " + KillsToCompleteObjective.ToString() + " enemies";

            PlayerEvents.OnLocalPlayerReady += OnLocalPlayerReady;

            killPlayersEventListener = GetComponent<ObjectiveKillPlayersEventListener>();
            killPlayersEventListener.OnScoreUpdate += OnScoreUpdate;
        }

        void OnLocalPlayerReady(GameObject player)
        {
            if (!playerReady)
            {
                if (PlayerCharacterController.LocalPlayer == null) return;
                UpdateLocalPlayerIndex();
                PlayerCharacterController.LocalPlayer.state.AddCallback("PlayerAffiliation", UpdateLocalPlayerIndex);
            }
        }

        void UpdateLocalPlayerIndex()
        {
            if (PlayerCharacterController.LocalPlayer == null) return;
            localPlayerIndex = PlayerCharacterController.LocalPlayer.state.PlayerAffiliation - 1;
        }

        void OnPlayerKilled(PlayerKillEvent evt)
        {
            // Server on
            if (!BoltNetwork.IsServer) return;

            // Objective must not be complete already
            if (IsCompleted) return;

            // Only update objective if player was killed by someone
            if (evt.Killer != null)
            {
                Debug.Log("Sent score event");
                ScoreUpdateEvent scoreUpdate = ScoreUpdateEvent.Create();
                scoreUpdate.ScoreAffiliation = evt.Killer.GetComponent<Actor>().Affiliation;
                scoreUpdate.ScoreValue = playerScores[scoreUpdate.ScoreAffiliation-1] + 1;
                scoreUpdate.Send();
            }
        }

        private void OnScoreUpdate(ScoreUpdateEvent evt)
        {
            Debug.Log("Score event " + evt.ToString());
            int index = evt.ScoreAffiliation - 1;

            if(index < 0)
            {
                Debug.LogError("Affiliation index is negative");
                return;
            }

            while (index > playerScores.Count - 1) playerScores.Add(0);

            // Increment score
            playerScores[index]++;

            // Update counter if the local player is from the right team and the objective isn't complete
            if (objectiveCounter != null && index == localPlayerIndex)
            {
                objectiveCounter.SetString(playerScores[index].ToString());
            }// else Debug.LogFormat("localPlayerIndex false : {0} {1}", index, localPlayerIndex);

            int targetRemaining = KillsToCompleteObjective - playerScores[index];

            // update the objective text according to how many enemies remain to kill
            if (targetRemaining == 0)
            {
                CompleteObjective(string.Empty, GetUpdatedCounterAmount(index), "Objective complete : " + Title);
                GameFlowManager.WinnerAffiliation = evt.ScoreAffiliation;
            }
            else
            {
                if (targetRemaining == 1)
                {
                    string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                        ? "One player left to kill (Team #" + index + ")"
                        : string.Empty;

                    UpdateObjective(string.Empty, GetUpdatedCounterAmount(index), notificationText);
                }
                else
                {
                    // create a notification text if needed, if it stays empty, the notification will not be created
                    string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                        ? targetRemaining + " players to kill left (Team #" + index + ")"
                        : string.Empty;

                    UpdateObjective(string.Empty, GetUpdatedCounterAmount(index), notificationText);
                }
            }
        }

        string GetUpdatedCounterAmount(int index)
        {
            return playerScores[index] + " / " + KillsToCompleteObjective;
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<PlayerKillEvent>(OnPlayerKilled);
        }
    }
}